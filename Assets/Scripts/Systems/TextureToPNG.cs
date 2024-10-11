using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureToPNG : MonoBehaviour
{
    [Header("レンダーテクスチャの色形式はSRGM")]
    [SerializeField]
    private RenderTexture renderTexture = null;


    [ContextMenu("RenderTextureToPNG")]
    public void RenderTextureToPNG()
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + "/SavedImage.png";
        File.WriteAllBytes(path, bytes);
    }
}
