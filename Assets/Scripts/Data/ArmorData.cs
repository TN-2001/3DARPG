using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ArmorData")]
public class ArmorData : ScriptableObject
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
    [SerializeField] // イメージ
    private Sprite image = null;
    public Sprite Image => image;
    [SerializeField] // プレハブ
    private GameObject prefab = null;
    public GameObject Prefab => prefab;
    [SerializeField] // 価値
    private int price = 0;
    public int Price => price;
    [SerializeField] // 防具のタイプ
    private ArmorType armorType = ArmorType.Head;
    public ArmorType ArmorType => armorType;
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
}

[System.Serializable]
public class Armor
{
    [SerializeField]
    private ArmorData data = null;
    public ArmorData Data => data;

    [SerializeField] // 番号
    private int number = 0;
    public int Number => number;
    [SerializeField] // レベル
    private int lev = 1;
    public int Lev => lev;
    // 必要経験値
    public int Exp => lev * 100;
    [SerializeField] // 現在の経験値
    private int currentExp = 0;
    public int CurrentExp => currentExp;
    // hp
    public int Hp => data.Hp * lev;
    [SerializeField] // 装備しているか
    private bool isEquip = false;
    public bool IsEquip => isEquip;


    public Armor(ArmorData data)
    {
        this.data = data;
        if(data) this.number = data.Number;
    }

    public void Init(ArmorData data)
    {
        this.data = data;
    }

    public void UpdateExp(int exp)
    {
        currentExp += exp;

        while(currentExp >= Exp)
        {
            currentExp = currentExp - Exp;
            lev ++;
        }
    }

    public void UpdateIsEquip(bool isEquip)
    {
        this.isEquip = isEquip;
    }
}

public enum ArmorType
{
    Head,
    Chest,
    Arm,
    Leg,
}
