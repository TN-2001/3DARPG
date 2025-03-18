using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private int number = 0; // 番号
    [SerializeField] private new string name = null; // 名前
    [SerializeField, TextArea] private string info = null; // 情報
    [SerializeField] private Sprite image = null; // イメージ
    [SerializeField] private int price = 0; // 価値
    [SerializeField] private List<ItemData> itemDataList = new(); // 作製に必要なアイテム
    [SerializeField] private WeaponType weaponType = WeaponType.Sword; // 武器のタイプ
    [SerializeField] private int atk = 0; // 攻撃力
    [SerializeField] private int recovery = 0; // 回復
    [SerializeField] private int guardTime = 0; // ガード時間
    [SerializeField] private GameObject leftObject = null; // 左手のオブジェクト
    [SerializeField] private GameObject rightObject = null; // 右手のオブジェクト

    public int Number => number;
    public string Name => name;
    public string Info => info;
    public Sprite Image => image; 
    public int Price => price;
    public List<ItemData> ItemDataList => itemDataList;
    public WeaponType WeaponType => weaponType;
    public int Atk => atk;
    public int Recovery => recovery;
    public int GuardTime => guardTime;
    public GameObject LeftObject => leftObject;
    public GameObject RightObject => rightObject;
}

[System.Serializable]
public class Weapon
{
    [SerializeField] private WeaponData data = null;
    public WeaponData Data => data;

    [SerializeField] private int number = 0; // 番号
    [SerializeField] private int lev = 1; // レベル
    [SerializeField] private int currentExp = 0; // 現在の経験値
    [SerializeField] private int equipNumber = 0; // 装備しているか

    public int Number => number;
    public int Lev => lev;
    public int Exp => lev * 100; // 必要経験値
    public int CurrentExp => currentExp;
    public int Atk => data.Atk * lev; // 攻撃力
    public int Recovery => data.Recovery; // 回復
    public int GuardTime => data.GuardTime; // ガード時間
    public int EquipNumber => equipNumber;


    public Weapon(WeaponData data)
    {
        this.data = data;
        if(data) number = data.Number;
    }

    public void Init(WeaponData data)
    {
        this.data = data;
    }

    public void UpdateExp(int exp)
    {
        currentExp += exp;

        while(currentExp >= Exp)
        {
            currentExp -= Exp;
            lev ++;
        }
    }

    public void UpdateIsEquip(int number)
    {
        equipNumber = number;
    }
}

public enum WeaponType
{
    Sword,
    Wand,
    Bow,
    Shield,
    SwordAndShield
}
