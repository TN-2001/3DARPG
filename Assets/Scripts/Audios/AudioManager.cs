using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioManager", menuName = "ScriptableObject/AudioManager")]
public class AudioManager : SingletonScriptableObject<AudioManager>
{
    [SerializeField] private AudioMixer audioMixer = null; // オーディオミキサー
    public AudioMixer AudioMixer => audioMixer;
    public AudioSource SEAudioSource { get; set; } // SEオーディオ


    public void PlaySE(AudioClip clip) // SEを鳴らす
    {
        if(SEAudioSource != null){
            SEAudioSource.PlayOneShot(clip);
        }
    }
}
