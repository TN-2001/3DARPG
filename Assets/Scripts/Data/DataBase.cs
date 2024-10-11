using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObject/DataBase")]
public class DataBase : ScriptableObject
{
    [SerializeField]
    private PlayerData playerData = null;
    public PlayerData PlayerData => playerData;
    [SerializeField]
    private List<EnemyData> enemyDataList = new List<EnemyData>();
    public List<EnemyData> EnemyDataList => enemyDataList;
    [SerializeField]
    private List<NpcData> npcDataList = new List<NpcData>();
    public List<NpcData> NpcDataList => npcDataList;
    [SerializeField]
    private List<ItemData> itemDataList = new List<ItemData>();
    public List<ItemData> ItemDataList => itemDataList;
    [SerializeField]
    private List<WeaponData> weaponDataList = new List<WeaponData>();
    public List<WeaponData> WeaponDataList => weaponDataList;
    [SerializeField]
    private List<ArmorData> armorData = new List<ArmorData>();
    public List<ArmorData> ArmorDataList => armorData;
    [SerializeField]
    private List<EventData> eventDataList = new List<EventData>();
    public List<EventData> EventDataList => eventDataList;
}

[System.Serializable]
public class SaveData
{
    [SerializeField]
    private int money = 1000;
    public int Money => money;
    [SerializeField]
    private Player player = null;
    public Player Player => player;
    [SerializeField]
    private List<Item> itemList = new List<Item>();
    public List<Item> ItemList => itemList;
    [SerializeField]
    private List<Armor> armorList = new List<Armor>();
    public List<Armor> ArmorList => armorList;
    [SerializeField]
    private List<Weapon> weaponList = new List<Weapon>();
    public List<Weapon> WeaponList => weaponList;
    [SerializeField] // モンスターを発見したか
    private List<bool> isFindEnemyList = new List<bool>();
    public List<bool> IsFindEnemyList => isFindEnemyList;
    [SerializeField] // NPCを発見したか
    private List<bool> isFindNpcList = new List<bool>();
    public List<bool> IsFindNpcList => isFindNpcList;
    [SerializeField] // 現在のイベント番号
    private int eventNumber = 0;
    public int EventNumber => eventNumber;
    [SerializeField] // 音量
    private List<int> volumeList = new List<int>(){80, 80};
    public List<int> VolumeList => volumeList;

    [HideInInspector]
    public UnityEvent onChageWeapon = null;


    public SaveData(DataBase data)
    {
        UpdateItem(data.ItemDataList[0], 5);
        AddWeapon(new Weapon(data.WeaponDataList[0]));
        EquipWeapon(weaponList[0], 1);
        while(isFindEnemyList.Count < data.EnemyDataList.Count){
            isFindEnemyList.Add(false);
        }
        while(isFindNpcList.Count < data.NpcDataList.Count){
            isFindNpcList.Add(false);
        }
        player = new Player(data.PlayerData);
    }

    public void Init(DataBase data)
    {
        for(int i = 0; i < itemList.Count; i++){
            itemList[i].Init(data.ItemDataList.Find(x => x.Number == itemList[i].Number));
        }
        for(int i = 0; i < armorList.Count; i++){
            armorList[i].Init(data.ArmorDataList.Find(x => x.Number == armorList[i].Number));
        }
        for(int i = 0; i < weaponList.Count; i++){
            weaponList[i].Init(data.WeaponDataList.Find(x => x.Number == weaponList[i].Number));
        }
        while(isFindEnemyList.Count < data.EnemyDataList.Count){
            isFindEnemyList.Add(false);
        }
        while(isFindNpcList.Count < data.NpcDataList.Count){
            isFindNpcList.Add(false);
        }
        if(volumeList.Count < 2){
            volumeList = new List<int>(){80, 80};
        }
        player.Init(data.PlayerData);
    }

    public void UpdateMoney(int money)
    {
        this.money += money;
    }

    public void UpdateItem(ItemData itemData, int count)
    {
        bool isFind = false;
        for(int i = 0; i < itemList.Count; i++){
            if(itemList[i].Data == itemData){
                itemList[i].UpadateCount(count);
                if(itemList[i].Count == 0) itemList.Remove(itemList[i]);
                isFind = true;
                break;
            }
        }

        if(!isFind) itemList.Add(new Item(itemData, count));

        itemList.Sort((a,b) => a.Number - b.Number);
    }

    // 武器
    public void AddWeapon(Weapon weapon)
    {
        weaponList.Add(weapon);
    }
    public void RemoveWeapon(Weapon weapon)
    {
        weaponList.Remove(weapon);
    }
    public void EquipWeapon(Weapon weapon, int number)
    {
        if(number > 0){
            weaponList.Find(x => x.EquipNumber == number)?.UpdateIsEquip(0);
        }
        weaponList.Find(x => x == weapon).UpdateIsEquip(number);

        onChageWeapon?.Invoke();
    }

    // 防具
    public void AddArmor(Armor armor)
    {
        armorList.Add(armor);
    }
    public void RemoveArmor(Armor armor)
    {
        armorList.Remove(armor);
    }
    public void EquipArmor(Armor armor, bool isEquip)
    {
        if(isEquip){
            armorList.Find(x => x.IsEquip & x.Data.ArmorType == armor.Data.ArmorType & x != armor)?.UpdateIsEquip(false);
        }
        armorList.Find(x => x == armor).UpdateIsEquip(isEquip);
    }

    public void UpdateFindEnemy(int number)
    {
        isFindEnemyList[number] = true;
    }

    public void UpdateFindNpc(int number)
    {
        isFindNpcList[number] = true;
    }

    public void UpdateEventNumber()
    {
        eventNumber += 1;
    }

    public void UpdateVolume(int volume, int number)
    {
        volumeList[number] = volume;
    }
}
