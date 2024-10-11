using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [SerializeField] // NPCデータ
    private NpcData npcData = null;
    public NpcData NpcData => npcData;
}
