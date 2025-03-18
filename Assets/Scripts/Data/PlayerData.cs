using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerData")]
public class PlayerData : ScriptableObject, IBattlerStatusData
{
    [SerializeField] private int hp = 0; // hp
    [SerializeField] private int atk = 0; // 攻撃力
    [SerializeField] private int str = 0; // スタミナ

    public int Hp => hp;
    public int Atk => atk;
    public int Str => str;
}

[System.Serializable]
public class Player : IBattlerStatus
{
    [SerializeField] private PlayerData data = null;
    public PlayerData Data => data;

    [SerializeField] private int currentExp = 0; // 現在の経験値
    [SerializeField] private int currentHp = 0; // 現在のhp
    private float currentStr = 0; // 現在のスタミナ

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
    public int Atk // 攻撃力
    {
        get{
            int atk = data.Atk;
            return atk;
        }
    }
    public int Str // スタミナ
    {
        get{
            return data.Str;
        }
    }
    public float CurrentStr => currentStr;
    public Weapon Weapon => DataManager.Instance.Data.WeaponList.Find(x => x.EquipNumber == 1);
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

    public int UpdateHp(int damage)
    {
        currentHp += damage;

        if(currentHp < 0) currentHp = 0;
        else if(currentHp > Hp) currentHp = Hp;

        return damage;
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
