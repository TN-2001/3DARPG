using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Header("基本パラメータ")]
    [SerializeField] // 当たり判定の対象のタグ
    private string tagName = null;
    [SerializeField] // 攻撃％
    private int atkPercent = 100;
    [SerializeField] // 最初射程距離
    private float minRange = 0;
    public float MinRange => minRange;
    [SerializeField] // 最大射程距離
    private float maxRange = 1;
    public float MaxRange => maxRange;

    [Header("遠隔攻撃パラメータ")]
    [SerializeField] // 投げる攻撃
    private bool isThrow = false;
    public bool IsThrow => isThrow;
    [SerializeField] // 移動速度
    private float speed = 0;
    [SerializeField] // 生存時間
    private float survivalTime = 0;

    // 攻撃力
    private int atk = 0;
    // 経過時間
    private float countTime = 0;
    // 当てた敵
    private List<DamageDetector> damageList = new List<DamageDetector>();


    public void Initialize(int atk)
    {
        this.atk = atk * atkPercent / 100;
        countTime = 0;
        damageList = new List<DamageDetector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == tagName){
            // 1匹の敵に1度だけヒットするように

            DamageDetector damage = null;
            if(other.TryGetComponent<DamageDetector>(out DamageDetector _damage)){
                damage = _damage;
            }
            else if(other.TryGetComponent<SubDamageDetector>(out SubDamageDetector _subDamage)){
                damage = _subDamage.DamageDetector;
            }

            if(damage != null){
                bool isHit = false;
                foreach(DamageDetector target in damageList){
                    if(target == damage) isHit = true;
                }
                if(!isHit){
                    damage.OnDamage(atk, other.ClosestPointOnBounds(transform.position));
                    damageList.Add(damage);
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
            transform.position += transform.up * speed * Time.fixedDeltaTime;
            countTime += Time.fixedDeltaTime;
        }
    }
}
