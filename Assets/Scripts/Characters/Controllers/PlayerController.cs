using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AnimationDetector), typeof(PlayerMoveController))]
public class PlayerController : MonoBehaviour, IBattler
{
    // ステート
    [SerializeField] private string stateName = null;
    private StateMachine<PlayerController> stateMachine = null;

    //コンポーネント
    private Animator anim = null; // アニメーションコンポーネント
    private PlayerMoveController move = null; // 移動
    private AnimationDetector animDetector = null; // アニメーションディテクター
    private CharacterEffect effect = null; // エフェク

    private Player player = null; // プレイヤー
    private readonly List<NpcController> npcControllerList = new(); // NPC
    private NpcData NpcData
    {
        get{
            if(npcControllerList.Count > 0){
                return npcControllerList[0].NpcData;
            }
            else{
                return null;
            }
        }
    }
    private readonly List<Transform> targetTraList = new(); // 敵
    public Transform TargetTra
    {
        get{
            for(int i = targetTraList.Count-1; i >= 0; i--){
                if(targetTraList[i] == null){
                    targetTraList.RemoveAt(i);
                }
            }
            Transform tra = null;
            for(int i = 0; i < targetTraList.Count; i++){
                if(i == 0){
                    tra = targetTraList[0];
                }
                else{
                    Vector3 dis1 = transform.position - tra.position;
                    Vector3 dis2 = transform.position - targetTraList[i].position;
                    if(dis1.magnitude > dis2.magnitude){
                        tra = targetTraList[i];
                    }
                }
            }
            return tra;
        }
    }

    [Header("BattleParameter")]
    [SerializeField] private CollisionDetector collisionDetector = null; // 当たり判定
    [SerializeField] private CollisionDetector serchDetector = null; // 索敵判定
    [SerializeField] private List<AttackController> attackControllerList = new(); // 攻撃リスト
    [SerializeField] private ParticleSystem barrierParticle = null; // バリアパーティクル

    [Header("UI")]
    [SerializeField] private BattleWindow battleWindow = null; // バトルウィンドウ
    [SerializeField] private ChatWindow chatWindow = null; // チャットウィンドウ

    private int nextAttackNumber = -1;
    private bool isGurd = false;


    private void Start()
    {
        // コンポーネント入手
        anim = GetComponent<Animator>();
        move = GetComponent<PlayerMoveController>();
        animDetector = GetComponent<AnimationDetector>();
        effect = GetComponent<CharacterEffect>();

        // キャラクター
        player = DataManager.Instance.Player;
        player.Transform = transform;

        // 当たり判定
        collisionDetector.onTriggerEnter.AddListener(OnCollisionDetectorEnter);
        collisionDetector.onTriggerExit.AddListener(OnCollisionDetectorExit);

        // 索敵判定
        serchDetector.onTriggerEnter.AddListener(OnSerchDetectorEnter);
        serchDetector.onTriggerExit.AddListener(OnSerchDetectorExit);

        // UI
        battleWindow.InitHpSlider(player.Hp);
        battleWindow.InitStrSlider(player.Str);
        battleWindow.InitMapUI(transform);

        move.recoveryStamina.AddListener(delegate{
            RecoveryStr();
        });
        move.decreasedStamina.AddListener(delegate{
            player.UpdateStr(-50*Time.fixedDeltaTime);
            battleWindow.UpdateStrSlider(player.CurrentStr);
        });

        // ステートマシン
        stateMachine = new StateMachine<PlayerController>(this);
        stateMachine.ChangeState(new Move());
    }

    private void FixedUpdate()
    {
        // インプットビュー
        if(NpcData != null){
            battleWindow.UpdateInputView("話しかける");
        }
        else{
            battleWindow.UpdateInputView();
        }

        // バリア
        if(player.BarrierTime > 0){
            player.UpdateBarrierTime(Time.fixedDeltaTime);
            if(player.BarrierTime == 0){
                barrierParticle.Stop(true);
            }
        }

        stateMachine.OnUpdate();
        stateName = stateMachine.CurrentState.ToString();
    }

    private class Move : StateBase<PlayerController> // 通常状態（立ち、走り、ダッシュ）
    {
        public override void OnStart()
        {
            Owner.move.enabled = true;
        }

        public override void OnUpdate()
        {
            Owner.move.isCanDash = Owner.player.CurrentStr > 0;
        }

        public override void OnEnd()
        {
            Owner.move.enabled = false;
        }
    }

