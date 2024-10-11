using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class ChaseController : MonoBehaviour
{
    // ステートマシン
    private StateMachine<ChaseController> stateMachine = null;

    // コンポーネント
    // ナビメッシュエージェント
    private NavMeshAgent agent = null;
    // アニメーター
    private Animator anim = null;

    // パラメータ
    [Header("コンポーネント")]
    [SerializeField] // ターゲット
    public Transform target = null;
    [Header("物理パラメータ")] 
    [SerializeField] // 追いかけ速度(1秒で移動できる距離 m)
    private float chaseSpeed = 2f;
    [SerializeField] // 追いかける距離
    public float chaseDistance = 2f, stopDistance = 1f;
    [Header("アニメーションパラメータ")]
    [SerializeField] // アニメ遷移速度
    private float animChangeSpeed = 3f;
    [SerializeField] // アニメーション再生速度
    private float animIdleSpeed = 1, animChaseSpeed = 1;

    // 変数
    // 起動時か
    private bool isOnEnable = false;


    private void OnEnable()
    {
        isOnEnable = true;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        stateMachine = new StateMachine<ChaseController>(this);
    }

    private void FixedUpdate()
    {
        if(isOnEnable){
            stateMachine.ChangeState(new Idle());
            isOnEnable = false;
        }

        stateMachine.OnUpdate();
    }

    private class Idle : StateBase<ChaseController>
    {
        public override void OnStart()
        {
            Owner.anim.SetFloat("playSpeed", Owner.animIdleSpeed);
        }

        public override void OnUpdate()
        {
            if(Owner.anim.GetFloat("speed") > 0f){
                if(Owner.anim.GetFloat("speed") - Time.fixedDeltaTime*Owner.animChangeSpeed > 0f)
                    Owner.anim.SetFloat("speed", Owner.anim.GetFloat("speed") - Time.fixedDeltaTime*Owner.animChangeSpeed);
                else{
                    Owner.anim.SetFloat("speed", 0f);
                }
            }

            if(Owner.target){
                if(Vector3.Distance(Owner.transform.position, Owner.target.position) >= Owner.chaseDistance){
                    Owner.stateMachine.ChangeState(new Chase());
                }
            }
        }
    }

    private class Chase : StateBase<ChaseController>
    {
        public override void OnStart()
        {
            Owner.anim.SetFloat("playSpeed", Owner.animChaseSpeed);
            Owner.agent.destination = Owner.target.position;
            Owner.agent.speed = Owner.chaseSpeed;
        }

        public override void OnUpdate()
        {
            if(!Owner.target){
                Owner.stateMachine.ChangeState(new Idle());
                return;
            }

            Owner.agent.destination = Owner.target.position;

            if(Owner.anim.GetFloat("speed") < 1f){
                if(Owner.anim.GetFloat("speed") + Time.fixedDeltaTime*Owner.animChangeSpeed < 1f)
                    Owner.anim.SetFloat("speed", Owner.anim.GetFloat("speed") + Time.fixedDeltaTime*Owner.animChangeSpeed);
                else{
                    Owner.anim.SetFloat("speed", 1f);
                }
            }

            if(Vector3.Distance(Owner.transform.position, Owner.target.position) < Owner.stopDistance){
                Owner.stateMachine.ChangeState(new Idle());
            }
        }

        public override void OnEnd()
        {
            Owner.agent.speed = 0;
        }
    }

    private void OnDisable()
    {
        anim.SetFloat("speed", 0f);
        anim.SetFloat("playSpeed",1);
        stateMachine.currentState.OnEnd();
    }
}
