using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Foot : MonoBehaviour
{
    private TwoBoneIKConstraint footIK = null; // IK

    [SerializeField] private Animator animator = null; // アニメーター
    [SerializeField] private string weightName = ""; // アニメーター上の値の名前 
    [SerializeField] private LayerMask groundLayerMask = 0; // レイヤーマスク
    [SerializeField] private float floatingHeight = 0.1f; // 足の浮く高さ

    private Vector3 position = Vector3.zero; // IKの座標
    private Quaternion rotation = Quaternion.identity; // IKの向き
    private Quaternion initRotation = Quaternion.identity; // IKの向き
    private Quaternion initLocalRotation = Quaternion.identity; 

    private void Start()
    {
        footIK = GetComponent<TwoBoneIKConstraint>();

        // 初期位置と回転を取得
        position = transform.position;
        rotation = transform.rotation;
        initRotation = rotation;
        initLocalRotation = transform.localRotation;

        UpdateFoot();
    }

    private void Update()
    {
        footIK.weight = animator.GetFloat(weightName);

        if (footIK.data.tip.position != position && footIK.weight < 0.9) {
            UpdateFoot();
        }

        // IKの位置と回転を更新
        transform.position = position;
        transform.rotation = rotation;
    }

    // 足の位置と向きを地面に合わせる
    private void UpdateFoot()
    {
        // レイキャストで地面を検出
        if (Physics.Raycast(footIK.data.tip.position + Vector3.up, Vector3.down, out RaycastHit hit, 2f, groundLayerMask)) {
            // 足が少し浮くようにオフセット
            position = new Vector3(footIK.data.tip.position.x, hit.point.y + floatingHeight, footIK.data.tip.position.z);

            // 通常回転
            rotation = Quaternion.Euler(initRotation.eulerAngles.x, 
                initLocalRotation.eulerAngles.y+transform.parent.eulerAngles.y, initRotation.eulerAngles.z);
            // 現在の回転に、地面との角度を適用
            rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * rotation;
        }
    }
}
