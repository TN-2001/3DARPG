using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I = null;

    [SerializeField] // データベース
    private DataBase dataBase = null;
    public DataBase DataBase => dataBase;
    [SerializeField] // セーブデータ
    private SaveData data = null;
    public SaveData Data => data;


    private void Awake()
    {
        if(!I){
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
            return;
        }

        Load();
    }

    private void Load()
    {
        try{
            StreamReader rd = new StreamReader(Application.dataPath + "/SaveData.json");
            string json = rd.ReadToEnd();
            rd.Close();
            data = JsonUtility.FromJson<SaveData>(json);
            data.Init(dataBase);
        }catch{
            data = new SaveData(dataBase);
            Save();
        }
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(data);
        StreamWriter wr = new StreamWriter(Application.dataPath + "/SaveData.json", false);
        wr.WriteLine(json);
        wr.Close(); 
    }

    private void OnDestroy()
    {
        if(data.Player.Data != null) Save();
    }
}
