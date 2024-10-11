using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class View : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> textList = new List<TextMeshProUGUI>();
    [SerializeField]
    private List<Image> imageList = new List<Image>();

    public void UpdateUI(List<string> stringList, List<Sprite> spriteList)
    {
        if(stringList == null) stringList = new List<string>();
        for(int i = 0; i < textList.Count; i++)
        {
            if(i < stringList.Count) textList[i].text = stringList[i];
            else textList[i].text = "";
        }

        if(spriteList == null) spriteList = new List<Sprite>();
        for(int i = 0; i < imageList.Count; i++)
        {
            if(i < spriteList.Count)
            {
                imageList[i].sprite = spriteList[i];
                imageList[i].enabled = true;
            }
            else
            {
                imageList[i].sprite = null;
                imageList[i].enabled = false;
            }
        }
    }
    public void UpdateUI(List<string> stringList, Sprite sprite)
    {
        UpdateUI(stringList, new List<Sprite>(){sprite});
    }
    public void UpdateUI(string _string, List<Sprite> spriteList)
    {
        UpdateUI(new List<string>(){_string}, spriteList);
    }
    public void UpdateUI(List<string> stringList)
    {
        UpdateUI(stringList, new List<Sprite>());
    }
    public void UpdateUI(List<Sprite> spriteList)
    {
        UpdateUI(new List<string>(), spriteList);
    }
    public void UpdateUI(string _string, Sprite sprite)
    {
        UpdateUI(new List<string>(){_string}, new List<Sprite>(){sprite});
    }
    public void UpdateUI(string _string)
    {
        UpdateUI(new List<string>(){_string});
    }
    public void UpdateUI(Sprite sprite)
    {
        UpdateUI(new List<Sprite>(){sprite});
    }
}
