using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    [SerializeField] private Button firstSelectButton = null; // 初めのターゲットボタン
    [SerializeField] private Button endButton = null; // ゲーム終了ボタン


    private void OnEnable()
    {
        firstSelectButton.Select();

        Time.timeScale = 0f;
    }

    private void Start()
    {
        endButton.onClick.AddListener(OnEndButton);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }


    // ボタン
    private void OnEndButton()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
