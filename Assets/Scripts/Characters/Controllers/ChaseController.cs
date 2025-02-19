using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class ChaseController : MonoBehaviour
{
    // ステートマシン
    [SerializeField] private string stateName = "";
    private StateMachine<ChaseController> stateMachine = null;

    // コンポーネント
    private NavMeshAgent agent = null; // ナビメッシュエージェント
    private Animator anim = null; // アニメーター

    // パラメータ
    [Header("コンポーネント")]
    public Transform target = null; // ターゲット
    [Header("物理パラメータ")] 
    [SerializeField] private float chaseSpeed = 2f; // 追いかけ速度(1秒で移動できる距離 m)
    public float chaseDistance = 2f, stopDistance = 1f; // 追いかける距離
    [SerializeField] private bool isRotate = false; // 回転するか
    [SerializeField] private float angleThreshold = 10.0f; // 許容する角度の範囲
    [Header("アニメーションパラメータ")]
    [SerializeField] private float animChangeSpeed = 0.3f; // アニメ遷移速度

    // 変数
    private Quaternion lookRotation = new(); // ターゲットの方向

    public bool IsLookTarget // ターゲットの方向を向いているか
    {
        get{
            if(target != null){
                return Quaternion.Angle(transform.rotation, lookRotation) < angleThreshold;
            }else{
                return false;
            }
        }
    }


    private void OnEnable()
    {
        stateMachine?.CurrentState?.OnStart();
    }

    private void Start()
    {
        // コンポーネント
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // ステート
        stateMachine = new StateMachine<ChaseController>(this);
        stateMachine.ChangeState(new Idle());
    }

    private void FixedUpdate()
    {
        if(target != null){
            // ターゲットの方向を取得
            Vector3 direction = (target.position - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            
        }else{
            // ステート
            if(stateMachine.CurrentState.GetType() != typeof(Idle)){
                stateMachine.ChangeState(new Idle());
            }
        }

        stateMachine.OnUpdate();
        stateName = stateMachine.CurrentState.ToString();
    }

    private class Idle : StateBase<ChaseController>
    {
        public override void OnUpdate()
        {
            // アニメーション
            Owner.UpdateAnimation(0f);

            if(Owner.target == null){
                return;
            }

            // ステート
            if(Owner.target){
                if(Vector3.Distance(Owner.transform.position, Owner.target.position) >= Owner.chaseDistance){
                    Owner.stateMachine.ChangeState(new Chase());
                }else if(!Owner.IsLookTarget && Owner.isRotate){
                    Owner.stateMachine.ChangeState(new Rotation());
                }
            }
        }
    }

    private class Chase : StateBase<ChaseController>
    {
        public override void OnStart()
        {
            // 移動
            Owner.agent.SetDestination(Owner.target.position);
            Owner.agent.speed = Owner.chaseSpeed;
        }

        public override void OnUpdate()
        {
            // 移動
            Owner.agent.SetDestination(Owner.target.position);

            // アニメーション
            Owner.UpdateAnimation(1f);

            // ステート
            if(Vector3.Distance(Owner.transform.position, Owner.target.position) < Owner.stopDistance){
                Owner.stateMachine.ChangeState(new Idle());
            }
        }

        public override void OnEnd()
        {
            // 移動
            Owner.agent.speed = 0;
        }
    }

    private class Rotation : StateBase<ChaseController>
    {
        private float countTime = 0f;

        public override void OnUpdate()
        {
            // 向き
            Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, Owner.lookRotation, Time.deltaTime * 5.0f);

            // アニメーション
            Owner.UpdateAnimation(0.5f);

            // ステート
            if(Vector3.Distance(Owner.transform.position, Owner.target.position) >= Owner.chaseDistance){
                Owner.stateMachine.ChangeState(new Chase());
            }else if(Owner.IsLookTarget && countTime > 1f){
                Owner.stateMachine.ChangeState(new Idle());
            }

            countTime += Time.fixedDeltaTime;
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
        stateMachine.CurrentState.OnEnd();
    }
}
