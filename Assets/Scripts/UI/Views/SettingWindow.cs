using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour {
    [SerializeField] private Slider bgmSlider = null; // BGM
    [SerializeField] private Slider seSlider = null; // SE

    private SaveData data = null;


    private void OnEnable() {
        Time.timeScale = 0f;

        bgmSlider.Select();
    }

    private void Start() {
        data = DataManager.Instance.Data;

        bgmSlider.maxValue = 100;
        bgmSlider.value = data.VolumeList[0];
        seSlider.maxValue = 100;
        seSlider.value = data.VolumeList[1];

        bgmSlider.onValueChanged.AddListener(delegate (float volume) {
            data.UpdateVolume((int)volume, 0);
            AudioManager.Instance.AudioMixer.SetFloat("BGM", data.VolumeList[0] - 80f);
        });
        seSlider.onValueChanged.AddListener(delegate (float volume) {
            data.UpdateVolume((int)volume, 1);
            AudioManager.Instance.AudioMixer.SetFloat("SE", data.VolumeList[1] - 80f);
        });
    }

    private void OnDisable() {
        Time.timeScale = 1f;
    }
}
