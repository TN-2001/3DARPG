using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ArmorData")]
public class ArmorData : ScriptableObject
{
    [SerializeField] private int number = 0; // 番号
    [SerializeField] private new string name = null; // 名前
    [SerializeField, TextArea] private string info = null; // 情報
    [SerializeField] private Sprite image = null; // イメージ
    [SerializeField] private int price = 0; // 価値
    [SerializeField] private ArmorType armorType = ArmorType.Head; // 防具のタイプ
    [SerializeField] private int hp = 0; // hp
    [SerializeField] private GameObject prefab = null; // プレハブ

    public int Number => number;
    public string Name => name;
    public string Info => info;
    public Sprite Image => image;
    public int Price => price;
    public ArmorType ArmorType => armorType;
    public int Hp => hp;
    public GameObject Prefab => prefab;
}

[System.Serializable]
public class Armor
{
    [SerializeField] private ArmorData data = null;
    public ArmorData Data => data;

    // パラメータ
    [SerializeField] private int number = 0; // 番号
    [SerializeField] private int lev = 1; // レベル
    [SerializeField] private int currentExp = 0; // 現在の経験値
    [SerializeField] private bool isEquip = false; // 装備しているか

    public int Number => number;
    public int Lev => lev;
    public int Exp => lev * 100; // 必要経験値
    public int CurrentExp => currentExp;
    public int Hp => data.Hp * lev; // hp
    public bool IsEquip => isEquip;


    public Armor(ArmorData data) // 生成
    {
        this.data = data;
        if(data) number = data.Number;
    }

    public void Init(ArmorData data) // 初期化
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

public enum ArmorType // 防具タイプ
{
    Head,
    Chest,
    Arm,
    Leg,
}
