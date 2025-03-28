using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour {
    [SerializeField] private Button firstSelectButton = null; // 初めのターゲットボタン
    [SerializeField] private Button endButton = null; // ゲーム終了ボタン
    [SerializeField] private LoadView loadView = null; // ロード画面


    private void OnEnable() {
        Time.timeScale = 0f;
        StartCoroutine(EOnEnable());
    }
    private IEnumerator EOnEnable() {
        yield return null;

        firstSelectButton.Select();
    }

    private void Start() {
        endButton.onClick.AddListener(OnEndButton);
    }

    private void OnDisable() {
        Time.timeScale = 1f;
    }


    // ボタン
    private void OnEndButton() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        loadView.LoadScene("Start");
#else
        Application.Quit();
#endif
    }
}
