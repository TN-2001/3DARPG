using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatWindow : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI nameText = null; // 名前テキスト
    [SerializeField] private TextMeshProUGUI chatText = null; // チャットテキスト
    [SerializeField] private Button nextBtn = null; // 次へボタン
    [SerializeField] private List<Button> selectBtnList = new(); // 選択ボタン
    [SerializeField] private GameObject battleWindowObj = null; // バトルウィンドウ

    private bool isNext = false; // 次へフラグ


    private void Start() {
        nextBtn.onClick.AddListener(() => isNext = true);
    }

    private IEnumerator IInit(string name, List<string> textList) {
        // 名前
        nameText.text = name;

        // テキスト更新
        for (int i = 0; i < textList.Count; i++) {
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
    public void Init(string name, List<string> textList) {
        battleWindowObj.SetActive(false);
        gameObject.SetActive(true);

        StartCoroutine(IInit(name, textList));
    }
}
