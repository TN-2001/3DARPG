using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubIBattler : MonoBehaviour
{
    // ダメージディテクター
    [SerializeField, SubclassSelector(typeof(IBattler))]
    private MonoBehaviour iBattler = null;
    public IBattler IBattler => iBattler as IBattler;
}
