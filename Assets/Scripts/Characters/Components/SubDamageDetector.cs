using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDamageDetector : MonoBehaviour
{
    [SerializeField] // ダメージディテクター
    private DamageDetector damageDetector = null;
    public DamageDetector DamageDetector => damageDetector;
}
