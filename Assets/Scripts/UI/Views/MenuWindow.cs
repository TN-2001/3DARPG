using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuWindow : MonoBehaviour
{
    [SerializeField] // 初めのターゲットボタン
    private Button firstSelectBtn = null;
    [SerializeField] // ゲーム終了ボタン
    private Button endBtn = null;


    private void OnEnable()
    {
        firstSelectBtn.Select();

        Time.timeScale = 0f;
    }

    private void Start()
    {
        endBtn.onClick.AddListener(delegate{
            SceneManager.LoadScene("Start");
            Time.timeScale = 1f;
        });
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
