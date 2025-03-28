using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadView : MonoBehaviour {
    [SerializeField] private Image image = null; // 背景画像
    [SerializeField] private Transform loadImageTransform = null; // ロード画像
    [SerializeField] private float imageAnimTime = 2f;

    public string sceneName = ""; // シーン名


    private void Start() {
        StartCoroutine(EImageDisable());
    }


    //　イベント実行
    public IEnumerator ELoadEvent(Action action) {
        yield return EImageEnable();

        action();

        // ロード画像を回転
        float countTime = 0f;
        while (countTime < 1f) {
            loadImageTransform.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
            countTime += Time.deltaTime;
            yield return null;
        }

        yield return EImageDisable();
    }

    // シーンロード
    public void LoadScene(string sceneName) {
        this.sceneName = sceneName;

        StartCoroutine(ELoadScene());
    }
    public void LoadScene() {
        StartCoroutine(ELoadScene());
    }

    private IEnumerator ELoadScene() {
        yield return EImageEnable();

        loadImageTransform.gameObject.SetActive(true); // ロード画像を起動

        yield return null;

        SceneManager.LoadSceneAsync(sceneName); // メインシーンをロード

        // ロード画像を回転
        while (true) {
            loadImageTransform.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
            yield return null;
        }
    }


    // 背景画像
    private IEnumerator EImageEnable() {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        image.gameObject.SetActive(true);

        yield return null;

        while (image.color.a < 1f) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + 1f * Time.deltaTime / imageAnimTime);
            yield return null;
        }
    }

    private IEnumerator EImageDisable() {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        image.gameObject.SetActive(true);

        yield return null;

        while (image.color.a > 0f) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - 1f * Time.deltaTime / imageAnimTime);
            yield return null;
        }

        image.gameObject.SetActive(false);
    }
}
