using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(Animator), typeof(PlayerInput))]
public class PlayerCharacterController : MonoBehaviour {
    // ステートマシン
    [SerializeField] private string stateName = "";
    private StateMachine<PlayerCharacterController> stateMachine = null;

    // コンポーネント
    private CharacterController con = null; // キャラクターコントローラー
    private Animator anim = null; // アニメーター
    private PlayerInput input = null; // 入力

    // パラメータ
    [SerializeField] private float gravity = 9.8f; // 重力
    [SerializeField] private float runSpeed = 3f; // 走り速度（1秒で移動できる距離 m）
    [SerializeField] private float dashSpeed = 5f; // ダッシュ速度
    [SerializeField, Range(0f, 1f)] private float rotationSpeed = 0.25f; // 振り向き速度
    [SerializeField] private float animChangeSpeed = 0.3f; // アニメ遷移速度

    // フラグ・イベント
    [HideInInspector] public bool isOn = true; // 起動しているか
    [HideInInspector] public bool isCanDash = true; // ダッシュできるか
    [HideInInspector] public UnityEvent recoveryStamina = null; // スタミナ回復イベント
    [HideInInspector] public UnityEvent decreasedStamina = null; // スタミナ減少イベント


    private void Start() {
        // コンポーネント
        con = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();

        // ステートマシン
        stateMachine = new StateMachine<PlayerCharacterController>(this);
        stateMachine.ChangeState(new Idle());
    }

    private void Update() {
        // ステートマシン
        stateMachine.OnUpdate();
    }

    private class Idle : StateBase<PlayerCharacterController> {
        public override void OnUpdate() {
            // ステート
            if (Owner.input.actions["Move"].ReadValue<Vector2>().magnitude > 0 && Owner.isOn) {
                Owner.stateMachine.ChangeState(new Run());
                return;
            }

            // 移動
            Vector3 dir = Vector3.zero;
            dir.y -= Owner.gravity * Time.deltaTime;
            Owner.con.Move(dir * Time.deltaTime);

            // アニメーション
            Owner.UpdateAnimationValue(0f);

            if (Owner.isOn) {
                // イベント
                Owner.recoveryStamina.Invoke();
            }
        }
    }

    private class Run : StateBase<PlayerCharacterController> {
        public override void OnUpdate() {
            // ステート
            if (Owner.input.actions["Move"].ReadValue<Vector2>().magnitude == 0 || !Owner.isOn) {
                Owner.stateMachine.ChangeState(new Idle());
                return;
            }

            // 向き
            Vector2 inpDir = Owner.input.actions["Move"].ReadValue<Vector2>();
            Quaternion camRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
            Vector3 dir = camRotation * new Vector3(inpDir.x, 0, inpDir.y).normalized;
            Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Owner.rotationSpeed);

            if (!Owner.input.actions["Dash"].IsPressed() || !Owner.isCanDash) {
                // 移動
                dir *= Owner.runSpeed;
                // アニメーション
                Owner.UpdateAnimationValue(0.5f);

                if (!Owner.input.actions["Dash"].IsPressed()) {
                    // イベント
                    Owner.recoveryStamina.Invoke();
                }
            } else {
                // 移動
                dir *= Owner.dashSpeed;
                // アニメーション
                Owner.UpdateAnimationValue(1f);
                // イベント
                Owner.decreasedStamina.Invoke();
            }
            // 移動
            dir.y -= Owner.gravity * Time.deltaTime;
            Owner.con.Move(dir * Time.deltaTime);
        }
    }


    // アニメーション遷移
    private void UpdateAnimationValue(float value) {
        if (anim.GetFloat("speed") > value) {
            if (anim.GetFloat("speed") - Time.deltaTime / animChangeSpeed > value)
                anim.SetFloat("speed", anim.GetFloat("speed") - Time.deltaTime / animChangeSpeed);
            else {
                anim.SetFloat("speed", value);
            }
        } else if (anim.GetFloat("speed") < value) {
            if (anim.GetFloat("speed") + Time.deltaTime / animChangeSpeed < value)
                anim.SetFloat("speed", anim.GetFloat("speed") + Time.deltaTime / animChangeSpeed);
            else {
                anim.SetFloat("speed", value);
            }
        }
    }
}
