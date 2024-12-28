using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AnimationDetector), typeof(PlayerMoveController))]
public class PlayerController : MonoBehaviour, IBattler
{
    // ステート
    [SerializeField] private string stateName = null;
    private StateMachine<PlayerController> stateMachine;

    //コンポーネント
    private Animator anim = null; // アニメーションコンポーネント
    private PlayerInput input = null; // 入力
    private PlayerMoveController move = null; // 移動
    private AnimationDetector animDetector = null; // アニメーションディテクター

    // カメラコントローラー
    private CameraController cameraController = null;

    // キャラクター
    private Player Player => GameManager.I.Data.Player;
    // ダメージ
    private bool isDamage = false;
    // NPC
    private readonly List<NpcController> npcControllerList = new();
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
    // 敵
    private readonly List<Transform> targetTraList = new();
    public Transform TargetTra
    {
        get{
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
    [SerializeField] private CollisionDetector collisionDetector = null; // 判定
    [SerializeField] private CollisionDetector serchDetector = null; // 索敵判定
    [SerializeField] private AttackController attackController = null; // 攻撃
    [SerializeField] private ParticleSystem barrierParticle = null; // バリアパーティクル

    [Header("UI")]
    [SerializeField] private BattleWindow battleWindow = null; // バトルウィンドウ
    [SerializeField] private ChatWindow chatWindow = null; // チャットウィンドウ


    private void Start()
    {
        // コンポーネント入手
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        move = GetComponent<PlayerMoveController>();
        animDetector = GetComponent<AnimationDetector>();

        cameraController = Camera.main.GetComponent<CameraController>();

        collisionDetector.onTriggerEnter.AddListener(delegate(Collider other){
            npcControllerList.Add(other.GetComponent<NpcController>());
        });
        collisionDetector.onTriggerExit.AddListener(delegate(Collider other){
            npcControllerList.Remove(other.GetComponent<NpcController>());
        });

        serchDetector.onTriggerEnter.AddListener(delegate(Collider other){
            targetTraList.Add(other.transform);
        });
        serchDetector.onTriggerExit.AddListener(delegate(Collider other){
            targetTraList.Remove(other.transform);
        });

        battleWindow.InitHpSlider(Player.Hp);
        battleWindow.InitStrSlider(Player.Str);
        battleWindow.InitMapUI(transform);

        move.recoveryStamina.AddListener(delegate{
            RecoveryStr();
        });
        move.decreasedStamina.AddListener(delegate{
            Player.UpdateStr(-50*Time.fixedDeltaTime);
            battleWindow.UpdateStrSlider(Player.CurrentStr);
        });

        // パーティクルストップ
        barrierParticle.Stop(true);

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
        if(Player.BarrierTime > 0){
            Player.UpdateBarrierTime(Time.fixedDeltaTime);
            if(Player.BarrierTime == 0){
                barrierParticle.Stop(true);
            }
        }

        stateMachine.OnUpdate();
        stateName = stateMachine.currentState.ToString();
    }

    private class Move : StateBase<PlayerController> // 通常状態（立ち、走り、ダッシュ）
    {
        public override void OnStart()
        {
            Owner.move.enabled = true;
        }

        public override void OnUpdate()
        {
            Owner.move.isCanDash = Owner.Player.CurrentStr > 0;

            // ステート変更
            if(Owner.isDamage){
                Owner.stateMachine.ChangeState(new Hit());
                return;
            }
            else if(Owner.input.actions["Do"].WasPressedThisFrame() & Owner.NpcData != null){
                Owner.stateMachine.ChangeState(new Talk());
                return;
            }
            for(int i = 0; i < 4; i++){
                if(Owner.input.actions[$"{i+1}"].WasPressedThisFrame() & Owner.Player.WeaponList[i] != null){
                    Owner.stateMachine.ChangeState(new Attack(i));
                    return;
                }
            }
        }

        public override void OnEnd()
        {
            Owner.move.enabled = false;
        }
    }

    private class Attack : StateBase<PlayerController>
    {
        private readonly int number = 0; // 番号
        private int nextNumber = -1; // 次の番号

        public Attack(int number)
        {
            this.number = number;
        }

        public override void OnStart()
        {
            // フラグ初期化
            Owner.animDetector.Init();
            Owner.animDetector.onAttackCollisionStart.AddListener(delegate{
                Owner.attackController.Initialize(Owner.Player.Atk);
                Owner.attackController.GetComponent<Collider>().enabled = true;
                // 回復
                if(Owner.Player.WeaponList[number].Recovery > 0){
                    Owner.Player.UpdateHp(Owner.Player.WeaponList[number].Recovery);
                    Owner.battleWindow.UpdateHpSlider(Owner.Player.CurrentHp);
                }
                // バリア
                if(Owner.Player.WeaponList[number].GuardTime > 0){
                    Owner.Player.InitBarrierTime(Owner.Player.WeaponList[number].GuardTime);
                    Owner.barrierParticle.Play(true);
                }
            });
            Owner.animDetector.onAttackCollisionEnd.AddListener(delegate{
                Owner.attackController.GetComponent<Collider>().enabled = false;
            });

            // 武器
            GameObject weaponObj = Owner.Player.WeaponList[number].Data.Prefab;
            foreach(Transform child in Owner.attackController.transform){
                Destroy(child.gameObject);
            }
            Instantiate(weaponObj, Owner.attackController.transform);

            if(Owner.Player.WeaponList[number].Data.WeaponType == WeaponType.Sword){
                Owner.anim.SetInteger("atkNum", 0);
            }
            else if(Owner.Player.WeaponList[number].Data.WeaponType == WeaponType.Wand){
                Owner.anim.SetInteger("atkNum", 1);
            }
            Owner.anim.SetTrigger("isAtk");
            Owner.attackController.GetComponent<Collider>().enabled = false;
            Owner.attackController.gameObject.SetActive(true);
            if(Owner.TargetTra){
                Vector3 dir = new Vector3(Owner.TargetTra.position.x-Owner.transform.position.x, 0, Owner.TargetTra.position.z-Owner.transform.position.z).normalized;
                Owner.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }

        public override void OnUpdate()
        {
            if(nextNumber == -1 & Owner.animDetector.isAnimStart){
                for(int i = 0; i < 4; i++){
                    if(Owner.input.actions[$"{i+1}"].WasPressedThisFrame() & Owner.Player.WeaponList[i] != null){
                        nextNumber = i;
                    }
                }
            }

            // スタミナ
            Owner.RecoveryStr();

            // ステート変更
            if(Owner.isDamage){
                Owner.stateMachine.ChangeState(new Hit());
            }
            else if(Owner.animDetector.isAttackCombo & nextNumber != -1){
                Owner.stateMachine.ChangeState(new Attack(nextNumber));
            }
            else if(Owner.animDetector.isAnimEnd){
                Owner.stateMachine.ChangeState(new Move());
            }
        }

        public override void OnEnd()
        {
            Owner.attackController.gameObject.SetActive(false);
        }
    }

    private class Talk : StateBase<PlayerController>
    {
        public override void OnStart()
        {
            // カメラ操作できなくする
            Owner.cameraController.IsMove(false);
            int number = 1;
            if(!GameManager.I.Data.IsFindNpcList[Owner.NpcData.Number - 1]){
                number = 0;
            }
            else{
                for(int i = 0; i < Owner.NpcData.TextDataList.Count; i++){
                    if(i >= 2){
                        if(Owner.NpcData.TextDataList[i].EventData.Number == GameManager.I.Data.EventNumber){
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
            // カメラを初期化
            Owner.cameraController.IsMove(true);
            // NPCを見つけた
            GameManager.I.Data.UpdateFindNpc(Owner.NpcData.Number - 1);
        }
    }

    private class Hit : StateBase<PlayerController>
    {
        private bool isEnter = false; // 死亡処理をしたか

        public override void OnStart()
        {
            // フラグ初期化
            Owner.animDetector.Init();

            if(Owner.Player.CurrentHp <= 0f){
                Owner.anim.SetInteger("hitNum",1);
            }
            else{
                Owner.anim.SetInteger("hitNum",0);
            }

            Owner.anim.SetTrigger("isHit");
            Owner.isDamage = false;
        }

        public override void OnUpdate()
        {
            if(Owner.Player.CurrentHp <= 0f & Owner.animDetector.isAnimEnd & !isEnter){
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
        if(Player.CurrentStr < Player.Str){
            Player.UpdateStr(100*Time.deltaTime);
            battleWindow.UpdateStrSlider(Player.CurrentStr);
        }
    }

    // IBattlerのダメージ関数
    public void OnDamage(int damage, Vector3 position)
    {
        if (Player.BarrierTime == 0) {
            Player.UpdateHp(-damage);
            battleWindow.UpdateHpSlider(Player.CurrentHp);
            isDamage = true;
            GetComponent<CharacterAudio>().PlayOneShot_Hit();
        }
    }
}
