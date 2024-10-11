using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemWindow : MonoBehaviour
{
    [SerializeField] // タブトグル
    private List<Toggle> tabToggleList = new List<Toggle>();
    [SerializeField] // アイテムコンテンツ
    private RectTransform itemContentTra = null;
    [SerializeField] // アイテムトグル
    private GameObject itemToggleObj = null;
    [SerializeField] // アイテム情報ビュー
    private View infoView = null;
    [SerializeField] // インプットボタン
    private List<Button> btnList = new List<Button>();
    [SerializeField] // 現在のタブ番号
    private int tabNumber = 0;
    [SerializeField] // 現在の選択番号
    private int number = 0;


    private void OnEnable()
    {
        foreach(Toggle child in tabToggleList){
            child.onValueChanged.RemoveAllListeners();
        }
        tabToggleList[0].onValueChanged.AddListener(delegate(bool isOn){
            if(isOn){
                tabNumber = 0;
                InitContent(GameManager.I.Data.ItemList.FindAll(x => x.Data.ItemType == ItemType.Recovery));
            }
        });
        tabToggleList[1].onValueChanged.AddListener(delegate(bool isOn){
            if(isOn){
                tabNumber = 1;
                InitContent(GameManager.I.Data.ItemList.FindAll(x => x.Data.ItemType == ItemType.None));
            }
        });
        tabToggleList[2].onValueChanged.AddListener(delegate(bool isOn){
            if(isOn){
                tabNumber = 2;
                InitContent(GameManager.I.Data.WeaponList);
            }
        });
        tabToggleList[3].onValueChanged.AddListener(delegate(bool isOn){
            if(isOn){
                tabNumber = 3;
                InitContent(GameManager.I.Data.ArmorList);
            }
        });

        // 初期化
        tabToggleList[tabNumber].onValueChanged.Invoke(true);

        Time.timeScale = 0f;
    }

    private void InitContent(List<Item> itemList)
    {
        // コンテンツ内をからに
        foreach(Transform child in itemContentTra){
            Destroy(child.gameObject);
        }

        // コンテンツ生成
        Toggle firstToggle = null;
        for(int i = 0; i < itemList.Count; i++){
            GameObject obj = Instantiate(
                itemToggleObj, itemToggleObj.transform.position, Quaternion.identity, itemContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI(itemList[i].Count.ToString(), itemList[i].Data.Image);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate(bool isOn){
                if(isOn){
                    number = int.Parse(obj.name);
                    OnSelect(itemList[number]);
                }
            });
            toggle.group = itemContentTra.GetComponent<ToggleGroup>();

            if(i == 0 | i == number){
                firstToggle = toggle;
            }
        }

        // 初期化
        if(firstToggle){
            infoView.gameObject.SetActive(true);
            firstToggle.Select();
            firstToggle.onValueChanged.Invoke(true);
        }
        else{
            infoView.gameObject.SetActive(false);
            UpdateCommand(new List<(string name, UnityAction action)>());
        }
    }
    private void InitContent(List<Weapon> weaponList)
    {
        // コンテンツ内をからに
        foreach(Transform child in itemContentTra){
            Destroy(child.gameObject);
        }

        // コンテンツ生成
        Toggle firstToggle = null;
        for(int i = 0; i < weaponList.Count; i++){
            GameObject obj = Instantiate(
                itemToggleObj, itemToggleObj.transform.position, Quaternion.identity, itemContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI($"Lv.{weaponList[i].Lev}", weaponList[i].Data.Image);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate(bool isOn){
                if(isOn){
                    number = int.Parse(obj.name);
                    OnSelect(weaponList[number]);
                }
            });
            toggle.group = itemContentTra.GetComponent<ToggleGroup>();

            if(i == 0 | i == number){
                firstToggle = toggle;
            }
        }

        // 初期化
        if(firstToggle){
            infoView.gameObject.SetActive(true);
            firstToggle.Select();
            firstToggle.onValueChanged.Invoke(true);
        }
        else{
            infoView.gameObject.SetActive(false);
            UpdateCommand(new List<(string name, UnityAction action)>());
        }
    }
    private void InitContent(List<Armor> armorList)
    {
        // コンテンツ内をからに
        foreach(Transform child in itemContentTra){
            Destroy(child.gameObject);
        }

        // コンテンツ生成
        Toggle firstToggle = null;
        for(int i = 0; i < armorList.Count; i++){
            GameObject obj = Instantiate(
                itemToggleObj, itemToggleObj.transform.position, Quaternion.identity, itemContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI($"Lv.{armorList[i].Lev}", armorList[i].Data.Image);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate(bool isOn){
                if(isOn){
                    number = int.Parse(obj.name);
                    OnSelect(armorList[number]);
                }
            });
            toggle.group = itemContentTra.GetComponent<ToggleGroup>();

            if(i == 0 | i == number){
                firstToggle = toggle;
            }
        }

        // 初期化
        if(firstToggle){
            infoView.gameObject.SetActive(true);
            firstToggle.Select();
            firstToggle.onValueChanged.Invoke(true);
        }
        else{
            infoView.gameObject.SetActive(false);
            UpdateCommand(new List<(string name, UnityAction action)>());
        }
    }

    private void OnSelect(Item item)
    {
        // 情報ビュー
        string info = $"{item.Data.Info}";
        infoView.UpdateUI(new List<string>(){item.Data.Name, info}, item.Data.Image);

        // コマンドボタン
        UpdateCommand(new List<(string name, UnityAction action)>(){
            ("換金", delegate{
            })
        });
    }
    private void OnSelect(Weapon weapon)
    {
        // 情報ビュー
        string info = $"攻撃力+{weapon.Atk}\n{weapon.Data.Info}";
        infoView.UpdateUI(new List<string>(){weapon.Data.Name, info}, weapon.Data.Image);

        // コマンドボタン
        UpdateCommand(new List<(string name, UnityAction action)>());
    }
    private void OnSelect(Armor armor)
    {
        // 情報ビュー
        string info = $"HP+{armor.Hp}\n{armor.Data.Info}";
        infoView.UpdateUI(new List<string>(){armor.Data.Name, info}, armor.Data.Image);

        // コマンドボタン
        UpdateCommand(new List<(string name, UnityAction action)>());
    }

    private void UpdateCommand(List<(string name, UnityAction action)> actionList)
    {
        // インプットボタンを初期化
        foreach(Button child in btnList){
            child.onClick.RemoveAllListeners();
            child.gameObject.SetActive(false);
        }

        // インプットボタンの更新
        for(int i = 0; i < actionList.Count; i++){
            btnList[i].GetComponent<View>().UpdateUI(actionList[i].name);
            btnList[i].onClick.AddListener(actionList[i].action);
            btnList[i].gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
