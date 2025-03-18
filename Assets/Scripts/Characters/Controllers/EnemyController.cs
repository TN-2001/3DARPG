using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AnimationDetector))]
[RequireComponent(typeof(MoveController), typeof(ChaseController))]
public class EnemyController : MonoBehaviour, IBattler
{
    // ステート
    [SerializeField] private string stateName = null;
    private StateMachine<EnemyController> stateMachine;

    // コンポーネント
    private Animator anim = null; // アニメーター
    private MoveController move = null; // 移動コントローラー
    private ChaseController chase = null; // 追跡コントローラー
    private AnimationDetector animDetector = null; // アニメーションディテクター
    private CharacterAudio caudio = null; // キャラクターオーディオ
    private CharacterEffect ceffect = null; // キャラクターエフェク

    // バトルウィンドウ
    private BattleWindow battleWindow = null;

    [Header("Componet")] // コンポーネントとデータ
    [SerializeField] private Enemy enemy = null; // キャラクター
    public Enemy Enemy => enemy;
    [SerializeField] private SearchDetector searchDetector = null; // エリア判定
    [SerializeField] private List<AttackController> attackControllers = null; // 攻撃
    [SerializeField] private List<AttackStatus> attackStatusList = new(); // 攻撃ステータスリスト

    [Header("Parameter")] // パラメータ
    [SerializeField] private float rotationSpeed = 90f; // 回転速度（1秒間の回転角度）
    [SerializeField] private float coolTime = 3f; // クールタイム

    // ターゲット
    private GameObject target = null; // ターゲット
    private float targetDistance = 0f; // ターゲットとの距離
    private Quaternion targetRotation = new(); // ターゲットの方向
    private float targetAngle = 0f; // ターゲットの方向との角度差

    // フラグ
    private float countCoolTime = 0f; // 時間カウント
    private bool isCanRotation = false;
    private int attackNumber = -1; // 攻撃番号

    // イベント
    [NonSerialized] public UnityEvent onDie = new(); // 死亡時


    private void Start()
    {
        // コンポーネント入手
        anim = GetComponent<Animator>();
        move = GetComponent<MoveController>();
        chase = GetComponent<ChaseController>();
        animDetector = GetComponent<AnimationDetector>();
        caudio = GetComponent<CharacterAudio>();
        ceffect = GetComponent<CharacterEffect>();

        battleWindow = GetComponentInParent<EnemyContent>().BattleWindow;

        searchDetector.onEnter.AddListener(delegate(Collider other){
            target = other.gameObject;
        });

        // キャラクター
        enemy = new(enemy.Data){
            Transform = transform
        };
        DataManager.Instance.EnemyList.Add(enemy);

        // 攻撃
        for (int i = 0; i < attackControllers.Count; i++){
            attackStatusList[i].Init(enemy);
            attackControllers[i].Initialize(attackStatusList[i]);
        }

        // UI
        battleWindow.AddMapEnemy(transform);

        // ステートマシン
        stateMachine = new StateMachine<EnemyController>(this);
        stateMachine.ChangeState(new Move());
    }

