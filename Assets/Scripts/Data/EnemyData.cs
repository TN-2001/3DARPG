using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject, IBattlerStatusData {
    [SerializeField] private int number = 0; // 番号
    [SerializeField] private new string name = null; // 名前
    [SerializeField, TextArea] private string info = null; // 情報
    [SerializeField] private Sprite image = null; // イメージ
    [SerializeField] private int hp = 0; // hp
    [SerializeField] private int atk = 0; // 攻撃力
    [SerializeField] private List<DropItem> dropItemList = null; // ドロップアイテム
    [SerializeField] private List<DropArmor> dropArmorList = null; // ドロップ防具
    [SerializeField] private List<DropWeapon> dropWeaponList = null; // ドロップ武器
    [SerializeField] private GameObject prefab = null; // プレハブ

    public int Number => number;
    public string Name => name;
    public string Info => info;
    public Sprite Image => image;
    public int Hp => hp;
    public int Atk => atk;
    public List<DropItem> DropItemList => dropItemList;
    public List<DropArmor> DropArmorList => dropArmorList;
    public List<DropWeapon> DropWeaponList => dropWeaponList;
    public GameObject Prefab => prefab;

    // ドロップアイテム
    [System.Serializable]
    public class DropItem {
        [SerializeField] private ItemData itemData = null; // アイテム
        [SerializeField] private int parsent = 0; // 生成パーセント

        public ItemData ItemData => itemData;
        public int Parsent => parsent;
    }
    [System.Serializable]
    public class DropArmor {
        [SerializeField] private ArmorData armorData = null; // アイテム
        [SerializeField] private int parsent = 0; // 生成パーセント

        public ArmorData ArmorData => armorData;
        public int Parsent => parsent;
    }
    [System.Serializable]
    public class DropWeapon {
        [SerializeField] private WeaponData weaponData = null; // アイテム
        [SerializeField] private int parsent = 0; // 生成パーセント

        public WeaponData WeaponData => weaponData;
        public int Parsent => parsent;
    }
}

[System.Serializable]
public class Enemy : IBattlerStatus {
    [SerializeField] private EnemyData data = null;
    public EnemyData Data => data;

    [SerializeField] private int currentHp = 0; // 現在のhp
    private readonly List<ItemData> dropItemList = new(); // ドロップアイテム
    private readonly List<ArmorData> dropArmorList = new(); // ドロップ防具
    private readonly List<WeaponData> dropWeaponList = new(); // ドロップ武器

    public int Hp => data.Hp; // hp
    public int CurrentHp => currentHp;
    public int Atk => data.Atk; // 攻撃力
    public List<ItemData> DropItemList => dropItemList;
    public List<ArmorData> DropArmorList => dropArmorList;
    public List<WeaponData> DropWeaponList => dropWeaponList;
    public Transform Transform { get; set; }


    public Enemy(EnemyData data) {
        this.data = data;
        currentHp = Hp;

        dropItemList = new List<ItemData>();
        for (int i = 0; i < data.DropItemList.Count; i++) {
            int parsent = Random.Range(1, 101);
            if (parsent < data.DropItemList[i].Parsent) {
                dropItemList.Add(data.DropItemList[i].ItemData);
            }
        }
        dropArmorList = new List<ArmorData>();
        for (int i = 0; i < data.DropArmorList.Count; i++) {
            int parsent = Random.Range(1, 101);
            if (parsent < data.DropArmorList[i].Parsent) {
                dropArmorList.Add(data.DropArmorList[i].ArmorData);
            }
        }
        dropWeaponList = new List<WeaponData>();
        for (int i = 0; i < data.DropWeaponList.Count; i++) {
            int parsent = Random.Range(1, 101);
            if (parsent < data.DropWeaponList[i].Parsent) {
                dropWeaponList.Add(data.DropWeaponList[i].WeaponData);
            }
        }
    }

    public int UpdateHp(int damage) {
        currentHp += damage;

        if (currentHp < 0) currentHp = 0;
        else if (currentHp > Hp) currentHp = Hp;

        return damage;
    }
}