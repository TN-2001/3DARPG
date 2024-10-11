using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(PlayerInput))]
public class PlayerMoveController : MonoBehaviour
{
    // コンポーネント
    // 物理
    private Rigidbody rb = null;
    // アニメーター
    private Animator anim = null;
    // 入力
    private PlayerInput input = null;

    // パラメータ
    [Header("物理パラメータ")]
    [SerializeField] // 走り速度（1秒で移動できる距離 m）
    private float runSpeed = 3f;
    [SerializeField] // ダッシュ速度
    private float dashSpeed = 5f;
    [SerializeField] // 振り向き速度
    private float rotateSpeed = 0.25f;
    [Header("アニメーションパラメータ")]
    [SerializeField] // アニメ遷移速度
    private float animChangeSpeed = 3f;

    [HideInInspector] // ダッシュできるか
    public bool isCanDash = true;
    [HideInInspector] // スタミナ回復
    public UnityEvent recoveryStamina, decreasedStamina;


    private void OnEnable()
    {
        if(input){
            if(input.actions["Move"].ReadValue<Vector2>().magnitude > 0){
                if(input.actions["Dash"].IsPressed() & isCanDash){
                    anim.SetFloat("speed", 1f);
                }
                else{
                    anim.SetFloat("speed", 0.5f);
                }
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        if(input.actions["Move"].ReadValue<Vector2>().magnitude > 0){
            // 向き
            Vector2 inpDir = input.actions["Move"].ReadValue<Vector2>();
            Quaternion camRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
            Vector3 dir = camRotation * new Vector3(inpDir.x, 0, inpDir.y).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), rotateSpeed);

            // ダッシュ移動
            if(input.actions["Dash"].IsPressed() & isCanDash){
                rb.velocity = new Vector3(transform.forward.x * dashSpeed, rb.velocity.y, transform.forward.z * dashSpeed);

                if(anim.GetFloat("speed") < 1f){
                    if(anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed < 1f)
                        anim.SetFloat("speed", anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed);
                    else{
                        anim.SetFloat("speed", 1f);
                    }
                }

                decreasedStamina?.Invoke();
            }
            // 走り移動
            else{
                rb.velocity = new Vector3(transform.forward.x * runSpeed, rb.velocity.y, transform.forward.z * runSpeed);

                if(anim.GetFloat("speed") > 0.5f){
                    if(anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed > 0.5f)
                        anim.SetFloat("speed", anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed);
                    else{
                        anim.SetFloat("speed", 0.5f);
                    }
                }
                else if(anim.GetFloat("speed") < 0.5f){
                    if(anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed < 0.5f)
                        anim.SetFloat("speed", anim.GetFloat("speed") + Time.fixedDeltaTime * animChangeSpeed);
                    else{
                        anim.SetFloat("speed", 0.5f);
                    }
                }

                if(!input.actions["Dash"].IsPressed()){
                    recoveryStamina?.Invoke();
                }
            }
        }
        else{
            if(anim.GetFloat("speed") > 0f){
                if(anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed > 0f)
                    anim.SetFloat("speed", anim.GetFloat("speed") - Time.fixedDeltaTime * animChangeSpeed);
                else{
                    anim.SetFloat("speed", 0f);
                }
            }

            recoveryStamina?.Invoke();
        }
    }

    private void OnDisable()
    {
        anim.SetFloat("speed", 0f);
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
}
