using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class MoveController : MonoBehaviour
{
    // ステートマシン
    private StateMachine<MoveController> stateMachine = null;

    // コンポーネント
    // ナビメッシュエージェント
    private NavMeshAgent agent = null;
    // アニメーター
    private Animator anim = null;

    // パラメータ
    [Header("コンポーネント")]
    [SerializeField] // ターゲット
    public List<Transform> targetList = new List<Transform>();
    [Header("物理パラメータ")] 
    [SerializeField] // 歩き速度(1秒で移動できる距離 m)
    private float walkSpeed = 2f;
    [SerializeField] // 移動のクールタイム
    private float maxCoolTime = 10f, minCoolTime = 3f;
    [Header("アニメーションパラメータ")]
    [SerializeField] // アニメ遷移速度
    private float animChangeSpeed = 3f;
    [SerializeField] // アニメーション再生速度
    private float animIdleSpeed = 1, animWalkSpeed = 1;

    // 変数
    // 起動時か
    private bool isOnEnable = false;
    // ターゲット
    private Transform target = null;


    private void OnEnable()
    {
        isOnEnable = true;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        stateMachine = new StateMachine<MoveController>(this);
    }

    private void FixedUpdate()
    {
        if(isOnEnable){
            stateMachine.ChangeState(new Idle());
            isOnEnable = false;
        }

        stateMachine.OnUpdate();
    }

    private class Idle : StateBase<MoveController>
    {
        // クールタイム
        private float coolTime = 0f;
        // 時間カウント
        private float countTime = 0f;

        public override void OnStart()
        {
            coolTime = Random.Range(Owner.minCoolTime, Owner.maxCoolTime);
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

            if(countTime >= coolTime & Owner.targetList.Count >= 2){
                Owner.stateMachine.ChangeState(new Move());
            }

            countTime += Time.fixedDeltaTime;
        }
    }

    private class Move : StateBase<MoveController>
    {
        public override void OnStart()
        {
            Owner.anim.SetFloat("playSpeed", Owner.animWalkSpeed);
            List<Transform> targetList = new List<Transform>();
            for(int i = 0; i < Owner.targetList.Count; i++){
                if(Owner.targetList[i] != Owner.target){
                    targetList.Add(Owner.targetList[i]);
                }
            }
            Owner.target = targetList[Random.Range(0, targetList.Count)];
            Owner.agent.destination = Owner.target.position;
            Owner.agent.speed = Owner.walkSpeed;
        }

        public override void OnUpdate()
        {
            if(Owner.anim.GetFloat("speed") < 0.5f){
                if(Owner.anim.GetFloat("speed") + Time.fixedDeltaTime*Owner.animChangeSpeed < 0.5f)
                    Owner.anim.SetFloat("speed", Owner.anim.GetFloat("speed") + Time.fixedDeltaTime*Owner.animChangeSpeed);
                else{
                    Owner.anim.SetFloat("speed", 0.5f);
                }
            }

            if(Owner.agent.remainingDistance < 0.1f){
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