    private void FixedUpdate()
    {
        if(target != null){
            // ターゲットのパラメータ
            targetDistance = Vector3.Distance(transform.position, target.transform.position);
            Vector3 direction = (target.transform.position - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            targetAngle = Quaternion.Angle(transform.rotation, targetRotation);
        }

        // ステートマシン
        stateMachine.OnUpdate();
        stateName = stateMachine.CurrentStateType.ToString();

        if(stateMachine.CurrentStateType != typeof(Attack)){
            // 時間カウント
            countCoolTime += Time.fixedDeltaTime;

            if(countCoolTime >= coolTime && target != null && attackNumber < 0){
                attackNumber = UnityEngine.Random.Range(0, attackControllers.Count);
            }
        }
    }

    private class Move : StateBase<EnemyController> // 通常状態（立ち、歩き移動）
    {
        public override void OnStart()
        {
            Owner.move.enabled = true;
        }

        public override void OnUpdate()
        {
            // ステート変更
            if(Owner.target){
                Owner.stateMachine.ChangeState(new Chase());
            }
        }

        public override void OnEnd()
        {
            Owner.move.enabled = false;
        }
    }

    private class Chase : StateBase<EnemyController> // 追いかけ状態（立ち、追いかけ）
    {
        public override void OnStart()
        {
            if(Owner.target){
                Owner.chase.target = Owner.target.transform;
                Owner.chase.enabled = true;
            }
        }

        public override void OnUpdate()
        {
            if(DataManager.Instance.Player.CurrentHp == 0){
                Owner.target = null;
            }

            // ステート変更
            if(!Owner.target){
                Owner.stateMachine.ChangeState(new Move());
            }

            if(Owner.targetAngle > 30){
                Owner.stateMachine.ChangeState(new Rotation());
            }else if(Owner.attackNumber >= 0){
                if(Owner.attackControllers[Owner.attackNumber].MinRange <= Owner.targetDistance 
                    && Owner.targetDistance <= Owner.attackControllers[Owner.attackNumber].MaxRange){
                    Owner.stateMachine.ChangeState(new Attack());
                }else if(Owner.attackControllers[Owner.attackNumber].MinRange > Owner.targetDistance){
                    Owner.stateMachine.ChangeState(new Back());
                }
            }
        }

        public override void OnEnd()
        {
            Owner.chase.enabled = false;
        }
    }

    private class Rotation : StateBase<EnemyController> // 回転
    {
        public override void OnStart()
        {
            Owner.anim.SetTrigger("rotationTrigger");
        }

        public override void OnUpdate()
        {
            if(Owner.isCanRotation && Owner.target != null){
                // 滑らかに向きを更新
                if(Owner.targetAngle > 1f){
                    Owner.transform.rotation = Quaternion.Slerp(
                        Owner.transform.rotation, Owner.targetRotation, Time.deltaTime * (Owner.rotationSpeed/Owner.targetAngle));
                }
            }
        }
    }

    private class Back : StateBase<EnemyController> // バックステップ
    {
        public override void OnStart()
        {
            Owner.OnAnimationStart();
            Owner.anim.SetTrigger("backTrigger");
        }
    }

    private class Attack : StateBase<EnemyController> // 攻撃
    {
        public override void OnStart()
        {
            Owner.OnAnimationStart();

            // アニメーション
            Owner.anim.SetInteger("attackNumber", Owner.attackNumber);
            Owner.anim.SetTrigger("attackTrigger");
        }

        public override void OnEnd()
        {
            Owner.OnAttackCollisionDisable();

            Owner.countCoolTime = 0f;
            Owner.attackNumber = -1;
        }
    }

    private class Hit : StateBase<EnemyController> // ヒット
    {
        public override void OnStart()
        {
            if(Owner.enemy.CurrentHp <= 0f){
                Owner.anim.SetInteger("hitNumber",1);
            }
            else{
                Owner.anim.SetInteger("hitNumber",0);
            }

            Owner.anim.SetTrigger("hitTrigger");
        }
    }


    // 死亡
    private IEnumerator EDie()
    {
        // 見つけた敵に追加
        DataManager.Instance.Data.UpdateFindEnemy(enemy.Data.Number - 1);

        foreach(ItemData data in Enemy.DropItemList){
            battleWindow.GetItem(data);
        }
        foreach(ArmorData data in Enemy.DropArmorList){
            battleWindow.GetArmor(data);
        }
        foreach(WeaponData data in Enemy.DropWeaponList){
            battleWindow.GetWeapon(data);
        }
        DataManager.Instance.EnemyList.Remove(enemy);
        // UI
        battleWindow.RemoveMapEnemy(transform);

        onDie.Invoke();

        Destroy(gameObject);

        yield return null;
    }


    // IBattlerのダメージ関数
    public void OnDamage(AttackStatus attack, Vector3 position)
    {
        if(enemy.CurrentHp > 0){
            int dam = enemy.UpdateHp(-attack.Atk);
            battleWindow.InitDamageText(-dam, position);
            if(enemy.CurrentHp == 0){
                stateMachine.ChangeState(new Hit());
            }else if(target == null && searchDetector.AllColliderList.Count > 0){
                target = searchDetector.AllColliderList[0].gameObject;
            }
            caudio.PlayOneShot_Hit();
            ceffect.PlayInstantiateParticle_Hit(position);
        }
    }


    // アニメーションイベント
    public void OnAnimationStart() // アニメーション開始
    {
        isCanRotation = false;
    }

    public void OnAttackAnimationStart() // 攻撃アニメーション開始
    {
        OnAnimationStart();
    }

    public void OnAnimationEnd() // アニメーション終了
    {
        if((stateMachine.CurrentStateType == typeof(Hit) && enemy.CurrentHp > 0)
            || stateMachine.CurrentStateType == typeof(Back)
            || stateMachine.CurrentStateType == typeof(Attack)
            || stateMachine.CurrentStateType == typeof(Rotation)){
            stateMachine.ChangeState(new Chase());
        }else if(stateMachine.CurrentStateType == typeof(Hit) && enemy.CurrentHp == 0){
            StartCoroutine(EDie());
        }
    }

    public void OnAttackCollisionEnable(int number)
    {
        attackControllers[attackNumber].Initialize(attackStatusList[attackNumber]);
        attackControllers[attackNumber].OnCollisionEnable();
    }

    public void OnAttackCollisionDisable()
    {
        attackControllers[attackNumber].OnCollisionDisable();
    }

    public void OnRotationStart() // 回転開始
    {
        isCanRotation = true;
    }

    public void OnRotationEnd() // 回転終了
    {
        isCanRotation = false;
    }
}