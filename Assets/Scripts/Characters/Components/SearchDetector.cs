using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SearchDetector : MonoBehaviour
{
    [Header("物理設定でレイがTriggerにはあたらないようにしよう")]
    [Header("高さは目線に合わせた方がいい")]
    public string tagName = "Player";
    [SerializeField] private float angle = 45f; // 視野角

    // 変数
    public List<Collider> AllColliderList { get; private set; } = new(); // コライダー内のコライダー
    public List<Collider> ColliderList { get; private set; } = new(); // 視野内のコライダー
    public Collider CloseCollider // 最も近いコライダー
    {
        get{
            Collider collider = null;
            for(int i = 0; i < ColliderList.Count; i++){
                if(collider){
                    float dis0 = Vector3.Distance(transform.position, collider.transform.position);
                    float dis1 = Vector3.Distance(transform.position, ColliderList[i].transform.position);
                    if(dis1 < dis0){
                        collider = ColliderList[i];
                    }
                }
                else{
                    collider = ColliderList[i];
                }
            }
            return collider;
        }
    }

    // UnityEvent
    [HideInInspector] public UnityEvent<Collider> onEnter;
    [HideInInspector] public UnityEvent<Collider> onExit;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(tagName)){
            AllColliderList.Add(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(tagName)) //視界の範囲内の当たり判定
        {
            //視界の角度内に収まっているか
            Vector3 posDelta = other.transform.position - transform.position;
            float target_angle = Vector3.Angle(transform.forward, posDelta);

            if (target_angle < angle) //target_angleがangleに収まっているかどうか
            {
                if(Physics.Raycast(transform.position, posDelta, out RaycastHit hit)) //Rayを使用してtargetに当たっているか判別
                {
                    if (hit.collider==other)
                    {
                        bool isNew = true;
                        for(int i = 0; i < ColliderList.Count; i++){
                            if(ColliderList[i] == other){
                                isNew = false;
                                break;
                            }
                        }
                        if(isNew){
                            ColliderList.Add(other);
                            onEnter?.Invoke(other);
                        }
                        return;
                    }
                }
            }

            for(int i = 0; i < ColliderList.Count; i++){
                if(ColliderList[i] == other){
                    ColliderList.Remove(other);
                    onExit?.Invoke(other);
                    return;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag(tagName)){
            AllColliderList.Remove(other);
            for(int i = 0; i < ColliderList.Count; i++){
                if(ColliderList[i] == other){
                    ColliderList.Remove(other);
                    onExit?.Invoke(other);
                    return;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // シーンビューでRayの軌跡を表示
        for(int i = 0; i < ColliderList.Count; i++){
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, ColliderList[i].transform.position - transform.position);
        }
    }
}
