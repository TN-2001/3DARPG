using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationDetector : MonoBehaviour {
    // フラグ
    public bool isAnimStart { get; private set; }
    public bool isAttackCollisionStart { get; private set; }
    public bool isAttackCollisionEnd { get; private set; }
    public bool isAttackCombo { get; private set; }
    public bool isAnimEnd { get; private set; }
    [HideInInspector] // イベントリスナー
    public UnityEvent onAnimStart, onAttackCollisionStart, onAttackCollisionEnd, onAttackCombo, onAnimEnd;

    public void Init() {
        isAnimStart = false;
        isAttackCollisionStart = false;
        isAttackCollisionEnd = false;
        isAttackCombo = false;
        isAnimEnd = false;

        onAnimStart.RemoveAllListeners();
        onAttackCollisionStart.RemoveAllListeners();
        onAttackCollisionEnd.RemoveAllListeners();
        onAttackCombo.RemoveAllListeners();
        onAnimEnd.RemoveAllListeners();
    }

    public void OnAnimStart() {
        isAnimStart = true;
        onAnimStart?.Invoke();
    }

    public void OnAttackCollisionStart() {
        if (isAnimStart) {
            isAttackCollisionStart = true;
            onAttackCollisionStart?.Invoke();
        }
    }

    public void OnAttackCollisionEnd() {
        if (isAnimStart) {
            isAttackCollisionEnd = true;
            onAttackCollisionEnd?.Invoke();
        }
    }

    public void OnAttackCombo() {
        if (isAnimStart) {
            isAttackCombo = true;
            onAttackCombo?.Invoke();
        }
    }

    public void OnAnimEnd() {
        if (isAnimStart) {
            isAnimEnd = true;
            onAnimEnd?.Invoke();
        }
    }
}
