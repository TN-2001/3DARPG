using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/NpcData")]
public class NpcData : ScriptableObject
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
    [SerializeField] // テキストデータ
    private List<TextData> textDataList = new List<TextData>();
    public List<TextData> TextDataList => textDataList;
}

[System.Serializable]
public class TextData
{
    [SerializeField] // 対称イベント
    private EventData eventData = null;
    public EventData EventData => eventData;
    [SerializeField, TextArea] // 会話文
    private List<string> textList = new List<string>();
    public List<string> TextList => textList;
    [SerializeField] // 座標
    private Vector3 position = Vector3.zero;
    public Vector3 Position => position;
    [SerializeField] // 向き
    private Vector3 rotation = Vector3.zero;
    public Vector3 Rotation => rotation;
    [SerializeField] // アニメーション
    private AnimationClip animationClip = null;
    public AnimationClip AnimationClip => animationClip;
}
