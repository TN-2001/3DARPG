using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private int hp = 0; // hp
    [SerializeField] private int str = 0; // スタミナ
    [SerializeField] private int atk = 0; // 攻撃力

    public int Hp => hp;
    public int Str => str;
    public int Atk => atk;
}

[System.Serializable]
public class Player
{
    [SerializeField] private PlayerData data = null;
    public PlayerData Data => data;

    [SerializeField] private int lev = 1; // レベル
    [SerializeField] private int currentExp = 0; // 現在の経験値
    [SerializeField] private int currentHp = 0; // 現在のhp
    private float currentStr = 0; // 現在のスタミナ

    public int Lev => lev;
    public int Exp => lev * 100; // 必要経験値
    public int CurrentExp => currentExp;
    public int Hp // hp
    {
        get{
            int hp = data.Hp;
            foreach(Armor armor in ArmorList){
                if(armor != null) hp += armor.Hp;
            }
            return hp;
        }
    }
    public int CurrentHp => currentHp;
    public int Str // スタミナ
    {
        get{
            return data.Str;
        }
    }
    public float CurrentStr => currentStr;
    public int Atk // 攻撃力
    {
        get{
            int atk = data.Atk;
            return atk;
        }
    }
    public List<Weapon> WeaponList // 武器リスト
    {
        get{
            List<Weapon> weaponList = new(){null, null, null, null};
            weaponList[0] = DataManager.Instance.Data.WeaponList.Find(x => x.EquipNumber == 1);
            weaponList[1] = DataManager.Instance.Data.WeaponList.Find(x => x.EquipNumber == 2);
            weaponList[2] = DataManager.Instance.Data.WeaponList.Find(x => x.EquipNumber == 3);
            weaponList[3] = DataManager.Instance.Data.WeaponList.Find(x => x.EquipNumber == 4);
            return weaponList;
        }
    }
    public List<Armor> ArmorList // 防具リスト
    {
        get{
            List<Armor> armorList = new(){null, null, null, null};
            armorList[0] = DataManager.Instance.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Head);
            armorList[1] = DataManager.Instance.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Chest);
            armorList[2] = DataManager.Instance.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Arm);
            armorList[3] = DataManager.Instance.Data.ArmorList.Find(x => x.IsEquip & x.Data.ArmorType == ArmorType.Leg);
            return armorList;
        }
    }
    public float BarrierTime { get; private set; } = 0; // バリア残り時間
    public Transform Transform { get; set; }


    public Player(PlayerData data)
    {
        this.data = data;
        currentHp = Hp;
        currentStr = Str;
    }

    public void Init(PlayerData data)
    {
        this.data = data;
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
