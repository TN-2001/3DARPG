using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileUI : MonoBehaviour {
    // モバイル以外でオフ
    private enum Type {
        None,
        Image,
        Gameobject
    }
    [SerializeField]
    private Type isNotMobileToOff = Type.None;


    private void Start() {
        if (Application.isMobilePlatform) {
            if (GetComponent<Selectable>()) {
                Selectable selectable = GetComponent<Selectable>();
                ColorBlock colorBlock = selectable.colors;
                colorBlock.selectedColor = colorBlock.normalColor;
                selectable.colors = colorBlock;
            }
        } else {
            if (isNotMobileToOff == Type.Image) {
                GetComponent<Image>().enabled = false;
            } else if (isNotMobileToOff == Type.Gameobject) {
                gameObject.SetActive(false);
            }
        }
    }
}
