using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    // コンポーネント
    private Collider collision = null;

    [Header("基本パラメータ")]
    [SerializeField] private string tagName = null; // 当たり判定の対象のタグ
    [SerializeField] private float minRange = 0; // 最初射程距離
    public float MinRange => minRange;
    [SerializeField] private float maxRange = 1; // 最大射程距離
    public float MaxRange => maxRange;

    [Header("遠隔攻撃パラメータ")]
    [SerializeField] private bool isThrow = false; // 投げる攻撃
    public bool IsThrow => isThrow;
    [SerializeField] private float speed = 0; // 移動速度
    [SerializeField] private float survivalTime = 0; // 生存時間

    private AttackStatus attack = null;
    private float countTime = 0; // 経過時間
    private List<IBattler> iBattlerList = new(); // 当てた敵


    public void Initialize(AttackStatus attack)
    {
        this.attack = attack;
        countTime = 0;
    }

    public void OnCollisionEnable()
    {
        iBattlerList = new();

        if(collision == null){
            if(TryGetComponent(out Collider _collider)){
                collision = _collider;
            }
        }

        if(collision != null){
            collision.enabled = true;
        }
    }

    public void OnCollisionDisable()
    {
        if(collision != null){
            collision.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(tagName))
        {
            // 1匹の敵に1度だけヒットするように

            IBattler iBattler = null;
            if(other.TryGetComponent(out IBattler _iBattler)){
                iBattler = _iBattler;
            }
            else if(other.TryGetComponent(out SubIBattler _subIBattler)){
                iBattler = _subIBattler.IBattler;
            }

            if(iBattler != null){
                bool isHit = false;
                foreach(IBattler target in iBattlerList){
                    if(target == iBattler) isHit = true;
                }
                if(!isHit){
                    iBattler.OnDamage(attack, other.ClosestPointOnBounds(transform.position));
                    iBattlerList.Add(iBattler);
                }
            }
        }

        // 障害物に当たったら消す
        if(isThrow) {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(isThrow){
            if(countTime > survivalTime) Destroy(gameObject);
            transform.position += speed * Time.fixedDeltaTime * transform.up;
            countTime += Time.fixedDeltaTime;
        }
    }
}
