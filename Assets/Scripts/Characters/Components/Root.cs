using UnityEngine;

public class Root : MonoBehaviour {
    [SerializeField] private LayerMask groundLayerMask = 0; // レイヤーマスク

    private Vector3 position = Vector3.zero; // 座標
    private Quaternion rotation = Quaternion.identity; // 向き 


    private void Start() {
        UpdateRoot();
        position = transform.position;
    }

    private void Update() {
        if (transform.position != position || transform.rotation != rotation) {
            UpdateRoot();
            position = transform.position;
        }

        // 滑らかに向きを更新
        if (Quaternion.Angle(transform.rotation, rotation) > 0.1f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3f);
        } else if (transform.rotation != rotation) {
            transform.rotation = rotation;
            rotation = transform.rotation;
        }
    }

    private void UpdateRoot() {
        // レイキャストで地面を検出
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2f, groundLayerMask)) {
            // 通常回転
            rotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);
            // 現在の回転に、地面との角度を適用
            rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * rotation;
        }
    }
}
