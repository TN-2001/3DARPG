using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class MoveController : MonoBehaviour
{
    // ステートマシン
    private StateMachine<MoveController> stateMachine = null;

    // コンポーネント
    private NavMeshAgent agent = null; // ナビメッシュエージェント
    private Animator anim = null; // アニメーター

    // パラメータ
    [Header("コンポーネント")]
    public List<Transform> targetList = new(); // ターゲット
    [Header("物理パラメータ")] 
    [SerializeField] private float walkSpeed = 2f; // 歩き速度(1秒で移動できる距離 m)
    [SerializeField] private float maxCoolTime = 10f; // 移動のクールタイム
    [SerializeField] private float minCoolTime = 3f;
    [Header("アニメーションパラメータ")]
    [SerializeField] private float animChangeSpeed = 0.3f; // アニメ遷移速度

    // 変数
    private Transform target = null; // ターゲット


    private void OnEnable()
    {
        stateMachine?.ChangeState(new Idle());
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        stateMachine = new StateMachine<MoveController>(this);
        stateMachine.ChangeState(new Idle());
    }

    private void FixedUpdate()
    {
        stateMachine.OnUpdate();
    }

    private class Idle : StateBase<MoveController>
    {
        private float coolTime = 0f; // クールタイム
        private float countTime = 0f; // 時間カウント

        public override void OnStart()
        {
            coolTime = Random.Range(Owner.minCoolTime, Owner.maxCoolTime);
        }

        public override void OnUpdate()
        {
            Owner.UpdateAnimation(0f);

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
            List<Transform> targetList = new();
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
            Owner.UpdateAnimation(0.5f);

            if(Owner.agent.remainingDistance < 0.1f){
                Owner.stateMachine.ChangeState(new Idle());
            }
        }

        public override void OnEnd()
        {
            Owner.agent.speed = 0;
        }
    }

    private void UpdateAnimation(float speed) // アニメーション更新
    {
        if(anim.GetFloat("speed") < speed){
            if(anim.GetFloat("speed") + Time.fixedDeltaTime/animChangeSpeed < speed)
                anim.SetFloat("speed", anim.GetFloat("speed") + Time.fixedDeltaTime/animChangeSpeed);
            else{
                anim.SetFloat("speed", speed);
            }
        }else if(anim.GetFloat("speed") > speed){
            if(anim.GetFloat("speed") - Time.fixedDeltaTime/animChangeSpeed > speed)
                anim.SetFloat("speed", anim.GetFloat("speed") - Time.fixedDeltaTime/animChangeSpeed);
            else{
                anim.SetFloat("speed", speed);
            }
        }
    }

    private void OnDisable()
    {
        anim.SetFloat("speed", 0f);
        stateMachine.OnEnd();
    }
}
