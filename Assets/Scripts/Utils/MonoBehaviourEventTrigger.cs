using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviourEventTrigger : MonoBehaviour {
    public UnityEvent onAwake = new();
    public UnityEvent onEnable = new();
    public UnityEvent onStart = new();
    public UnityEvent onDisable = new();
    public UnityEvent onDestroy = new();


    private void Awake() {
        onAwake.Invoke();
    }

    private void OnEnable() {
        onEnable.Invoke();
    }

    private void Start() {
        onStart.Invoke();
    }

    private void OnDisable() {
        onDisable.Invoke();
    }

    private void OnDestroy() {
        onDestroy.Invoke();
    }
}
