using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerWindow : MonoBehaviour {
    [SerializeField] private List<Toggle> toggleList = new(); // トグルリスト
    [SerializeField] private View playerView = null; // プレイヤービュー
    [SerializeField] private View infoView = null; // アイテム情報ビュー
    [SerializeField] private List<Button> btnList = new(); // コマンドボタン
    [SerializeField] private EquipWindow equipWindow = null; // 装備ウィンドウ

    private SaveData data = null; // データ
    private int number = 0; // 番号


    private void OnEnable() {
        Time.timeScale = 0f;

        data = DataManager.Instance.Data;

        for (int i = 0; i < 4; i++) {
            Armor armor = data.Player.ArmorList[i];
            if (armor != null) {
                toggleList[i].GetComponent<View>().UpdateUI(new List<Sprite>() { armor.Data.Image });
            } else {
                toggleList[i].GetComponent<View>().UpdateUI("");
            }
        }
        Weapon weapon = data.Player.Weapon;
        toggleList[4].GetComponent<View>().UpdateUI(new List<Sprite>() { weapon.Data.Image });

        Player player = data.Player;
        playerView.UpdateUI(new List<string>(){
            $"HP\n攻撃力",
            $"{player.Hp}\n{player.Atk}"
        });

        StartCoroutine(IOnEnable());
    }
    private IEnumerator IOnEnable() {
        yield return null;
        toggleList[number].Select();
        OnSelect();

        while (true) {
            yield return new WaitUntil(() => !EventSystem.current.currentSelectedGameObject);
            EventSystem.current.SetSelectedGameObject(toggleList[number].gameObject);
        }
    }

    private void Start() {
        // トグルリストの初期化
        for (int i = 0; i < toggleList.Count; i++) {
            int f = i;
            toggleList[i].onValueChanged.AddListener(delegate (bool isOn) {
                if (isOn) {
                    number = f;
                    equipWindow.typeNumber = number;
                    OnSelect();
                }
            });
        }
    }

    private void OnSelect() {
        // 情報ビュー
        if (number < 4) {
            Armor armor = data.Player.ArmorList[number];
            if (armor != null) {
                // 情報ビュー
                string info = $"HP+{armor.Hp}\n{armor.Data.Info}";
                infoView.UpdateUI(new List<string>() { armor.Data.Name, info }, armor.Data.Image);
            } else {
                infoView.UpdateUI(new List<string>());
            }
        } else {
            Weapon weapon = data.Player.Weapon;
            if (weapon != null) {
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
            } else {
                infoView.UpdateUI(new List<string>());
            }
        }

        // コマンドボタン
        if ((number == 0 & data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Head).Count == 0)
            | (number == 1 & data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Chest).Count == 0)
            | (number == 2 & data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Arm).Count == 0)
            | (number == 3 & data.ArmorList.FindAll(x => x.Data.ArmorType == ArmorType.Leg).Count == 0)) {
            UpdateCommand(new List<(string name, UnityAction action)>());
        } else {
            UpdateCommand(new List<(string name, UnityAction action)>(){
                ("装備変更", delegate{

                })
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
        StopCoroutine(IOnEnable());

        Time.timeScale = 1f;
    }
}
