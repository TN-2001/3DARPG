using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionDetector : MonoBehaviour {
    public string tagName = null; // 当たり判定の対象のタグ

    // 引数にColliderを持ったUnityEvent
    [HideInInspector] public UnityEvent<Collider> onTriggerEnter = null;
    [HideInInspector] public UnityEvent<Collider> onTriggerStay = null;
    [HideInInspector] public UnityEvent<Collider> onTriggerExit = null;


    private void OnTriggerEnter(Collider other) {
        if (tagName != "") {
            if (other.gameObject.CompareTag(tagName)) {
                onTriggerEnter?.Invoke(other);
            }
        } else {
            onTriggerEnter?.Invoke(other);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (tagName != "") {
            if (other.gameObject.CompareTag(tagName)) {
                onTriggerStay?.Invoke(other);
            }
        } else {
            onTriggerStay?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (tagName != "") {
            if (other.gameObject.CompareTag(tagName)) {
                onTriggerExit?.Invoke(other);
            }
        } else {
            onTriggerExit?.Invoke(other);
        }
    }
}
