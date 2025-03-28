using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "DataManager", menuName = "ScriptableObject/DataManager")]
public class DataManager : SingletonScriptableObject<DataManager> {
    [SerializeField] private DataBase dataBase = null; // データベース
    public DataBase DataBase => dataBase;
    public SaveData Data { get; private set; } = null; // セーブデータ

    public Player Player => Data.Player;
    public List<Enemy> EnemyList { get; private set; } = new();


    public void Init() {
        EnemyList.Clear();
    }

    // セーブデータ
    public void Load() // ロード
    {
        try {
            StreamReader rd = new(Application.dataPath + "/SaveData.json");
            string json = rd.ReadToEnd();
            rd.Close();
            Data = JsonUtility.FromJson<SaveData>(json);
            Data.Init(dataBase);
        } catch {
            Data = new SaveData(dataBase);
            Save();
        }
    }

    public void Save() // セーブ
    {
        string json = JsonUtility.ToJson(Data);
        StreamWriter wr = new(Application.dataPath + "/SaveData.json", false);
        wr.WriteLine(json);
        wr.Close();
    }
}
