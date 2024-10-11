using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AnimationDetector), typeof(DamageDetector), typeof(PlayerMoveController))]
public class PlayerController : MonoBehaviour
{
    // ステート
    [SerializeField]
    private string stateName = null;
    private StateMachine<PlayerController> stateMachine;

    //コンポーネント
    // アニメーションコンポーネント
    private Animator anim = null;
    // 入力
    private PlayerInput input = null;
    // 移動
    private PlayerMoveController move = null;
    // アニメーションディテクター
    private AnimationDetector animDetector = null;
    // ダメージディテクター
    private DamageDetector damageDetector = null;

    // カメラコントローラー
    private CameraController cameraController = null;

    // キャラクター
    private Player player => GameManager.I.Data.Player;
    // 回復可能フラグ
    private bool isCanRecovery{get{
        ItemData itemData = GameManager.I.DataBase.ItemDataList[0];
        Item item = GameManager.I.Data.ItemList.Find(x => x.Data == itemData);
        int count = 0;
        if(item != null){
            count = item.Count;
        }
        if(player.CurrentHp < player.Hp & count > 0){
            return true;
        }
        else{
            return false;
        }
    }}
    // ダメージ
    private bool isDamage = false;
    // NPC
    private List<NpcController> npcControllerList = new List<NpcController>();
    private NpcData npcData{get{
        if(npcControllerList.Count > 0){
            return npcControllerList[0].NpcData;
        }
        else{
            return null;
        }
    }}
    // 敵
    private List<Transform> targetTraList = new List<Transform>();
    public Transform targetTra{get{
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
    }}

    [Header("BattleParameter")]
    [SerializeField] // 判定
    private CollisionDetector collisionDetector = null;
    [SerializeField] // 索敵判定
    private CollisionDetector serchDetector = null;
    [SerializeField] // 攻撃
    private AttackController attackController = null;
    [SerializeField] // バリアパーティクル
    private ParticleSystem barrierParticle = null;

    [Header("UI")]
    [SerializeField] // バトルウィンドウ
    private BattleWindow battleWindow = null;
    [SerializeField] // チャットウィンドウ
    private ChatWindow chatWindow = null;


    private void Start()
    {
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        move = GetComponent<PlayerMoveController>();
        animDetector = GetComponent<AnimationDetector>();
        damageDetector = GetComponent<DamageDetector>();

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

        damageDetector.onDamage.AddListener(delegate(int damage, Vector3 pos){
            if(player.BarrierTime == 0){
                player.UpdateHp(-damage);
                battleWindow.UpdateHpSlider(player.CurrentHp);
                isDamage = true;
                GetComponent<CharacterAudio>().PlayOneShot_Hit();
            }
        });

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

        // パーティクルストップ
        barrierParticle.Stop(true);

        // ステートマシン
        stateMachine = new StateMachine<PlayerController>(this);
        stateMachine.ChangeState(new Move());
    }

    private void FixedUpdate()
    {
        // インプットビュー
        if(npcData != null){
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
        stateName = stateMachine.currentState.ToString();
    }

    private class Move : StateBase<PlayerController>
    {
        public override void OnStart()
        {
            Owner.move.enabled = true;
        }

        public override void OnUpdate()
        {
            Owner.move.isCanDash = Owner.player.CurrentStr > 0;

            // ステート変更
            if(Owner.isDamage){
                Owner.stateMachine.ChangeState(new Hit());
                return;
            }
            else if(Owner.input.actions["Do"].WasPressedThisFrame() & Owner.npcData != null){
                Owner.stateMachine.ChangeState(new Talk());
                return;
            }
            for(int i = 0; i < 4; i++){
                if(Owner.input.actions[$"{i+1}"].WasPressedThisFrame() & Owner.player.WeaponList[i] != null){
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
        // 番号
        private readonly int number;
        // 次の番号
        private int nextNumber = -1;

        public Attack(int number)
        {
            this.number = number;
        }

        public override void OnStart()
        {
            // フラグ初期化
            Owner.animDetector.Init();
            Owner.animDetector.onAttackCollisionStart.AddListener(delegate{
                Owner.attackController.Initialize(Owner.player.Atk);
                Owner.attackController.GetComponent<Collider>().enabled = true;
                // 回復
                if(Owner.player.WeaponList[number].Recovery > 0){
                    Owner.player.UpdateHp(Owner.player.WeaponList[number].Recovery);
                    Owner.battleWindow.UpdateHpSlider(Owner.player.CurrentHp);
                }
                // バリア
                if(Owner.player.WeaponList[number].GuardTime > 0){
                    Owner.player.InitBarrierTime(Owner.player.WeaponList[number].GuardTime);
                    Owner.barrierParticle.Play(true);
                }
            });
            Owner.animDetector.onAttackCollisionEnd.AddListener(delegate{
                Owner.attackController.GetComponent<Collider>().enabled = false;
            });

            // 武器
            GameObject weaponObj = Owner.player.WeaponList[number].Data.Prefab;
            foreach(Transform child in Owner.attackController.transform){
                Destroy(child.gameObject);
            }
            Instantiate(weaponObj, Owner.attackController.transform);

            if(Owner.player.WeaponList[number].Data.WeaponType == WeaponType.Sword){
                Owner.anim.SetInteger("atkNum", 0);
            }
            else if(Owner.player.WeaponList[number].Data.WeaponType == WeaponType.Wand){
                Owner.anim.SetInteger("atkNum", 1);
            }
            Owner.anim.SetTrigger("isAtk");
            Owner.attackController.GetComponent<Collider>().enabled = false;
            Owner.attackController.gameObject.SetActive(true);
            if(Owner.targetTra){
                Vector3 dir = new Vector3(Owner.targetTra.position.x-Owner.transform.position.x, 0, Owner.targetTra.position.z-Owner.transform.position.z).normalized;
                Owner.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }

        public override void OnUpdate()
        {
            if(nextNumber == -1 & Owner.animDetector.isAnimStart){
                for(int i = 0; i < 4; i++){
                    if(Owner.input.actions[$"{i+1}"].WasPressedThisFrame() & Owner.player.WeaponList[i] != null){
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
            if(!GameManager.I.Data.IsFindNpcList[Owner.npcData.Number - 1]){
                number = 0;
            }
            else{
                for(int i = 0; i < Owner.npcData.TextDataList.Count; i++){
                    if(i >= 2){
                        if(Owner.npcData.TextDataList[i].EventData.Number == GameManager.I.Data.EventNumber){
                            number = i;
                        }
                    }
                }
            }
            Owner.chatWindow.Init(Owner.npcData.Name, Owner.npcData.TextDataList[number].TextList);
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
            GameManager.I.Data.UpdateFindNpc(Owner.npcData.Number - 1);
        }
    }

    private class Hit : StateBase<PlayerController>
    {
        // 死亡処理をしたか
        private bool isEnter = false;

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
            Owner.isDamage = false;
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
}
