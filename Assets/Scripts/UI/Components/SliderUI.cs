using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderUI : MonoBehaviour {
    // スライダー
    private Slider slider = null;
    [SerializeField] // アニメスライダー
    private Slider animSlider = null;
    [SerializeField] // スピード
    private float animSpeed = 10f;
    [SerializeField] // 最大値のときオフするか
    private bool isMaxToOff = false;
    // オフにする画像
    private Image offImage = null;
    // オフにするオブジェクト
    private List<GameObject> offObjList = new List<GameObject>();
    [SerializeField] // テキスト
    private TextMeshProUGUI text = null;
    [SerializeField] // 値テキスト
    private TextMeshProUGUI valueText = null;
    [SerializeField] // 最大値テキスト
    private TextMeshProUGUI maxValueText = null;
    // 値
    private float value = 0;
    // 最大値
    private float maxValue = 0;


    private void Start() {
        slider = GetComponent<Slider>();
        if (isMaxToOff) {
            offImage = GetComponent<Image>();
            foreach (Transform child in transform) {
                offObjList.Add(child.gameObject);
            }
        }
    }

    private void LateUpdate() {
        if (animSlider) {
            // 値変化時にアニメーション
            if (animSlider.value > slider.value) {
                animSlider.value -= 1f / animSpeed * slider.maxValue * Time.deltaTime;
            } else if (animSlider.value < slider.value) {
                animSlider.value = slider.value;
            }
        }

        // 値変化時
        if (value != slider.value) {
            if (animSlider) {
                animSlider.value = value;
            }

            // テキスト更新
            if (text) {
                text.text = $"{(int)slider.value}/{(int)slider.maxValue}";
            }
            if (valueText) {
                valueText.text = $"{(int)slider.value}";
            }

            // オンオフ
            if (isMaxToOff) {
                if (slider.value < slider.maxValue) {
                    offImage.enabled = true;
                    foreach (GameObject obj in offObjList) {
                        obj.SetActive(true);
                    }
                } else if (slider.value == slider.maxValue) {
                    offImage.enabled = false;
                    foreach (GameObject obj in offObjList) {
                        obj.SetActive(false);
                    }
                }
            }

            value = slider.value;
        }

        // 最大値の変化時
        if (maxValue != slider.maxValue) {
            if (animSlider) {
                animSlider.maxValue = slider.maxValue;
            }

            // テキスト更新
            if (text) {
                text.text = $"{(int)slider.value}/{(int)slider.maxValue}";
            }
            if (maxValueText) {
                maxValueText.text = $"{(int)slider.maxValue}";
            }

            maxValue = slider.maxValue;
        }
    }
}
