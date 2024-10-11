using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] // 当たり判定の対象のタグ
    public string tagName = null;

    [HideInInspector] // 引数にColliderを持ったUnityEvent
    public UnityEvent<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == tagName | tagName == ""){
            onTriggerEnter?.Invoke(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == tagName | tagName == ""){
            onTriggerStay?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.tag == tagName | tagName == ""){
            onTriggerExit?.Invoke(other);
        }
    }
}
