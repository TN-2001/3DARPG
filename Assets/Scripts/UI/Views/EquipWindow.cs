using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EquipWindow : MonoBehaviour {
    [SerializeField] private RectTransform itemContentTra = null; // アイテムコンテンツ
    [SerializeField] private GameObject itemToggleObj = null; // アイテムトグル
    [SerializeField] private View infoView = null; // アイテム情報ビュー
    [SerializeField] private List<Button> btnList = new(); // インプットボタン
    [SerializeField] private UnityEvent<Weapon> onChangeWeapon = new();

    private SaveData data = null;
    public int typeNumber = 0; // タイプ番号
    private int number = 0; // 現在の選択番号


    private void OnEnable() {
        Time.timeScale = 0f;

        data = DataManager.Instance.Data;

        if (typeNumber == 0) {
            InitContent(data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Head));
        } else if (typeNumber == 1) {
            InitContent(data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Chest));
        } else if (typeNumber == 2) {
            InitContent(data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Arm));
        } else if (typeNumber == 3) {
            InitContent(data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Leg));
        } else {
            InitContent(data.WeaponList);
        }
    }

    private void InitContent(List<Weapon> weaponList) {
        // コンテンツ内をからに
        foreach (Transform child in itemContentTra) {
            Destroy(child.gameObject);
        }

        // コンテンツ生成
        Toggle firstToggle = null;
        for (int i = 0; i < weaponList.Count; i++) {
            GameObject obj = Instantiate(
                itemToggleObj, itemToggleObj.transform.position, Quaternion.identity, itemContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI($"Lv.{weaponList[i].Lev}", weaponList[i].Data.Image);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate (bool isOn) {
                if (isOn) {
                    number = int.Parse(obj.name);
                    OnSelect(weaponList[number]);
                }
            });
            toggle.group = itemContentTra.GetComponent<ToggleGroup>();

            if (i == 0 | i == number) {
                firstToggle = toggle;
            }
        }

        // 初期化
        if (firstToggle) {
            infoView.gameObject.SetActive(true);
            firstToggle.Select();
            firstToggle.onValueChanged.Invoke(true);
        } else {
            infoView.gameObject.SetActive(false);
            UpdateCommand(new List<(string name, UnityAction action)>());
        }
    }
    private void InitContent(List<Armor> armorList) {
        // コンテンツ内をからに
        foreach (Transform child in itemContentTra) {
            Destroy(child.gameObject);
        }

        // コンテンツ生成
        Toggle firstToggle = null;
        for (int i = 0; i < armorList.Count; i++) {
            GameObject obj = Instantiate(
                itemToggleObj, itemToggleObj.transform.position, Quaternion.identity, itemContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI($"Lv.{armorList[i].Lev}", armorList[i].Data.Image);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate (bool isOn) {
                if (isOn) {
                    number = int.Parse(obj.name);
                    OnSelect(armorList[number]);
                }
            });
            toggle.group = itemContentTra.GetComponent<ToggleGroup>();

            if (i == 0 | i == number) {
                firstToggle = toggle;
            }
        }

        // 初期化
        if (firstToggle) {
            infoView.gameObject.SetActive(true);
            firstToggle.Select();
            firstToggle.onValueChanged.Invoke(true);
        } else {
            infoView.gameObject.SetActive(false);
            UpdateCommand(new List<(string name, UnityAction action)>());
        }
    }

    private void OnSelect(Weapon weapon) {
        // 情報ビュー
        string info = $"攻撃力+{weapon.Atk}";
        if (weapon.Recovery > 0) {
            info = $"{info}\n回復力：{weapon.Atk}";
        }
        if (weapon.GuardTime > 0) {
            info = $"{info}\nシールド：{weapon.GuardTime}秒";
        }
        info = $"{info}\n{weapon.Data.Info}";
        infoView.UpdateUI(new List<string>() { weapon.Data.Name, info }, weapon.Data.Image);

        // コマンドボタン
        if (weapon.EquipNumber != typeNumber - 3) {
            UpdateCommand(new List<(string name, UnityAction action)>(){
                ("装備", delegate{
                    data.EquipWeapon(weapon, typeNumber-3);
                    OnSelect(weapon);
                    onChangeWeapon.Invoke(weapon);
                }),
                ("", null),
                // ("強化", delegate{

                // })
            });
        } else if (weapon.EquipNumber > 0) {
            UpdateCommand(new List<(string name, UnityAction action)>(){
                ("", null),
                ("", null),
                // ("強化", delegate{

                // })
            });
        } else {
            UpdateCommand(new List<(string name, UnityAction action)>(){
                ("", null),
                ("", null),
                // ("強化", delegate{

                // })
            });
        }
    }
    private void OnSelect(Armor armor) {
        // 情報ビュー
        string info = $"HP+{armor.Hp}\n{armor.Data.Info}";
        infoView.UpdateUI(new List<string>() { armor.Data.Name, info }, armor.Data.Image);

        // コマンドボタン
        if (!armor.IsEquip) {
            UpdateCommand(new List<(string name, UnityAction action)>(){
                ("装備", delegate{
                    data.EquipArmor(armor, true);
                    OnSelect(armor);
                }),
                ("", null),
                // ("強化", delegate{

                // })
            });
        } else {
            UpdateCommand(new List<(string name, UnityAction action)>(){
                ("はずす", delegate{
                    data.EquipArmor(armor, false);
                    OnSelect(armor);
                }),
                ("", null),
                // ("強化", delegate{

                // })
            });
        }
    }

    private void UpdateCommand(List<(string name, UnityAction action)> actionList) {
        // インプットボタンを初期化
        foreach (Button child in btnList) {
            child.onClick.RemoveAllListeners();
            child.gameObject.SetActive(false);
        }

        // インプットボタンの更新
        for (int i = 0; i < actionList.Count; i++) {
            if (actionList[i].name != "") {
                btnList[i].GetComponent<View>().UpdateUI(actionList[i].name);
                btnList[i].onClick.AddListener(actionList[i].action);
                btnList[i].gameObject.SetActive(true);
            }
        }
    }

    private void OnDisable() {
        Time.timeScale = 1f;
    }
}
