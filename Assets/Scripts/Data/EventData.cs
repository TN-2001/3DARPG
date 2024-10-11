using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EventData")]
public class EventData : ScriptableObject
{
    [SerializeField] // 番号
    private int number = 0;
    public int Number => number;
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
    [SerializeField, TextArea] // 情報
    private string info = null;
    public string Info => info;
}
