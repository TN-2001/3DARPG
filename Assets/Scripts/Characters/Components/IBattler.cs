using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattler
{
    public void OnDamage(int damage, Vector3 position);
}
