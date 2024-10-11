using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContent : MonoBehaviour
{
    [Header("キャラクター")]
    [SerializeField] // 敵プレハブ
    private List<GameObject> enemyPrefabList = new();
    [SerializeField] // 設置場所
    private List<Transform> enemyTraList = new();

    // 時間
    private List<float> timeList = new List<float>();

    [Header("UI")]
    [SerializeField] // バトルウィンドウ
    private BattleWindow battleWindow = null;
    public BattleWindow BattleWindow => battleWindow;


    private void Start()
    {
        for(int i = 0; i < enemyTraList.Count; i++){
            timeList.Add(-1);
        }
    }

    private void Update()
    {
        for(int i = 0; i < enemyTraList.Count; i++){
            if(timeList[i] > 300){
                timeList[i] = -1;
                Instantiate(enemyPrefabList[Random.Range(0, enemyPrefabList.Count)], enemyTraList[i]);
            }
            else if(timeList[i] >= 0){
                timeList[i] += Time.deltaTime;
            }
            else if(enemyTraList[i].childCount == 0){
                timeList[i] = 0;
            }
        }
    }
}
