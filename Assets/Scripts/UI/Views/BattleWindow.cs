using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class BattleWindow : MonoBehaviour
{
    [SerializeField] // オーディオミキサー
    private AudioMixer audioMixer = null;

    [SerializeField] // HPスライダー
    private Slider hpSlider = null;
    [SerializeField] // スタミナスライダー
    private Slider strSlider = null;
    [SerializeField] // インプットビュー
    private View inputView = null;
    [SerializeField] // アイテムビュー
    private View itemView = null;
    [SerializeField] // アイテムコンテンツ
    private Transform itemContentTra = null;
    [SerializeField] // ダメージテキス
    private TextMeshProUGUI dmgText = null;
    [SerializeField] // ダメージコンテンツ
    private Transform dmgContentTra = null;
    [SerializeField] // 武器ビュー
    private List<View> weaponViewList = new();
    [SerializeField] // マップ
    private List<Follow> mapFollowList = new();


    private void OnEnable()
    {
        for(int i = 0; i < weaponViewList.Count; i++){
            weaponViewList[i].UpdateUI(new List<Sprite>());
            if(GameManager.I.Data.Player.WeaponList[i] != null){
                weaponViewList[i].UpdateUI(GameManager.I.Data.Player.WeaponList[i].Data.Image);
            }
        }
    }

    private void Start()
    {
        audioMixer.SetFloat("BGM", GameManager.I.Data.VolumeList[0] - 80f);
        audioMixer.SetFloat("SE", GameManager.I.Data.VolumeList[1] - 80f);
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
        GameManager.I.Data.UpdateItem(itemData,1);
        itemView.UpdateUI(itemData.Name);
        GameObject obj = Instantiate(itemView.gameObject, Vector3.zero, Quaternion.identity, itemContentTra);
        Destroy(obj, 3f);
    }
    public void GetArmor(ArmorData armorData)
    {
        GameManager.I.Data.AddArmor(new Armor(armorData));
        itemView.UpdateUI(armorData.Name);
        GameObject obj = Instantiate(itemView.gameObject, Vector3.zero, Quaternion.identity, itemContentTra);
        Destroy(obj, 3f);
    }
    public void GetWeapon(WeaponData weaponData)
    {
        GameManager.I.Data.AddWeapon(new Weapon(weaponData));
        itemView.UpdateUI(weaponData.Name);
        GameObject obj = Instantiate(itemView.gameObject, Vector3.zero, Quaternion.identity, itemContentTra);
        Destroy(obj, 3f);
    }

    // ダメージテキスト
    public void InitDamageText(int damage, Vector3 position)
    {
        dmgText.text = damage.ToString();
        GameObject obj = Instantiate(dmgText.gameObject, Vector3.zero, Quaternion.identity, dmgContentTra);
        obj.GetComponent<FollowUI>().Init(position);
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
            mapFollowList[i].target = target;
        }
    }
}
