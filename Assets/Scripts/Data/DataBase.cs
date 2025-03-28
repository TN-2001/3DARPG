using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObject/DataBase")]
public class DataBase : ScriptableObject {
    [SerializeField] private PlayerData playerData = null;
    [SerializeField] private List<EnemyData> enemyDataList = new();
    [SerializeField] private List<NpcData> npcDataList = new();
    [SerializeField] private List<ItemData> itemDataList = new();
    [SerializeField] private List<WeaponData> weaponDataList = new();
    [SerializeField] private List<ArmorData> armorData = new();
    [SerializeField] private List<EventData> eventDataList = new();

    public PlayerData PlayerData => playerData;
    public List<EnemyData> EnemyDataList => enemyDataList;
    public List<NpcData> NpcDataList => npcDataList;
    public List<ItemData> ItemDataList => itemDataList;
    public List<WeaponData> WeaponDataList => weaponDataList;
    public List<ArmorData> ArmorDataList => armorData;
    public List<EventData> EventDataList => eventDataList;
}

[System.Serializable]
public class SaveData {
    // データ
    [SerializeField] private int money = 1000;
    [SerializeField] private Player player = null;
    [SerializeField] private List<Item> itemList = new();
    [SerializeField] private List<Armor> armorList = new();
    [SerializeField] private List<Weapon> weaponList = new();
    [SerializeField] private List<bool> isFindEnemyList = new(); // モンスターを発見したか
    [SerializeField] private List<bool> isFindNpcList = new(); // NPCを発見したか
    [SerializeField] private int eventNumber = 0; // 現在のイベント番号
    [SerializeField] private List<int> volumeList = new() { 80, 80 }; // 音量

    public int Money => money;
    public Player Player => player;
    public List<Item> ItemList => itemList;
    public List<Armor> ArmorList => armorList;
    public List<Weapon> WeaponList => weaponList;
    public List<bool> IsFindEnemyList => isFindEnemyList;
    public List<bool> IsFindNpcList => isFindNpcList;
    public int EventNumber => eventNumber;
    public List<int> VolumeList => volumeList;

    // イベント
    [HideInInspector] public UnityEvent<Weapon> onChageWeapon = null;


    public SaveData(DataBase data) // 生成
    {
        UpdateItem(data.ItemDataList[0], 5);
        AddWeapon(new Weapon(data.WeaponDataList[0]));
        EquipWeapon(weaponList[0], 1);
        while (isFindEnemyList.Count < data.EnemyDataList.Count) {
            isFindEnemyList.Add(false);
        }
        while (isFindNpcList.Count < data.NpcDataList.Count) {
            isFindNpcList.Add(false);
        }
        player = new Player(data.PlayerData);
    }

    public void Init(DataBase data) // 初期化
    {
        for (int i = 0; i < itemList.Count; i++) {
            itemList[i].Init(data.ItemDataList.Find(x => x.Number == itemList[i].Number));
        }
        for (int i = 0; i < armorList.Count; i++) {
            armorList[i].Init(data.ArmorDataList.Find(x => x.Number == armorList[i].Number));
        }
        for (int i = 0; i < weaponList.Count; i++) {
            weaponList[i].Init(data.WeaponDataList.Find(x => x.Number == weaponList[i].Number));
        }
        while (isFindEnemyList.Count < data.EnemyDataList.Count) {
            isFindEnemyList.Add(false);
        }
        while (isFindNpcList.Count < data.NpcDataList.Count) {
            isFindNpcList.Add(false);
        }
        if (volumeList.Count < 2) {
            volumeList = new List<int>() { 80, 80 };
        }
        player.Init(data.PlayerData);
    }

    public void UpdateMoney(int money) {
        this.money += money;
    }

    public void UpdateItem(ItemData itemData, int count) {
        bool isFind = false;
        for (int i = 0; i < itemList.Count; i++) {
            if (itemList[i].Data == itemData) {
                itemList[i].UpadateCount(count);
                if (itemList[i].Count == 0) itemList.Remove(itemList[i]);
                isFind = true;
                break;
            }
        }

        if (!isFind) itemList.Add(new Item(itemData, count));

        itemList.Sort((a, b) => a.Number - b.Number);
    }

    // 武器
    public void AddWeapon(Weapon weapon) {
        weaponList.Add(weapon);
    }
    public void RemoveWeapon(Weapon weapon) {
        weaponList.Remove(weapon);
    }
    public void EquipWeapon(Weapon weapon, int number) {
        if (number > 0) {
            weaponList.Find(x => x.EquipNumber == number)?.UpdateIsEquip(0);
        }
        weaponList.Find(x => x == weapon).UpdateIsEquip(number);

        onChageWeapon?.Invoke(weapon);
    }

    // 防具
    public void AddArmor(Armor armor) {
        armorList.Add(armor);
    }
    public void RemoveArmor(Armor armor) {
        armorList.Remove(armor);
    }
    public void EquipArmor(Armor armor, bool isEquip) {
        if (isEquip) {
            armorList.Find(x => x.IsEquip & x.Data.ArmorType == armor.Data.ArmorType & x != armor)?.UpdateIsEquip(false);
        }
        armorList.Find(x => x == armor).UpdateIsEquip(isEquip);
    }

    public void UpdateFindEnemy(int number) {
        isFindEnemyList[number] = true;
    }

    public void UpdateFindNpc(int number) {
        isFindNpcList[number] = true;
    }

    public void UpdateEventNumber() {
        eventNumber += 1;
    }

    public void UpdateVolume(int volume, int number) {
        volumeList[number] = volume;
    }
}