    private class Attack : StateBase<PlayerController>
    {
        private int number = 0; // 番号
        private Player player = null;
        private Weapon weapon = null;
        private AttackController atkController = null;

        private bool isStart = false;
        private float countTime = 0f;
        private int count = 0;

        public override void OnStart()
        {
            isStart = false;
            countTime = 0f;
            count = 0;
            number = Owner.nextAttackNumber;
            Owner.nextAttackNumber = -1;

            player = Owner.player;
            weapon = Owner.player.WeaponList[number];

            AnimationDetector animDetector = Owner.animDetector;
            if(weapon.Data.WeaponType == WeaponType.Sword){
                atkController = Owner.attackControllerList[0];
            }else if(weapon.Data.WeaponType == WeaponType.Shield){
                atkController = Owner.attackControllerList[1];
            }else if(weapon.Data.WeaponType == WeaponType.Bow){
                atkController = Owner.attackControllerList[2];
            }

            animDetector.Init();
            animDetector.onAnimStart.AddListener(OnAnimStart);
            animDetector.onAttackCollisionStart.AddListener(delegate{
                atkController.Initialize(player.Atk);
                if(weapon.Data.WeaponType == WeaponType.Sword){
                    atkController.GetComponent<Collider>().enabled = true;
                }
            });
            animDetector.onAttackCollisionEnd.AddListener(delegate{
                if(weapon.Data.WeaponType == WeaponType.Sword){
                    atkController.GetComponent<Collider>().enabled = false;
                }
            });
            animDetector.onAnimEnd.AddListener(OnAnimEnd);

            Animator anim = Owner.anim;
            if(weapon.Data.WeaponType == WeaponType.Sword){
                anim.SetInteger("atkNum", 0);
            }else if(weapon.Data.WeaponType == WeaponType.Shield){
                anim.SetInteger("atkNum", 1);
            }else if(weapon.Data.WeaponType == WeaponType.Bow){
                anim.SetInteger("atkNum", 2);
            }
            anim.SetTrigger("atkTrigger");

            Owner.isGurd = false;
        }

        private void OnAnimStart()
        {
            foreach(Transform child in atkController.transform){
                Destroy(child.gameObject);
            }
            Instantiate(weapon.Data.Prefab, atkController.transform);

            Transform target = Owner.TargetTra;
            if(target){
                Vector3 dir = new Vector3(target.position.x-Owner.transform.position.x, 0, target.position.z-Owner.transform.position.z).normalized;
                Owner.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }

            if(weapon.Data.WeaponType == WeaponType.Sword){
                atkController.GetComponent<Collider>().enabled = false;
            }

            atkController.gameObject.SetActive(true);

            Owner.nextAttackNumber = -1;
            isStart = true;
            countTime = 0f;
            count = 0;

            Owner.isGurd = false;
        }

        private void OnAnimEnd()
        {
            if(weapon.Data.WeaponType == WeaponType.Sword){
                atkController.GetComponent<Collider>().enabled = false;
            }

            atkController.gameObject.SetActive(false);
            Owner.isGurd = false;
        }

        public override void OnUpdate()
        {
            if(isStart && Owner.anim.GetBool("isAttack")){
                if(countTime > 3f && count == 2){
                    count = 3;
                    Owner.effect.PlayParticle(1);
                    // 攻撃
                    atkController.Initialize(player.Atk*3);
                    // 回復
                    if(weapon.Recovery > 0){
                        player.UpdateHp(weapon.Recovery);
                        Owner.battleWindow.UpdateHpSlider(player.CurrentHp);
                    }
                    // バリア
                    if(weapon.GuardTime > 0){
                        player.InitBarrierTime(weapon.GuardTime);
                        Owner.barrierParticle.Play(true);
                    }

                }else if(countTime > 2f && count == 1){
                    count = 2;
                    Owner.effect.PlayParticle(1);
                }else if(countTime > 1f && count == 0){
                    count = 1;
                    Owner.effect.PlayParticle(1);
                }

                countTime += Time.fixedDeltaTime;

                Owner.isGurd = true;
            }else{
                Owner.isGurd = false;
            }

            // スタミナ
            Owner.RecoveryStr();

            // ステート変更
            if(Owner.animDetector.isAttackCombo && isStart){
                if(Owner.nextAttackNumber >= 0){
                    OnStart();
                }else if(Owner.anim.GetCurrentAnimatorStateInfo(0).IsName("Locomotion")){
                    Owner.stateMachine.ChangeState(new Move());
                }
            }
        }

        public override void OnEnd()
        {
            OnAnimEnd();
        }
    }

