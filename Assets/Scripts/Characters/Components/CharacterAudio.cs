using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    // コンポーネント
    private AudioSource audioSource = null; // オーディオソース

    [SerializeField] private AudioClip hitAudio = null; // ダメージ音


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayOneShot_Hit()
    {
        audioSource.PlayOneShot(hitAudio);
    }
}
