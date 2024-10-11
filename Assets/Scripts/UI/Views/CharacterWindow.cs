using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWindow : MonoBehaviour
{
    [SerializeField] // アイテムコンテンツ
    private RectTransform contentTra = null;
    [SerializeField] // アイテムトグル
    private GameObject toggleObj = null;
    [SerializeField] // アイテム情報ビュー
    private View infoView = null;
    [SerializeField] // 現在の選択番号
    private int number = 0;


    private void OnEnable()
    {
        List<EnemyData> enemyList = new List<EnemyData>();
        for(int i = 0; i < GameManager.I.Data.IsFindEnemyList.Count; i++){
            if(GameManager.I.Data.IsFindEnemyList[i]){
                enemyList.Add(GameManager.I.DataBase.EnemyDataList[i]);
            }
        }
        InitContent(enemyList);

        Time.timeScale = 0f;
    }

    private void InitContent(List<EnemyData> enemyList)
    {
        // コンテンツ内をからに
        foreach(Transform child in contentTra){
            Destroy(child.gameObject);
        }

        // コンテンツ生成
        Toggle firstToggle = null;
        for(int i = 0; i < enemyList.Count; i++){
            GameObject obj = Instantiate(
                toggleObj, toggleObj.transform.position, Quaternion.identity, contentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI(enemyList[i].Name, enemyList[i].Image);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate(bool isOn){
                if(isOn){
                    number = int.Parse(obj.name);
                    OnSelect(enemyList[number]);
                }
            });
            toggle.group = contentTra.GetComponent<ToggleGroup>();

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
        }
    }

    private void OnSelect(EnemyData enemy)
    {
        // 情報ビュー
        string info = $"{enemy.Info}";
        infoView.UpdateUI(new List<string>(){enemy.Name, info}, enemy.Image);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
