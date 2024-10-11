using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputToggleGroup : MonoBehaviour
{
    // トグル
    private List<Toggle> toggleList = new List<Toggle>();
    [SerializeField] // 進む入力
    private InputAction forwardAction = null;
    [SerializeField] // 戻る入力
    private InputAction returnAction = null;


    private void OnEnable()
    {
        forwardAction.performed += OnForward;
        returnAction.performed += OnReturn;

        forwardAction.Enable();
        returnAction.Enable();
    }

    private void Start()
    {
        // トグルを入手
        for(int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).GetComponent<Toggle>()){
                toggleList.Add(transform.GetChild(i).GetComponent<Toggle>());
            }
        }
    }

    private void OnForward(InputAction.CallbackContext context)
    {
        for(int i = 0; i < toggleList.Count; i++){
            if(toggleList[i].isOn & i+1 < toggleList.Count){
                ExecuteEvents.Execute(toggleList[i+1].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                // toggleList[i+1].Select();
                break;
            }
        }
    }

    private void OnReturn(InputAction.CallbackContext context)
    {
        for(int i = 0; i < toggleList.Count; i++){
            if(toggleList[i].isOn & i-1 >= 0){
                ExecuteEvents.Execute(toggleList[i-1].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                // toggleList[i-1].Select();
                break;
            }
        }
    }

    private void OnDisable()
    {
        forwardAction.performed -= OnForward;
        returnAction.performed -= OnReturn;

        forwardAction.Disable();
        returnAction.Disable();
    }
}
