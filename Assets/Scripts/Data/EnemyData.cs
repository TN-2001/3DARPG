using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
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
    [SerializeField] // hp
    private int hp = 0;
    public int Hp => hp;
    [SerializeField] // 攻撃力
    private int atk = 0;
    public int Atk => atk;
    [SerializeField] // ドロップアイテム
    private List<DropItem> dropItemList = null;
    public List<DropItem> DropItemList => dropItemList;
    [SerializeField] // ドロップ防具
    private List<DropArmor> dropArmorList = null;
    public List<DropArmor> DropArmorList => dropArmorList;
    [SerializeField] // ドロップ武器
    private List<DropWeapon> dropWeaponList = null;
    public List<DropWeapon> DropWeaponList => dropWeaponList;

    // ドロップアイテム
    [System.Serializable]
    public class DropItem
    {
        [SerializeField] // アイテム
        private ItemData itemData = null;
        public ItemData ItemData => itemData;
        [SerializeField] // 生成パーセント
        private int parsent = 0;
        public int Parsent => parsent;
    }
    [System.Serializable]
    public class DropArmor
    {
        [SerializeField] // アイテム
        private ArmorData armorData = null;
        public ArmorData ArmorData => armorData;
        [SerializeField] // 生成パーセント
        private int parsent = 0;
        public int Parsent => parsent;
    }
    [System.Serializable]
    public class DropWeapon
    {
        [SerializeField] // アイテム
        private WeaponData weaponData = null;
        public WeaponData WeaponData => weaponData;
        [SerializeField] // 生成パーセント
        private int parsent = 0;
        public int Parsent => parsent;
    }
}

[System.Serializable]
public class Enemy
{
    [SerializeField]
    private EnemyData data = null;
    public EnemyData Data => data;

    // hp
    public int Hp => data.Hp;
    // 攻撃力
    public int Atk => data.Atk;
    [SerializeField] // 現在のhp
    private int currentHp = 0;
    public int CurrentHp => currentHp;
    // ドロップアイテム
    private List<ItemData> dropItemList = new List<ItemData>();
    public List<ItemData> DropItemList => dropItemList;
    // ドロップ防具
    private List<ArmorData> dropArmorList = new List<ArmorData>();
    public List<ArmorData> DropArmorList => dropArmorList;
    // ドロップ武器
    private List<WeaponData> dropWeaponList = new List<WeaponData>();
    public List<WeaponData> DropWeaponList => dropWeaponList;


    public Enemy(EnemyData data)
    {
        this.data = data;
        currentHp = Hp;

        dropItemList = new List<ItemData>();
        for(int i = 0; i < data.DropItemList.Count; i++){
            int parsent = Random.Range(1, 101);
            if(parsent < data.DropItemList[i].Parsent){
                dropItemList.Add(data.DropItemList[i].ItemData);
            }
        }
        dropArmorList = new List<ArmorData>();
        for(int i = 0; i < data.DropArmorList.Count; i++){
            int parsent = Random.Range(1, 101);
            if(parsent < data.DropArmorList[i].Parsent){
                dropArmorList.Add(data.DropArmorList[i].ArmorData);
            }
        }
        dropWeaponList = new List<WeaponData>();
        for(int i = 0; i < data.DropWeaponList.Count; i++){
            int parsent = Random.Range(1, 101);
            if(parsent < data.DropWeaponList[i].Parsent){
                dropWeaponList.Add(data.DropWeaponList[i].WeaponData);
            }
        }
    }

    public int UpdateHp(int para)
    {
        currentHp += para;

        if(currentHp < 0) currentHp = 0;
        else if(currentHp > Hp) currentHp = Hp;

        return para;
    }
}