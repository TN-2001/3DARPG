using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UIManager", menuName = "ScriptableObject/UIManager")]
public class UIManager : SingletonScriptableObject<UIManager> {
    private readonly List<GameObject> windowObjectList = new(); // ウィンドウオブジェクトを保存

    // 入力
    public EventSystem EventSystem { get; set; } // イベントシステム
    public PlayerInput PlayerInput { get; set; } // プレイヤーインプット
    public CinemachineInputAxisController CinemachineInput { get; set; } // シネマシーンへの入力


    // ウィンドウ
    public void InitWindow(GameObject windowObject) // 最初のウィンドウを保存
    {
        windowObjectList.Clear();
        windowObjectList.Add(windowObject);
    }
    public void ChangeWindow(GameObject windowObject) // ウィンドウを変更
    {
        if (windowObjectList.Count == 0) {
            Debug.Log("最初のウィンドウが登録されてないよ");
            return;
        }

        windowObjectList[^1].SetActive(false);
        windowObject.SetActive(true);
        windowObjectList.Add(windowObject);
    }
    public void ReturnWindow() // 前のウィンドウに戻る
    {
        if (windowObjectList.Count <= 1) {
            Debug.Log("これ以上戻れないよ");
            if (windowObjectList.Count == 0) {
                Debug.Log("最初のウィンドウが登録されてないよ");
            }
            return;
        }

        windowObjectList[^1].SetActive(false);
        windowObjectList.RemoveAt(windowObjectList.Count - 1);
        windowObjectList[^1].SetActive(true);
    }


    // カーソル
    public void EnableCursor() // カーソルの表示
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void DisableCursor() // カーソルの非表示
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    // 入力
    public void EnableInput() // 全ての入力をOn
    {
        EventSystem.enabled = true;
        EnablePlayerInput();
    }
    public void DisableInput() // 全ての入力をOff
    {
        EventSystem.enabled = false;
        DisablePlayerInput();
    }

    public void EnablePlayerInput() // プレイヤーの操作をOn
    {
        if (PlayerInput != null) {
            PlayerInput.actions.FindActionMap(PlayerInput.defaultActionMap).Enable();
        }
        if (CinemachineInput != null) {
            CinemachineInput.enabled = true;
        }
    }
    public void DisablePlayerInput() // プレイヤーの操作をOff
    {
        if (PlayerInput != null) {
            PlayerInput.actions.FindActionMap(PlayerInput.defaultActionMap).Disable();
        }
        if (CinemachineInput != null) {
            CinemachineInput.enabled = false;
        }
    }
}
