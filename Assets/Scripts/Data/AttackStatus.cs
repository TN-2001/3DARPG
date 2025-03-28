using UnityEngine;

[System.Serializable]
public class AttackStatus {
    private IBattlerStatus status = null; // ステータス

    [SerializeField] private float atkMultiplier = 1f; // 攻撃倍率
    [SerializeField] private bool isFly = false; // 吹っ飛ぶか

    public int Atk => (int)(status.Atk * atkMultiplier); // 攻撃力
    public bool IsFly => isFly; // 吹っ飛ぶか

    // 初期化
    public void Init(IBattlerStatus status) {
        this.status = status;
    }
}
