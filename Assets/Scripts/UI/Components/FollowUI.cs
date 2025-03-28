using UnityEngine;

public class FollowUI : MonoBehaviour {
    // コンポーネント
    private RectTransform rectTransform = null;

    // ターゲット
    public Transform targetTransform = null; // 追従する対象
    public Vector3 targetPosition = Vector3.zero; // 追従する対象の位置

    // パラメータ
    [SerializeField] private MoveType moveType = MoveType.None;
    [SerializeField] private bool isRotate = false;
    [SerializeField] private bool isConstScale = false; // 固定スケール
    [SerializeField] private bool isDestroy = false; // 消すか否か
    [SerializeField] private float survivalTime = 0; // 生存時間

    private Vector3 initialLossyScale = new(1, 1, 1); // 最初のスケール
    private Vector3 currentLossyScale = new(1, 1, 1); // 現在のスケール


    private void Start() {
        rectTransform = GetComponent<RectTransform>();

        initialLossyScale = transform.lossyScale;
        currentLossyScale = initialLossyScale;

        if (isDestroy) {
            Destroy(gameObject, survivalTime);
        }
    }

    private void LateUpdate() {
        if (targetTransform) {
            targetPosition = targetTransform.position;

            // 回転
            if (isRotate) {
                Vector3 rot = rectTransform.rotation.eulerAngles;
                rot.z = -targetTransform.rotation.eulerAngles.y;
                rectTransform.rotation = Quaternion.Euler(rot);
            }
        }

        // 座標
        if (moveType == MoveType.WorldToScreen) {
            Vector3 pos = Camera.main.WorldToScreenPoint(targetPosition);
            pos.z = 0;
            float scale = Screen.width / Camera.main.pixelWidth;
            rectTransform.position = pos * scale;
        } else if (moveType == MoveType.WorldToUI) {
            rectTransform.anchoredPosition3D =
                new Vector3(targetTransform.position.x, targetTransform.position.z, 0);
        } else if (moveType == MoveType.MinusWorldToUI) {
            rectTransform.anchoredPosition3D =
                new Vector3(-targetTransform.position.x, -targetTransform.position.z, 0);
        }

        // 固定スケール
        if (isConstScale) {
            if (transform.lossyScale != currentLossyScale) {
                Vector3 scaleChange = initialLossyScale;
                currentLossyScale = transform.lossyScale;
                scaleChange.x /= currentLossyScale.x;
                scaleChange.y /= currentLossyScale.y;
                scaleChange.z /= currentLossyScale.z;
                transform.localScale = Vector3.Scale(transform.localScale, scaleChange);
            }
        }
    }


    // タイプ
    private enum MoveType {
        None,
        WorldToScreen,
        WorldToUI,
        MinusWorldToUI,
    }
}
