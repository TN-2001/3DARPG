// PlayerCharacterController.csに移動
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.InputSystem;

// [RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(PlayerInput))]
// public class PlayerMoveController : MonoBehaviour {
//     // コンポーネント
//     private Rigidbody rb = null; // 物理
//     private Animator anim = null; // アニメーター
//     private PlayerInput input = null; // 入力

//     // パラメータ
//     [Header("物理パラメータ")]
//     [SerializeField] private float runSpeed = 3f; // 走り速度（1秒で移動できる距離 m）
//     [SerializeField] private float dashSpeed = 5f; // ダッシュ速度
//     [SerializeField] private float rotateSpeed = 0.25f; // 振り向き速度
//     [Header("アニメーションパラメータ")]
//     [SerializeField] private float animChangeSpeed = 3f; // アニメ遷移速度

//     // フラグ・イベント
//     [HideInInspector] public bool isCanDash = true; // ダッシュできるか
//     [HideInInspector] public UnityEvent recoveryStamina = null; // スタミナ回復イベント
//     [HideInInspector] public UnityEvent decreasedStamina = null; // スタミナ減少イベント


//     private void Start() {
//         // コンポーネント取得
//         rb = GetComponent<Rigidbody>();
//         anim = GetComponent<Animator>();
//         input = GetComponent<PlayerInput>();
//     }

//     private void FixedUpdate() {
//         if (input.actions["Move"].ReadValue<Vector2>().magnitude > 0) {
//             // 向き
//             Vector2 inpDir = input.actions["Move"].ReadValue<Vector2>();
//             Quaternion camRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
//             Vector3 dir = camRotation * new Vector3(inpDir.x, 0, inpDir.y).normalized;
//             transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), rotateSpeed);

//             // ダッシュ移動
//             if (input.actions["Dash"].IsPressed() & isCanDash) {
//                 rb.linearVelocity = new Vector3(transform.forward.x * dashSpeed, rb.linearVelocity.y, transform.forward.z * dashSpeed);

//                 if (anim.GetFloat("speed") < 1f) {
//                     if (anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed < 1f)
//                         anim.SetFloat("speed", anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed);
//                     else {
//                         anim.SetFloat("speed", 1f);
//                     }
//                 }

//                 decreasedStamina?.Invoke();
//             }
//             // 走り移動
//             else {
//                 rb.linearVelocity = new Vector3(transform.forward.x * runSpeed, rb.linearVelocity.y, transform.forward.z * runSpeed);

//                 if (anim.GetFloat("speed") > 0.5f) {
//                     if (anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed > 0.5f)
//                         anim.SetFloat("speed", anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed);
//                     else {
//                         anim.SetFloat("speed", 0.5f);
//                     }
//                 } else if (anim.GetFloat("speed") < 0.5f) {
//                     if (anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed < 0.5f)
//                         anim.SetFloat("speed", anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed);
//                     else {
//                         anim.SetFloat("speed", 0.5f);
//                     }
//                 }

//                 if (!input.actions["Dash"].IsPressed()) {
//                     recoveryStamina?.Invoke();
//                 }
//             }
//         }
//         // 立ち
//         else {
//             if (anim.GetFloat("speed") > 0f) {
//                 if (anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed > 0f)
//                     anim.SetFloat("speed", anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed);
//                 else {
//                     anim.SetFloat("speed", 0f);
//                 }
//             }

//             recoveryStamina?.Invoke();
//         }
//     }

//     private void OnDisable() {
//         // 初期化
//         anim.SetFloat("speed", 0f);
//         rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
//     }
// }
