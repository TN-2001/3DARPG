using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SearchDetector : MonoBehaviour
{
    [Header("物理設定でレイがTriggerにはあたらないようにしよう")]
    [Header("高さは目線に合わせた方がいい")]
    // 設定パラメータ
    [SerializeField] // 当たり判定の対象のタグ
    public string tagName = "Player";
    [SerializeField] // 視野角
    private float angle = 45f;

    // 変数
    // 範囲内の全てのコライダー
    public List<Collider> colliderList { get; private set; } = new List<Collider>();
    // 最も近いコライダー
    public Collider closeCollider {get{
        Collider collider = null;
        for(int i = 0; i < colliderList.Count; i++){
            if(collider){
                float dis0 = Vector3.Distance(transform.position, collider.transform.position);
                float dis1 = Vector3.Distance(transform.position, colliderList[i].transform.position);
                if(dis1 < dis0){
                    collider = colliderList[i];
                }
            }
            else{
                collider = colliderList[i];
            }
        }
        return collider;
    }}

    [HideInInspector] // 引数にColliderを持ったUnityEvent
    public UnityEvent<Collider> onEnter, onExit;


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == tagName) //視界の範囲内の当たり判定
        {
            //視界の角度内に収まっているか
            Vector3 posDelta = other.transform.position - this.transform.position;
            float target_angle = Vector3.Angle(this.transform.forward, posDelta);

            if (target_angle < angle) //target_angleがangleに収まっているかどうか
            {
                if(Physics.Raycast(this.transform.position, posDelta, out RaycastHit hit)) //Rayを使用してtargetに当たっているか判別
                {
                    if (hit.collider==other)
                    {
                        bool isNew = true;
                        for(int i = 0; i < colliderList.Count; i++){
                            if(colliderList[i] == other){
                                isNew = false;
                                break;
                            }
                        }
                        if(isNew){
                            colliderList.Add(other);
                            onEnter?.Invoke(other);
                        }
                        return;
                    }
                }
            }

            for(int i = 0; i < colliderList.Count; i++){
                if(colliderList[i] == other){
                    colliderList.Remove(other);
                    onExit?.Invoke(other);
                    return;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == tagName){
            for(int i = 0; i < colliderList.Count; i++){
                if(colliderList[i] == other){
                    colliderList.Remove(other);
                    onExit?.Invoke(other);
                    return;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // シーンビューでRayの軌跡を表示
        for(int i = 0; i < colliderList.Count; i++){
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, colliderList[i].transform.position - transform.position);
        }
    }
}
