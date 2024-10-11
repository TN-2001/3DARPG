using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
[RequireComponent(typeof(AnimationDetector), typeof(DamageDetector))]
[RequireComponent(typeof(MoveController), typeof(ChaseController))]
public class EnemyController : MonoBehaviour
{
    // ステート
    [SerializeField]
    private string stateName = null;
    private StateMachine<EnemyController> stateMachine;

    // コンポーネント
    // ナビメッシュエージェント
    private NavMeshAgent agent = null;
    // アニメーター
    private Animator anim = null;
    // 移動コントローラー
    private MoveController move = null;
    // 追跡コントローラー
    private ChaseController chase = null;
    // アニメーションディテクター
    private AnimationDetector animDetector = null;
    // ダメージディテクター
    private DamageDetector damageDetector = null;

    // バトルウィンドウ
    private BattleWindow battleWindow = null;

    [Header("コンポーネント")]
    [SerializeField] // キャラクター
    private Enemy enemy = null;
    public Enemy Enemy => enemy;
    [SerializeField] // エリア判定
    private SearchDetector searchDetector = null;
    [SerializeField] // 攻撃
    private List<AttackController> attackControllers = null;
    [SerializeField] // パーティクル
    private ParticleSystem particle = null;

    // パラメータ
    [Header("パラメータ")]
    [SerializeField] // クールタイム
    private float coolTime = 3f;
    // ダメージ
    private bool isDamage = false;
    // 攻撃
    private AttackController attackController = null;
    // 攻撃番号
    private int atkNum = 0;
    // 時間カウント
    private float countCoolTime = 0f;
    // ターゲット
    private GameObject target = null;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        move = GetComponent<MoveController>();
        chase = GetComponent<ChaseController>();
        animDetector = GetComponent<AnimationDetector>();
        damageDetector = GetComponent<DamageDetector>();

        battleWindow = GetComponentInParent<EnemyContent>().BattleWindow;

        searchDetector.onEnter.AddListener(delegate(Collider other){
            target = other.gameObject;
        });
        searchDetector.onExit.AddListener(delegate(Collider other){
            target = null;
        });

        enemy = new Enemy(enemy.Data);
        for(int i = 0; i < attackControllers.Count; i++){
            attackControllers[i].Initialize(enemy.Atk);
        }

        damageDetector.onDamage.AddListener(delegate(int damage, Vector3 pos){
            if(enemy.CurrentHp >= 0f){
                int dam = enemy.UpdateHp(-damage);
                isDamage = true;
                battleWindow.InitDamageText(-dam, pos);
            }
            GetComponent<CharacterAudio>().PlayOneShot_Hit();
        });

