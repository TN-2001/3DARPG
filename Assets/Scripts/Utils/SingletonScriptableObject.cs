using UnityEngine;

/*
    シングルトンのScriptableObject（ジェネリック）
    ResourcesフォルダにScriptableObjectを作成
*/

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject {
    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = Resources.Load<T>(typeof(T).Name);
                if (_instance == null) {
                    Debug.LogError($"{typeof(T).Name} not found in Resources folder.");
                }
            }
            return _instance;
        }
    }
}
