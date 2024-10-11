using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingWindow : MonoBehaviour
{
    [SerializeField] // BGM
    private Slider bgmSlider = null;
    [SerializeField] // SE
    private Slider seSlider = null;
    [SerializeField] // オーディオミキサー
    private AudioMixer audioMixer = null;


    private void OnEnable()
    {
        Time.timeScale = 0f;

        bgmSlider.Select();
    }

    private void Start()
    {
        bgmSlider.maxValue = 100;
        bgmSlider.value = GameManager.I.Data.VolumeList[0];
        seSlider.maxValue = 100;
        seSlider.value = GameManager.I.Data.VolumeList[1];

        bgmSlider.onValueChanged.AddListener(delegate(float volume){
            GameManager.I.Data.UpdateVolume((int)volume, 0);
            audioMixer.SetFloat("BGM", GameManager.I.Data.VolumeList[0] - 80f);
        });
        seSlider.onValueChanged.AddListener(delegate(float volume){
            GameManager.I.Data.UpdateVolume((int)volume, 1);
            audioMixer.SetFloat("SE", GameManager.I.Data.VolumeList[1] - 80f);
        });
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
