using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // スタミナ
    private int str = 0;
    public int Str => str;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
}

[System.Serializable]
public class Player
{
    [SerializeField]
    private PlayerData data = null;
    public PlayerData Data => data;

    [SerializeField] // レベル
    private int lev = 1;
    public int Lev => lev;
    // 必要経験値
    public int Exp => lev * 100;
    [SerializeField] // 現在の経験値
    private int currentExp = 0;
    public int CurrentExp => currentExp;
    // hp
    public int Hp{get{
        int hp = data.Hp;
        foreach(Armor armor in ArmorList){
            if(armor != null) hp += armor.Hp;
        }
        return hp;
    }}
    [SerializeField] // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;
    // スタミナ
    public int Str{get{
        return data.Str;
    }}
    // 現在のスタミナ
    private float currentStr = 0;
    public float CurrentStr => currentStr;
    // 攻撃力
    public int Atk{get{
        int atk = data.Atk;
        return atk;
    }}
    // 武器リスト
    public List<Weapon> WeaponList {get{
        List<Weapon> weaponList = new(){null, null, null, null};
        weaponList[0] = GameManager.I.Data.WeaponList.Find(x => x.EquipNumber == 1);
        weaponList[1] = GameManager.I.Data.WeaponList.Find(x => x.EquipNumber == 2);
        weaponList[2] = GameManager.I.Data.WeaponList.Find(x => x.EquipNumber == 3);
        weaponList[3] = GameManager.I.Data.WeaponList.Find(x => x.EquipNumber == 4);
        return weaponList;
    }}
    // 防具リスト
    public List<Armor> ArmorList {get{
        List<Armor> armorList = new(){null, null, null, null};
        armorList[0] = GameManager.I.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Head);
        armorList[1] = GameManager.I.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Chest);
        armorList[2] = GameManager.I.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Arm);
        armorList[3] = GameManager.I.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Leg);
        return armorList;
    }}
    // バリア残り時間
    public float BarrierTime { get; private set;} = 0;


    public Player(PlayerData data)
    {
        this.data = data;
        currentHp = Hp;
        currentStr = Str;
    }

    public void Init(PlayerData data)
    {
        this.data = data;
        currentHp = Hp;
        currentStr = Str;
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

    public int UpdateHp(int para)
    {
        currentHp += para;

        if(currentHp < 0) currentHp = 0;
        else if(currentHp > Hp) currentHp = Hp;

        return para;
    }

    public void UpdateStr(float para)
    {
        currentStr += para;
        if(currentStr < 0) currentStr = 0;
        else if(currentStr > Str) currentStr = Str;
    }

    public void InitBarrierTime(float time)
    {
        BarrierTime = time;
    }
    public void UpdateBarrierTime(float deltaTime)
    {
        BarrierTime -= deltaTime;
        if(BarrierTime < 0){
            BarrierTime = 0;
        }
    }
}
