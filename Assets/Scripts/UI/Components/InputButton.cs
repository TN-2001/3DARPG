using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputButton : MonoBehaviour {
    // ボタン
    private Button button = null;
    [SerializeField] // 入力
    private InputAction inputAction = null;


    private void OnEnable() {
        inputAction.performed += OnPerformed;
        inputAction.Enable();
    }

    private void Start() {
        if (GetComponent<Button>()) {
            button = GetComponent<Button>();
        }
    }

    private void OnPerformed(InputAction.CallbackContext context) {
        ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }

    private void OnDisable() {
        inputAction.performed -= OnPerformed;
        inputAction.Disable();
    }
}
