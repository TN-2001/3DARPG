using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContent : MonoBehaviour {
    [SerializeField] private List<EnemyInfomation> enemyInfomationList = new(); // 敵の情報リスト
    [SerializeField] private Transform playerTransform = null; // プレイヤーの位置
    [SerializeField] private float instanceDistance = 100f; // 敵が生成できるプレイヤーとの距離
    [SerializeField] private float instanceTime = 60f; // 敵が生成できる時間

    [Header("UI")]
    [SerializeField] // バトルウィンドウ
    private BattleWindow battleWindow = null;
    public BattleWindow BattleWindow => battleWindow;


    private void Start() {
        // 敵の情報リスト
        foreach (EnemyInfomation enemyInfomation in enemyInfomationList) {
            enemyInfomation.Owner = this;
            enemyInfomation.InitEnemyController();
        }
    }


    [System.Serializable] // 敵の情報
    private class EnemyInfomation {
        [SerializeField] private List<GameObject> enemyPefabList = new(); // 現れる敵のプレハブリスト
        [SerializeField] private EnemyController enemyController = null; // 現在の敵コントローラ
        [SerializeField] private Transform initTransform = null; // 初期トランスフォーム

        public EnemyContent Owner { get; set; } // 親クラス


        private IEnumerator OnDie() {
            // 1分待ち、プレイヤーが一定距離離れるまで待つ
            yield return new WaitForSeconds(Owner.instanceTime);
            yield return new WaitUntil(() => Vector3.Distance(Owner.playerTransform.position, initTransform.position) > Owner.instanceDistance);

            // 敵を生成
            GameObject enemyObject = Instantiate(
                enemyPefabList[Random.Range(0, enemyPefabList.Count)],
                initTransform.position,
                initTransform.rotation,
                Owner.transform
            );
            enemyController = enemyObject.GetComponent<EnemyController>();
            InitEnemyController();
        }

        public void InitEnemyController() // 敵コントローラの設定
        {
            enemyController.onDie.AddListener(() => Owner.StartCoroutine(OnDie()));
        }
    }
}
