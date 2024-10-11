using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    [SerializeField] // オーディオソース
    private AudioSource audioSource = null;

    [SerializeField] // ダメージ音
    private AudioClip hitAudio = null;


    public void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayOneShot_Hit()
    {
        audioSource.PlayOneShot(hitAudio);
    }
}
