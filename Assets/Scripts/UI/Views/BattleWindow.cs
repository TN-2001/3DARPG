using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleWindow : MonoBehaviour
{
    [SerializeField] private Slider hpSlider = null; // HPスライダー
    [SerializeField] private Slider strSlider = null; // スタミナスライダー
    [SerializeField] private View inputView = null; // インプットビュー
    [SerializeField] private View itemView = null; // アイテムビュー
    [SerializeField] private Transform itemContentTra = null; // アイテムコンテンツ
    [SerializeField] private TextMeshProUGUI dmgText = null; // ダメージテキス
    [SerializeField] private Transform dmgContentTra = null; // ダメージコンテンツ
    [SerializeField] private List<FollowUI> mapFollowList = new(); // マップ
    [SerializeField] private Transform mapContentTransform = null; // マップコンテンツ
    [SerializeField] private FollowUI enemyMapFollow = null; // 敵のマップアイコン
    
    private readonly Dictionary<Transform, GameObject> enemyMapFollowList = new(); // 敵のマップアイコンリスト


    private void Start()
    {
        AudioManager.Instance.AudioMixer.SetFloat("BGM", DataManager.Instance.Data.VolumeList[0] - 80f);
        AudioManager.Instance.AudioMixer.SetFloat("SE", DataManager.Instance.Data.VolumeList[1] - 80f);
    }

    // HpSlider
    public void InitHpSlider(float hp)
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
    }
    public void UpdateHpSlider(float hp)
    {
        hpSlider.value = hp;
    }

    // StrSlider
    public void InitStrSlider(float str)
    {
        strSlider.maxValue = str;
        strSlider.value = str;
    }
    public void UpdateStrSlider(float str)
    {
        strSlider.value = str;
    }
    
    // アイテムゲット
    public void GetItem(ItemData itemData)
    {
        DataManager.Instance.Data.UpdateItem(itemData,1);
        itemView.UpdateUI(itemData.Name);
        GameObject obj = Instantiate(itemView.gameObject, Vector3.zero, Quaternion.identity, itemContentTra);
        Destroy(obj, 3f);
    }
    public void GetArmor(ArmorData armorData)
    {
        DataManager.Instance.Data.AddArmor(new Armor(armorData));
        itemView.UpdateUI(armorData.Name);
        GameObject obj = Instantiate(itemView.gameObject, Vector3.zero, Quaternion.identity, itemContentTra);
        Destroy(obj, 3f);
    }
    public void GetWeapon(WeaponData weaponData)
    {
        DataManager.Instance.Data.AddWeapon(new Weapon(weaponData));
        itemView.UpdateUI(weaponData.Name);
        GameObject obj = Instantiate(itemView.gameObject, Vector3.zero, Quaternion.identity, itemContentTra);
        Destroy(obj, 3f);
    }

    // ダメージテキスト
    public void InitDamageText(int damage, Vector3 position)
    {
        dmgText.text = damage.ToString();
        GameObject obj = Instantiate(dmgText.gameObject, Vector3.zero, Quaternion.identity, dmgContentTra);
        obj.GetComponent<FollowUI>().targetPosition = position;
    }

    // インプットビュー
    public void UpdateInputView(string text)
    {
        inputView.UpdateUI(text);
        inputView.gameObject.SetActive(true);
    }
    public void UpdateInputView()
    {
        inputView.gameObject.SetActive(false);
    }

    // マップ
    public void InitMapUI(Transform target)
    {
        for(int i = 0; i < mapFollowList.Count; i++){
            mapFollowList[i].targetTransform = target;
        }
    }
    public void AddMapEnemy(Transform target)
    {
        enemyMapFollow.targetTransform = target;
        GameObject gameObject = Instantiate(enemyMapFollow.gameObject, mapContentTransform);
        enemyMapFollowList.Add(target, gameObject);
    }
    public void RemoveMapEnemy(Transform target)
    {
        Destroy(enemyMapFollowList[target]);
        enemyMapFollowList.Remove(target);
    }
}
