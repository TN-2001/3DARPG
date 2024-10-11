using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudio : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] // オーディオソース
    private AudioSource audioSource = null;
    [SerializeField] // 選択音
    private AudioClip selectAudioClip = null;
    [SerializeField] // クリック音
    private AudioClip clickAudioClip = null;
    // 音開始できるか
    private bool isCanPlay = false;


    private void OnEnable()
    {
        StartCoroutine(StartCount());
    }

    private IEnumerator StartCount()
    {
        isCanPlay = false;

        yield return null;

        isCanPlay = true;
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayOneShot(selectAudioClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayOneShot_Instatiate(clickAudioClip);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlayOneShot_Instatiate(clickAudioClip);
    }

    private void PlayOneShot(AudioClip audioClip)
    {
        if(audioClip & isCanPlay){
            audioSource.PlayOneShot(audioClip);
        }
    }

    private void PlayOneShot_Instatiate(AudioClip audioClip)
    {
        if(audioClip & isCanPlay){
            GameObject obj = Instantiate(audioSource.gameObject);
            obj.GetComponent<AudioSource>().PlayOneShot(audioClip);
            Destroy(obj, 3f);
        }
    }
}
