using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // ターゲット
    public Transform target = null;
    // UIトランスフォーム
    private RectTransform rectTra = null;
    // タイプ
    private enum Type
    {
        Normal,
        WorldToScreen,
        WorldToUI,
    }
    [SerializeField]
    private Type type = Type.Normal;
    [SerializeField] // 座標
    private bool posX, posY, posZ = false;
    [SerializeField] // 回転
    private bool rotX, rotY, rotZ = false;


    private void Start()
    {
        if(GetComponent<RectTransform>()){
            rectTra = GetComponent<RectTransform>();
        }
    }

    private void LateUpdate()
    {
        if(target){
            if(type == Type.Normal){
                Vector3 pos = transform.position;
                if(posX){
                    pos.x = target.position.x;
                }
                if(posY){
                    pos.y = target.position.y;
                }
                if(posZ){
                    pos.z = target.position.z;
                }
                transform.position = pos;

                Vector3 rot = transform.rotation.eulerAngles;
                if(rotX){
                    rot.x = target.rotation.eulerAngles.x;
                }
                if(rotY){
                    rot.y = target.rotation.eulerAngles.y;
                }
                if(rotZ){
                    rot.z = target.rotation.eulerAngles.z;
                }
                transform.rotation = Quaternion.Euler(rot);
            }
            else if(type == Type.WorldToScreen){
                Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
                if(!posX){
                    pos.x = 0;
                }
                if(!posY){
                    pos.y = 0;
                }
                if(!posZ){
                    pos.z = 0;
                }
                transform.position = pos;

                Vector3 rot = transform.rotation.eulerAngles;
                if(rotZ){
                    rot.z = -target.rotation.eulerAngles.y;
                }
                rectTra.rotation = Quaternion.Euler(rot);
            }
            else if(type == Type.WorldToUI){
                Vector3 pos = rectTra.anchoredPosition3D;
                if(posX){
                    pos.x = -target.position.x*rectTra.localScale.x;
                }
                if(posY){
                    pos.y = -target.position.z*rectTra.localScale.z;
                }
                rectTra.anchoredPosition3D = pos;

                Vector3 rot = transform.rotation.eulerAngles;
                if(rotZ){
                    rot.z = -target.rotation.eulerAngles.y;
                }
                rectTra.rotation = Quaternion.Euler(rot);
            }
        }
    }
}
