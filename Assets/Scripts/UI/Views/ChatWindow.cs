using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ChatWindow : MonoBehaviour
{
    [SerializeField] // 名前テキスト
    private TextMeshProUGUI nameText = null;
    [SerializeField] // チャットテキスト
    private TextMeshProUGUI chatText = null;
    [SerializeField] // 次へボタン
    private Button nextBtn = null;
    [SerializeField] // 選択ボタン
    private List<Button> selectBtnList = new List<Button>();
    [SerializeField] // バトルウィンドウ
    private GameObject battleWindowObj = null;
    // 次へフラグ
    private bool isNext = false;


    private void Start()
    {
        nextBtn.onClick.AddListener(() => isNext = true);
    }

    private IEnumerator IInit(string name, List<string> textList)
    {
        // 名前
        nameText.text = name;

        // テキスト更新
        for(int i = 0; i < textList.Count; i++){
            nextBtn.gameObject.SetActive(false);
            isNext = false;
            chatText.text = textList[i];
            yield return new WaitForSeconds(0.5f);
            nextBtn.gameObject.SetActive(true);
            nextBtn.Select();
            yield return new WaitUntil(() => isNext);
        }

        battleWindowObj.SetActive(true);
        gameObject.SetActive(false);
        yield return null;
    }
    public void Init(string name, List<string> textList)
    {
        battleWindowObj.SetActive(false);
        gameObject.SetActive(true);

        StartCoroutine(IInit(name, textList));
    }
}
