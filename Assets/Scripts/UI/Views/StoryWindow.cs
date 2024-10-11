using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryWindow : MonoBehaviour
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
        List<EventData> eventDataList = new List<EventData>();
        for(int i = 0; i < GameManager.I.Data.EventNumber; i++){
            eventDataList.Add(GameManager.I.DataBase.EventDataList[i]);
        }
        InitContent(eventDataList);

        Time.timeScale = 0f;
    }

    private void InitContent(List<EventData> eventDataList)
    {
        // コンテンツ内をからに
        foreach(Transform child in contentTra){
            Destroy(child.gameObject);
        }

        // コンテンツ生成
        Toggle firstToggle = null;
        for(int i = 0; i < eventDataList.Count; i++){
            GameObject obj = Instantiate(
                toggleObj, toggleObj.transform.position, Quaternion.identity, contentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI(eventDataList[i].Name);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate(bool isOn){
                if(isOn){
                    number = int.Parse(obj.name);
                    OnSelect(eventDataList[number]);
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

    private void OnSelect(EventData enemy)
    {
        // 情報ビュー
        string info = $"{enemy.Info}";
        infoView.UpdateUI(new List<string>(){enemy.Name, info});
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
