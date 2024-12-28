using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private int number = 0; // 番号
    [SerializeField] private new string name = null; // 名前
    [SerializeField, TextArea] private string info = null; // 情報
    [SerializeField] private Sprite image = null; // イメージ
    [SerializeField] private int price = 0; // 価値
    [SerializeField] private ItemType itemType = ItemType.None; // アイテムのタイプ
    [SerializeField] private int value = 0; // 値
    [SerializeField] private GameObject prefab = null; // プレハブ

    public int Number => number;
    public string Name => name;
    public string Info => info;
    public Sprite Image => image;
    public int Price => price;
    public ItemType ItemType => itemType;
    public int Value => value;
    public GameObject Prefab => prefab;
}

[System.Serializable]
public class Item
{
    private ItemData data = null;
    public ItemData Data => data;

    [SerializeField] private int number = 0; // 番号
    [SerializeField] private int count = 0; // 数

    public int Number => number;
    public int Count => count;


    public Item(ItemData data, int count)
    {
        this.data = data;
        number = data.Number;
        this.count = count;
    }

    public void Init(ItemData data)
    {
        this.data = data;
    }

    public void UpadateCount(int count)
    {
        this.count += count;
    }
}

public enum ItemType
{
    None,
    Recovery,
}