        agent.autoBraking = false;

        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(new Move());
    }

    private void FixedUpdate()
    {
        stateMachine.OnUpdate();
        stateName = stateMachine.currentState.ToString();

        // 時間カウント
        countCoolTime += Time.fixedDeltaTime;
        if(countCoolTime >= coolTime & !attackController){
            atkNum = Random.Range(0, attackControllers.Count);
            attackController = attackControllers[atkNum];
        }
    }

    private class Move : StateBase<EnemyController>
    {
        public override void OnStart()
        {
            Owner.move.enabled = true;
        }

        public override void OnUpdate()
        {
            // ステート変更
            if(Owner.isDamage){
                Owner.stateMachine.ChangeState(new Hit());
            }
            else if(Owner.target){
                Owner.stateMachine.ChangeState(new Chase());
            }
        }

        public override void OnEnd()
        {
            Owner.move.enabled = false;
        }
    }

    private class Chase : StateBase<EnemyController>
    {
        public override void OnStart()
        {
            Owner.chase.target = Owner.target.transform;
            Owner.chase.enabled = true;
        }

        public override void OnUpdate()
        {
            // ステート変更
            if(Owner.isDamage){
                Owner.stateMachine.ChangeState(new Hit());
            }
            else if(!Owner.target){
                Owner.stateMachine.ChangeState(new Move());
            }
            else if(Owner.attackController){
                if(Owner.attackController.MinRange > Vector3.Distance(Owner.transform.position, Owner.target.transform.position)){
                    Owner.stateMachine.ChangeState(new Back());
                }
                else if(Owner.attackController.MinRange <= Vector3.Distance(Owner.transform.position, Owner.target.transform.position) &
                    Vector3.Distance(Owner.transform.position, Owner.target.transform.position) <= Owner.attackController.MaxRange){
                    Owner.stateMachine.ChangeState(new Attack());
                }
            }
        }

        public override void OnEnd()
        {
            Owner.chase.enabled = false;
        }
    }

    private class Back : StateBase<EnemyController>
    {
        public override void OnStart()
        {
            Owner.animDetector.Init();
            Owner.anim.SetTrigger("isBack");
        }

        public override void OnUpdate()
        {
            // ステート変更
            if(Owner.animDetector.isAnimEnd){
                Owner.stateMachine.ChangeState(new Chase());
            }
        }
    }

    private class Attack : StateBase<EnemyController>
    {
        public override void OnStart()
        {
            // フラグ初期化
            Owner.animDetector.Init();

            // イベントリスナー
            Owner.animDetector.onAttackCollisionStart.AddListener(delegate{
                if(!Owner.attackController.IsThrow){
                    Owner.attackController.GetComponent<Collider>().enabled = true;
                }
            });
            Owner.animDetector.onAttackCollisionEnd.AddListener(delegate{
                if(!Owner.attackController.IsThrow){
                    Owner.attackController.GetComponent<Collider>().enabled = false;
                }
            });

            // 敵の方を向く
            Owner.transform.rotation = Quaternion.FromToRotation(Vector3.forward, (Owner.target.transform.position - Owner.transform.position).normalized);

            // 攻撃オブジェクトを初期化
            if(Owner.attackController.IsThrow){
                GameObject obj = Instantiate(Owner.attackController.gameObject, 
                    Owner.attackController.transform.position,
                    Owner.attackController.transform.rotation);
                obj.transform.SetParent(Owner.transform.parent);
                obj.SetActive(true);
            }
            else{
                Owner.attackController.GetComponent<Collider>().enabled = false;
                Owner.attackController.gameObject.SetActive(true);
            }
            Owner.attackController.Initialize(Owner.enemy.Atk);

            // アニメーション
            Owner.anim.SetInteger("atkNum", Owner.atkNum);
            Owner.anim.SetTrigger("isAtk");
        }

        public override void OnUpdate()
        {
            // ステート変更
            if(Owner.isDamage){
                Owner.stateMachine.ChangeState(new Hit());
            }
            if(Owner.animDetector.isAnimEnd){
                if(Owner.target){
                    Owner.stateMachine.ChangeState(new Chase());
                }
                else{
                    Owner.stateMachine.ChangeState(new Move());
                }
            }
        }

        public override void OnEnd()
        {
            if(!Owner.attackController.IsThrow){
                Owner.attackController.gameObject.SetActive(false);
            }
            Owner.countCoolTime = 0f;
            Owner.attackController = null;
        }
    }

    private class Hit : StateBase<EnemyController>
    {
        public override void OnStart()
        {
            Owner.animDetector.Init();
            if(Owner.enemy.CurrentHp <= 0f){
                Owner.anim.SetInteger("hitNum",1);
                Owner.animDetector.onAnimEnd.AddListener(delegate{
                    Owner.StartCoroutine(IDie());
                });
            }
            else{
                Owner.anim.SetInteger("hitNum",0);
            }

            Owner.anim.SetTrigger("isHit");
            Owner.isDamage = false;
        }

        public override void OnUpdate()
        {
            // ステート変更
            if(Owner.animDetector.isAnimEnd){
                Owner.stateMachine.ChangeState(new Move());
            }
        }

        private IEnumerator IDie()
        {
            // 見つけた敵に追加
            GameManager.I.Data.UpdateFindEnemy(Owner.enemy.Data.Number - 1);

            GameObject obj = Instantiate(Owner.particle.gameObject, Owner.transform.position, Owner.particle.transform.rotation);
            obj.SetActive(true);
            Destroy(obj, 2f);

            yield return new WaitForSeconds(1f);
            foreach(ItemData data in Owner.Enemy.DropItemList){
                Owner.battleWindow.GetItem(data);
            }
            foreach(ArmorData data in Owner.Enemy.DropArmorList){
                Owner.battleWindow.GetArmor(data);
            }
            foreach(WeaponData data in Owner.Enemy.DropWeaponList){
                Owner.battleWindow.GetWeapon(data);
            }
            Destroy(Owner.gameObject);
        }
    }
}