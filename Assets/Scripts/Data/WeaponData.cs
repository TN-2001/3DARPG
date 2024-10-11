using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/WeaponData")]
public class WeaponData : ScriptableObject
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
    [SerializeField] // 作製に必要なアイテム
    private List<ItemData> itemDataList = new List<ItemData>();
    public List<ItemData> ItemDataList => itemDataList;
    [SerializeField] // 武器のタイプ
    private WeaponType weaponType = WeaponType.Sword;
    public WeaponType WeaponType => weaponType;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
    [SerializeField] // 回復
    private int recovery = 0;
    public int Recovery => recovery;
    [SerializeField] // ガード時間
    private int guardTime = 0;
    public int GuardTime => guardTime;
}

[System.Serializable]
public class Weapon
{
    [SerializeField]
    private WeaponData data = null;
    public WeaponData Data => data;

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
    // 攻撃力
    public int Atk => data.Atk * lev;
    // 回復
    public int Recovery => data.Recovery;
    // ガード時間
    public int GuardTime => data.GuardTime;
    [SerializeField] // 装備しているか
    private int equipNumber = 0;
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
}
