using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    [SerializeField] // 追従するか否か
    private bool isFollow = false;
    [SerializeField] // 消すか否か
    private bool isDestroy = false;
    [SerializeField] // 生存時間
    private float survivalTime = 0;
    [SerializeField] // 追従する対象
    private Transform target = null;
    // 追従する対象の最初の位置
    private Vector3 firstPosition = Vector3.zero;


    public void Init(Transform target)
    {
        this.target = target;
        firstPosition = target.position;
        Vector3 pos = Camera.main.WorldToScreenPoint(firstPosition);
        pos.z = 0;
        transform.position = pos*2;
        gameObject.SetActive(true);

        if(isDestroy) Destroy(gameObject, survivalTime);
    }
    public void Init(Vector3 position)
    {
        firstPosition = position;
        Vector3 pos = Camera.main.WorldToScreenPoint(firstPosition);
        pos.z = 0;
        transform.position = pos*2;
        gameObject.SetActive(true);

        if(isDestroy) Destroy(gameObject, survivalTime);
    }

    private void LateUpdate()
    {
        if(isFollow){
            if(target){
                Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
                pos.z = 0;
                transform.position = pos*2;
            }
            else{
                Destroy(gameObject);
            }
        }
        else{
            Vector3 pos = Camera.main.WorldToScreenPoint(firstPosition);
            pos.z = 0;
            transform.position = pos*2;
        }
    }
}