    private class Talk : StateBase<PlayerController>
    {
        public override void OnStart()
        {
            int number = 1;
            if(!DataManager.Instance.Data.IsFindNpcList[Owner.NpcData.Number - 1]){
                number = 0;
            }
            else{
                for(int i = 0; i < Owner.NpcData.TextDataList.Count; i++){
                    if(i >= 2){
                        if(Owner.NpcData.TextDataList[i].EventData.Number == DataManager.Instance.Data.EventNumber){
                            number = i;
                        }
                    }
                }
            }
            Owner.chatWindow.Init(Owner.NpcData.Name, Owner.NpcData.TextDataList[number].TextList);
        }

        public override void OnUpdate()
        {
            // スタミナ
            Owner.RecoveryStr();

            // ステート変更
            if(!Owner.chatWindow.gameObject.activeSelf){
                Owner.stateMachine.ChangeState(new Move());
            }
        }

        public override void OnEnd()
        {
            // NPCを見つけた
            DataManager.Instance.Data.UpdateFindNpc(Owner.NpcData.Number - 1);
        }
    }

    private class Hit : StateBase<PlayerController>
    {
        private bool isEnter = false; // 死亡処理をしたか

        public override void OnStart()
        {
            // フラグ初期化
            Owner.animDetector.Init();

            if(Owner.player.CurrentHp <= 0f){
                Owner.anim.SetInteger("hitNum",1);
            }
            else{
                Owner.anim.SetInteger("hitNum",0);
            }

            Owner.anim.SetTrigger("isHit");
        }

        public override void OnUpdate()
        {
            if(Owner.player.CurrentHp <= 0f & Owner.animDetector.isAnimEnd & !isEnter){
                isEnter = true;
                // 見つけた敵に追加
                Owner.GetComponent<Rigidbody>().useGravity = false;
                Owner.GetComponent<Collider>().isTrigger = true;
            }

            // ステート変更
            if(Owner.anim.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") & Owner.animDetector.isAnimEnd){
                Owner.stateMachine.ChangeState(new Move());
            }
        }
    }


   
    private void RecoveryStr()
    {
        if(player.CurrentStr < player.Str){
            player.UpdateStr(100*Time.deltaTime);
            battleWindow.UpdateStrSlider(player.CurrentStr);
        }
    }


    // 当たり判定
    private void OnCollisionDetectorEnter(Collider other)
    {
        npcControllerList.Add(other.GetComponent<NpcController>());
    }

    private void OnCollisionDetectorExit(Collider other)
    {
        npcControllerList.Remove(other.GetComponent<NpcController>());
    }


    // 索敵判定
    private void OnSerchDetectorEnter(Collider other)
    {
        targetTraList.Add(other.transform);
    }

    private void OnSerchDetectorExit(Collider other)
    {
        targetTraList.Remove(other.transform);
    }


    // InputSytem
    public void OnDo(InputValue value)
    {
        if(stateMachine.CurrentState.GetType() == typeof(Move)){
            if(NpcData != null){
                stateMachine.ChangeState(new Talk());
            }
        }
    }

    public void On_1(InputAction.CallbackContext context)
    {
        OnAttack(context, 0);
    }

    public void On_2(InputAction.CallbackContext context)
    {
        OnAttack(context, 1);
    }

    public void On_3(InputAction.CallbackContext context)
    {
        OnAttack(context, 2);
    }

    public void On_4(InputAction.CallbackContext context)
    {
        OnAttack(context, 3);
    }

    private void OnAttack(InputAction.CallbackContext context, int number)
    {
        if(context.phase == InputActionPhase.Started){
            anim.SetBool("isAttack", true);
            if(stateMachine.CurrentState.GetType() == typeof(Move)){
                if(player.WeaponList[number] != null){
                    nextAttackNumber = number;
                    stateMachine.ChangeState(new Attack());
                }
            }else if(stateMachine.CurrentState.GetType() == typeof(Attack)){
                if(player.WeaponList[number] != null){
                    nextAttackNumber = number;
                }
            }
        }else if(context.phase == InputActionPhase.Canceled){
            anim.SetBool("isAttack", false);
        }
    }


    // IBattler：ダメージ関数
    public void OnDamage(int damage, Vector3 position)
    {
        if (player.BarrierTime == 0 && !isGurd) {
            player.UpdateHp(-damage);
            battleWindow.UpdateHpSlider(player.CurrentHp);
            stateMachine.ChangeState(new Hit());
        }
    }
}
