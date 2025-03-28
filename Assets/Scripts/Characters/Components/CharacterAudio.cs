using UnityEngine;

public class CharacterAudio : MonoBehaviour {
    // コンポーネント
    private AudioSource audioSource = null; // オーディオソース

    // 音
    [SerializeField] private AudioClip hitAudio = null; // ダメージ音


    private void Start() {
        // コンポーネント
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayOneShot(AudioClip audioClip) {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayOneShot_Hit() {
        audioSource.PlayOneShot(hitAudio);
    }
}
