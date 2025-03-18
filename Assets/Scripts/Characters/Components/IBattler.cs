using UnityEngine;

public interface IBattler
{
    // 攻撃、ヒット座標
    public void OnDamage(AttackStatus attack, Vector3 position);
}
