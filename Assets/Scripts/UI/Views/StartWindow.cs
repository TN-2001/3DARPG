using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartWindow : MonoBehaviour
{
    [SerializeField] // スタートボタン
    private Button startBtn = null;
    [SerializeField] // ロード画像
    private Transform loadImgTra = null;


    private void Start()
    {
        startBtn.Select();
        startBtn.onClick.AddListener(delegate{StartCoroutine(EStart());});
    }

    private IEnumerator EStart()
    {
        startBtn.interactable = false;
        loadImgTra.gameObject.SetActive(true);

        SceneManager.LoadSceneAsync("Main");

        while(true){
            yield return null;
            loadImgTra.Rotate(new Vector3(0,0,360*Time.deltaTime));
        }
    }
}
