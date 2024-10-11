using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageDetector : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<int , Vector3> onDamage;

    public void OnDamage(int damage, Vector3 position)
    {
        onDamage?.Invoke(damage, position);
    }
}
