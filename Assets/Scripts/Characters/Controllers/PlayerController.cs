using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IBattler {
    // ステート
    [SerializeField] private string stateName = null;
    private StateMachine<PlayerController> stateMachine = null;

    //コンポーネント
    private Animator anim = null; // アニメーションコンポーネント
    private PlayerCharacterController move = null; // 移動

    private Player player = null; // プレイヤー
    private readonly List<NpcController> npcControllerList = new(); // NPC
    private NpcData NpcData {
        get {
            if (npcControllerList.Count > 0) {
                return npcControllerList[0].NpcData;
            } else {
                return null;
            }
        }
    }
    private readonly List<Transform> targetTraList = new(); // 敵
    public Transform TargetTra {
        get {
            for (int i = targetTraList.Count - 1; i >= 0; i--) {
                if (targetTraList[i] == null) {
                    targetTraList.RemoveAt(i);
                }
            }
            Transform tra = null;
            for (int i = 0; i < targetTraList.Count; i++) {
                if (i == 0) {
                    tra = targetTraList[0];
                } else {
                    Vector3 dis1 = transform.position - tra.position;
                    Vector3 dis2 = transform.position - targetTraList[i].position;
                    if (dis1.magnitude > dis2.magnitude) {
                        tra = targetTraList[i];
                    }
                }
            }
            return tra;
        }
    }

    [Header("Component")] // コンポーネント
    [SerializeField] private CollisionDetector collisionDetector = null; // 当たり判定
    [SerializeField] private CollisionDetector serchDetector = null; // 索敵判定
    [SerializeField] private ParticleSystem barrierParticle = null; // バリアパーティクル
    [SerializeField] private List<WeaponController> weaponControllerList = new(); // 武器リスト
    [SerializeField] private List<AttackStatus> attackStatusList = new(); // 攻撃ステータスリスト

    [Header("UI")]
    [SerializeField] private BattleWindow battleWindow = null; // バトルウィンドウ
    [SerializeField] private ChatWindow chatWindow = null; // チャットウィンドウ
    [SerializeField] private LoadView loadView = null; // ロード画面
    [SerializeField] private CinemachinePanTilt cinemachinePanTilt = null; // カメラ

    // 初期値
    private Vector3 initPostion = Vector3.zero;
    private Quaternion initRotation = Quaternion.identity;
    private Quaternion initCameraRotation = Quaternion.identity;

    // フラグ
    private bool isGuard = false; // ガード状態か
    private bool isGuardInput = false; // ガード入力があったか
    private bool isAttackInput = false; // 攻撃の入力があったか
    private bool isCanCombo = true; // コンボ可能か
    private bool isDahInput = false; // ダッシュ入力を中か
    private bool isCanInput = false; // 入力を受け付けるか
    private bool isStrRecovery = false; // スタミナ回復するか
    private int attackNumber = 0; // 攻撃番号
    private int attackCollisionNumber = 0; // 攻撃当たり判定番号


    private void Start() {
        // コンポーネント入手
        anim = GetComponent<Animator>();
        move = GetComponent<PlayerCharacterController>();

        // キャラクター
        player = DataManager.Instance.Player;
        player.Transform = transform;
        player.UpdateHp(player.Hp);

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

        // 移動時の関数
        move.recoveryStamina.AddListener(() => UpdateStr(30 * Time.deltaTime));
        move.decreasedStamina.AddListener(() => UpdateStr(-10 * Time.fixedDeltaTime));

        // 武器
        weaponControllerList[attackCollisionNumber].EnableWeapon(player.Weapon.Data.LeftObject, player.Weapon.Data.RightObject);
        foreach (AttackStatus attackStatus in attackStatusList) {
            attackStatus.Init(player);
        }

        // 初期値保存
        initPostion = transform.position;
        initRotation = transform.rotation;
        initCameraRotation = Camera.main.transform.rotation;

        // ステートマシン
        stateMachine = new StateMachine<PlayerController>(this);
        stateMachine.ChangeState(new Move());
    }

    private void FixedUpdate() {
        // インプットビュー
        if (NpcData != null) {
            battleWindow.UpdateInputView("話しかける");
        } else {
            battleWindow.UpdateInputView();
        }

        // バリア
        if (player.BarrierTime > 0) {
            player.UpdateBarrierTime(Time.fixedDeltaTime);
            if (player.BarrierTime == 0) {
                barrierParticle.Stop(true);
            }
        }

        // スタミナ回復
        if (isStrRecovery) {
            UpdateStr(100 * Time.deltaTime);
        }

        stateMachine.OnUpdate();
        stateName = stateMachine.CurrentStateType.ToString();
    }

    private class Move : StateBase<PlayerController> // 通常状態（立ち、走り、ダッシュ）
    {
        public override void OnStart() {
            Owner.isCanInput = true;
            Owner.move.isOn = true;
            Owner.isStrRecovery = false;
        }

        public override void OnUpdate() {
            Owner.move.isCanDash = Owner.player.CurrentStr > 0;
        }

        public override void OnEnd() {
            Owner.move.isOn = false;
            Owner.isStrRecovery = true;
        }
    }

    private class Roll : StateBase<PlayerController> // 回避
    {
        public override void OnStart() {
            Owner.anim.SetTrigger("rollTrigger");
            Owner.isStrRecovery = false;
        }

        public override void OnEnd() {
            Owner.isStrRecovery = true;
        }
    }

    private class BattleMove : StateBase<PlayerController> // 抜刀状態（立ち、走り）
    {
        public override void OnStart() {
            Owner.isCanInput = true;
            Owner.move.isOn = true;
            Owner.move.isCanDash = false;
            Owner.isStrRecovery = false;
        }

        public override void OnUpdate() {
            // 武器しまうアニメーション
            if (Owner.isDahInput && Owner.anim.GetInteger("stateNumber") != 0) {
                Owner.stateMachine.ChangeState(new SheathWeapon());
            }
        }

        public override void OnEnd() {
            Owner.anim.SetLayerWeight(1, 0);

            if (Owner.stateMachine.NextStateType != typeof(SheathWeapon)) {
                Owner.move.isOn = false;
            }

            Owner.isStrRecovery = true;
        }
    }

    private class SheathWeapon : StateBase<PlayerController> // 納刀
    {
        public override void OnUpdate() {
            if (Owner.weaponControllerList[0].IsDraw) {
                if (Owner.anim.GetLayerWeight(1) >= 1) {
                    Owner.anim.SetInteger("stateNumber", 0);
                } else {
                    Owner.anim.SetLayerWeight(1, Owner.anim.GetLayerWeight(1) + Time.fixedDeltaTime / 0.2f);
                }
            } else {
                if (Owner.anim.GetLayerWeight(1) <= 0) {
                    Owner.stateMachine.ChangeState(new Move());
                } else {
                    Owner.anim.SetLayerWeight(1, Owner.anim.GetLayerWeight(1) - Time.fixedDeltaTime / 0.2f);
                }
            }
        }

        public override void OnEnd() {
            Owner.anim.SetInteger("stateNumber", 0);
            Owner.anim.SetLayerWeight(1, 0);
            Owner.weaponControllerList[0].SheathWeapon();

            if (Owner.stateMachine.NextStateType != typeof(Move)) {
                Owner.move.isOn = false;
            }
        }
    }

    private class Attack : StateBase<PlayerController> // 攻撃
    {
        public override void OnStart() {
            Owner.OnAnimationStart();

            if (!Owner.weaponControllerList[0].IsDraw) {
                Owner.weaponControllerList[0].DrawWeapon();
            }

            Owner.anim.SetInteger("stateNumber", 1);
            Owner.anim.SetTrigger("attackTrigger");
        }

        public override void OnEnd() {
            Owner.OnAttackCollisionDisable();
        }
    }

    private class Guard : StateBase<PlayerController> // ガード
    {
        public override void OnStart() {
            if (!Owner.weaponControllerList[0].IsDraw) {
                Owner.weaponControllerList[0].DrawWeapon();
            }

            Owner.anim.SetInteger("stateNumber", 1);
            Owner.anim.SetTrigger("guardTrigger");
        }

        public override void OnEnd() {
            Owner.OnGuardDisable();
            Owner.anim.SetBool("isGuard", false);
        }
    }

    private class Talk : StateBase<PlayerController> {
        public override void OnStart() {
            int number = 1;
            if (!DataManager.Instance.Data.IsFindNpcList[Owner.NpcData.Number - 1]) {
                number = 0;
            } else {
                for (int i = 0; i < Owner.NpcData.TextDataList.Count; i++) {
                    if (i >= 2) {
                        if (Owner.NpcData.TextDataList[i].EventData.Number == DataManager.Instance.Data.EventNumber) {
                            number = i;
                        }
                    }
                }
            }
            Owner.chatWindow.Init(Owner.NpcData.Name, Owner.NpcData.TextDataList[number].TextList);
        }

        public override void OnUpdate() {
            // ステート変更
            if (!Owner.chatWindow.gameObject.activeSelf) {
                if (Owner.weaponControllerList[Owner.attackCollisionNumber].IsDraw) {
                    Owner.stateMachine.ChangeState(new BattleMove());
                } else {
                    Owner.stateMachine.ChangeState(new Move());
                }
            }
        }

        public override void OnEnd() {
            // NPCを見つけた
            DataManager.Instance.Data.UpdateFindNpc(Owner.NpcData.Number - 1);
        }
    }

    private class Hit : StateBase<PlayerController> {
        public override void OnStart() {
            if (Owner.player.CurrentHp <= 0f) {
                Owner.anim.SetInteger("hitNum", 2);
            } else {
                Owner.anim.SetInteger("hitNum", 0);
            }

            Owner.anim.SetTrigger("isHit");
        }
    }

    private void OnDisable() {
        // データベースイベント
        DataManager.Instance.Data.onChageWeapon.RemoveListener(OnChangeWeapon);
    }


    // スタミナ回復
    private void UpdateStr(float value) {
        if ((player.CurrentStr < player.Str && value > 0) || (player.CurrentStr > 0 && value < 0)) {
            player.UpdateStr(value);
            battleWindow.UpdateStrSlider(player.CurrentStr);
        }
    }

    // プレイヤー死亡
    private void OnDie() {
        StartCoroutine(loadView.ELoadEvent(delegate {
            anim.Play("Locomotion");
            stateMachine.ChangeState(new Move());
            CharacterController con = GetComponent<CharacterController>();
            con.enabled = false;
            transform.SetPositionAndRotation(initPostion, initRotation);
            con.enabled = true;
            weaponControllerList[attackCollisionNumber].SheathWeapon();
            player.UpdateHp(player.Hp);
            battleWindow.UpdateHpSlider(player.CurrentHp);
            cinemachinePanTilt.PanAxis.Value = initCameraRotation.eulerAngles.y;
        }));
    }

    // 当たり判定
    private void OnCollisionDetectorEnter(Collider other) {
        npcControllerList.Add(other.GetComponent<NpcController>());
    }

    private void OnCollisionDetectorExit(Collider other) {
        npcControllerList.Remove(other.GetComponent<NpcController>());
    }


    // 索敵判定
    private void OnSerchDetectorEnter(Collider other) {
        targetTraList.Add(other.transform);
    }

    private void OnSerchDetectorExit(Collider other) {
        targetTraList.Remove(other.transform);
    }


    // InputSytem
    public void OnDash(InputAction.CallbackContext context) // ダッシュ
    {
        if (context.phase == InputActionPhase.Started) {
            isDahInput = true;
        } else if (context.phase == InputActionPhase.Canceled) {
            isDahInput = false;
        }
    }

    public void OnEvent(InputAction.CallbackContext context) // 会話、剥ぎ取り、採取
    {
        if (context.phase == InputActionPhase.Started) {
            if (stateMachine.CurrentStateType == typeof(Move)) {
                if (NpcData != null) {
                    stateMachine.ChangeState(new Talk());
                }
            }
        }
    }

    public void OnItem(InputAction.CallbackContext context) // アイテム使用
    {
        if (context.phase == InputActionPhase.Started) {
            if (stateMachine.CurrentStateType == typeof(Move) || stateMachine.CurrentStateType == typeof(BattleMove)) {
            }
        }
    }

    public void OnRoll(InputAction.CallbackContext context) // 回避
    {
        if (context.phase == InputActionPhase.Started) {
            if (stateMachine.CurrentStateType == typeof(Move) || stateMachine.CurrentStateType == typeof(BattleMove)) {
                if (player.CurrentStr > 30) {
                    stateMachine.ChangeState(new Roll());
                    UpdateStr(-30);
                }
            }
        }
    }

    public void OnGuard(InputAction.CallbackContext context) // ガード
    {
        if (context.phase == InputActionPhase.Started) {
            anim.SetBool("isGuard", true);

            if (isCanInput || stateMachine.CurrentStateType == typeof(Move) || stateMachine.CurrentStateType == typeof(BattleMove)) {
                if (stateMachine.CurrentStateType == typeof(Move) || stateMachine.CurrentStateType == typeof(BattleMove)) {
                    stateMachine.ChangeState(new Guard());
                } else if (stateMachine.CurrentStateType == typeof(Attack)) {
                    if (isCanCombo) {
                        stateMachine.ChangeState(new Guard());
                    } else {
                        isGuardInput = true;
                    }
                }
            }
        } else if (context.phase == InputActionPhase.Canceled) {
            anim.SetBool("isGuard", false);
        }
    }

    public void OnAttack(InputAction.CallbackContext context) // 攻撃
    {
        if (NpcData != null) {
            stateMachine.ChangeState(new Talk());
        } else {
            OnAttack(context, 0);
        }
    }

    public void OnTiltAttack(InputAction.CallbackContext context) // 強攻撃
    {
        OnAttack(context, 1);
    }

    private void OnAttack(InputAction.CallbackContext context, int number) {
        if (context.phase == InputActionPhase.Started) {
            anim.SetInteger("attackNumber", number);

            if (isCanInput || stateMachine.CurrentStateType == typeof(Move) || stateMachine.CurrentStateType == typeof(BattleMove)) {
                if (stateMachine.CurrentStateType == typeof(Move) || stateMachine.CurrentStateType == typeof(BattleMove)) {
                    stateMachine.ChangeState(new Attack());
                } else if (stateMachine.CurrentStateType == typeof(Attack) || stateMachine.CurrentStateType == typeof(Guard)) {
                    if (isCanCombo) {
                        stateMachine.ChangeState(new Attack());
                    } else {
                        isAttackInput = true;
                    }
                }
            }
        }
    }



    // IBattler：ダメージ関数
    public void OnDamage(AttackStatus attack, Vector3 position) {
        if (player.BarrierTime == 0 && !isGuard) {
            player.UpdateHp(-attack.Atk);
            battleWindow.UpdateHpSlider(player.CurrentHp);
            stateMachine.ChangeState(new Hit());
        }
    }


    // アニメーションイベント
    public void OnAnimationStart() // アニメーションスタート
    {
        isCanCombo = false;
        isAttackInput = false;
        isGuardInput = false;
        isCanInput = false;
    }

    public void OnAttackAnimationStart(int number) // 攻撃時アニメーションスタート
    {
        attackNumber = number;
        OnAnimationStart();
    }

    public void OnAnimationEnd() // アニメーション終了
    {
        if (stateMachine.CurrentStateType == typeof(Hit) && player.CurrentHp == 0) {
            OnDie();
        } else if ((stateMachine.CurrentStateType == typeof(Roll) && !weaponControllerList[0].IsDraw)
            || (stateMachine.CurrentStateType == typeof(Hit) && !weaponControllerList[0].IsDraw && player.CurrentHp > 0)) {
            stateMachine.ChangeState(new Move());
        } else if ((stateMachine.CurrentStateType == typeof(Roll) && weaponControllerList[0].IsDraw)
            || stateMachine.CurrentStateType == typeof(Guard)
            || stateMachine.CurrentStateType == typeof(Attack)
            || (stateMachine.CurrentStateType == typeof(Hit) && weaponControllerList[0].IsDraw && player.CurrentHp > 0)) {
            stateMachine.ChangeState(new BattleMove());
        }
    }

    public void OnSheatWeapon() // 武器をしまう
    {
        weaponControllerList[0].SheathWeapon();
    }

    public void OnAttackCollisionEnable(int number) {
        attackCollisionNumber = number;
        weaponControllerList[attackCollisionNumber].AttackController.Initialize(attackStatusList[attackNumber]);
        weaponControllerList[attackCollisionNumber].AttackController.OnCollisionEnable();
    }

    public void OnAttackCollisionDisable() {
        weaponControllerList[attackCollisionNumber].AttackController.OnCollisionDisable();
    }

    public void OnInputEnable() {
        isCanInput = true;
    }

    public void OnInputDisable() {
        isCanInput = false;
    }

    public void OnCanCombo() // コンボ可能
    {
        if (isAttackInput) {
            stateMachine.ChangeState(new Attack());
        } else if (isGuardInput) {
            stateMachine.ChangeState(new Guard());
        } else {
            isCanCombo = true;
        }
    }

    public void OnGuardEnable() // ガードOn
    {
        isGuard = true;
    }

    public void OnGuardDisable() // ガードOff
    {
        isGuard = false;
    }


    // データベースイベント
    public void OnChangeWeapon(Weapon weapon) {
        weapon ??= player.Weapon;
        weaponControllerList[attackCollisionNumber].ChangeWeapon(weapon.Data.LeftObject, weapon.Data.RightObject);
    }


    [System.Serializable] // 攻撃
    private class WeaponController {
        [SerializeField] private WeaponType type = WeaponType.SwordAndShield; // 武器のタイプ
        [SerializeField] private GameObject leftObject = null; // 左手の武器
        [SerializeField] private GameObject rightObject = null; // 右手の武器
        [SerializeField] private GameObject leftSheathObject = null; // 左手の収納武器
        [SerializeField] private GameObject rightSheathObject = null; // 右手の収納武器
        [SerializeField] private AttackController attackController = null;

        public WeaponType Type => type;
        public AttackController AttackController => attackController;

        private bool isDraw = false;
        public bool IsDraw => isDraw;


        public void EnableWeapon(GameObject leftPrefab, GameObject rightPrefab) // 武器を表示
        {
            ChangeWeapon(leftPrefab, rightPrefab);

            if (leftSheathObject != null) {
                leftSheathObject.SetActive(true);
            } else if (leftObject != null) {
                leftObject.SetActive(true);
            }

            if (rightSheathObject != null) {
                rightSheathObject.SetActive(true);
            } else if (rightObject != null) {
                rightObject.SetActive(true);
            }
        }

        public void ChangeWeapon(GameObject leftPrefab, GameObject rightPrefab) // 武器を変える
        {
            if (leftSheathObject != null) {
                ChangeObject(leftSheathObject, leftPrefab);
            }
            if (leftObject != null) {
                ChangeObject(leftObject, leftPrefab);
            }

            if (rightSheathObject != null) {
                ChangeObject(rightSheathObject, rightPrefab);
            }
            if (rightObject != null) {
                ChangeObject(rightObject, rightPrefab);
            }
        }

        private void ChangeObject(GameObject parentObject, GameObject newObject) {
            foreach (Transform child in parentObject.transform) {
                Destroy(child.gameObject);
            }
            Instantiate(newObject, parentObject.transform);
        }


        public void DrawWeapon() // 武器を構える
        {
            if (leftObject != null && leftSheathObject != null) {
                leftObject.SetActive(true);
                leftSheathObject.SetActive(false);
            }
            if (rightObject != null && rightSheathObject != null) {
                rightObject.SetActive(true);
                rightSheathObject.SetActive(false);
            }
            isDraw = true;
        }

        public void SheathWeapon() // 武器をしまう
        {
            if (leftObject != null && leftSheathObject != null) {
                leftObject.SetActive(false);
                leftSheathObject.SetActive(true);
            }
            if (rightObject != null && rightSheathObject != null) {
                rightObject.SetActive(false);
                rightSheathObject.SetActive(true);
            }
            isDraw = false;
        }

        public void DiableWeapon() // 武器を非表示
        {
            if (leftSheathObject != null) {
                leftSheathObject.SetActive(false);
            }
            if (leftObject != null) {
                leftObject.SetActive(false);
            }
            if (rightSheathObject != null) {
                rightSheathObject.SetActive(false);
            }
            if (rightObject != null) {
                rightObject.SetActive(false);
            }
        }
    }
}
