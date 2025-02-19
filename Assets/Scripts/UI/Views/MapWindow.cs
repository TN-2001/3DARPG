using UnityEngine;

public class MapWindow : MonoBehaviour
{
    [SerializeField] private FollowUI playerFollow = null; // プレイヤーのアイコン
    [SerializeField] private Transform enemyContentTransform = null; // 敵のコンテンツ
    [SerializeField] private FollowUI enemyFollow = null; // 敵のアイコン


    private void OnEnable()
    {
        Time.timeScale = 0f;

        // 敵
        foreach(Transform transform in enemyContentTransform){
            Destroy(transform.gameObject);
        }
        foreach(Enemy enemy in DataManager.Instance.EnemyList){
            enemyFollow.targetTransform = enemy.Transform;
            Instantiate(enemyFollow.gameObject, enemyContentTransform);
        }
    }

    private void Start()
    {
        // プレイヤー
        playerFollow.targetTransform = DataManager.Instance.Player.Transform;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
