using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NativeGallery;

public class LoadHint : MonoBehaviour
{
    private GameObject clothSlot;
    [SerializeField]private Sprite template;

    public void Init(GameObject slot)
    {
        clothSlot = slot;
        template = AppearanceAtlasManager.Instance.GetPartSprite("Body", 1);
    }

    public void SaveTemplate()
    {
        if (template == null)
        {
            Debug.LogError("template is null");
            return;
        }

        Texture2D srcTex = template.texture;
        Rect rect = template.rect;

        // 创建可读纹理
        Texture2D readableTex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        Color[] pixels = srcTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        readableTex.SetPixels(pixels);
        readableTex.Apply();
        readableTex.filterMode = FilterMode.Point;

        byte[] bytes = readableTex.EncodeToPNG();
        NativeGallery.SaveImageToGallery(readableTex, "人生养成", "template.png");
        Debug.Log("Saved template to gallery");

        Destroy(readableTex); // 释放内存
    }

    public void PickImage()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, -1, false);
                if (texture == null)
                {
                    ErrorNotifier.NotifyError("无法加载图片");
                    return;
                }

                if (texture.width == 28 && texture.height == 24)
                {
                    texture.filterMode = FilterMode.Point;
                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        9
                    );

                    clothSlot.GetComponent<ClothUI>().UploadCloth(sprite);
                }
                else
                {
                    ErrorNotifier.NotifyError($"尺寸不符，要求 28x24，实际 {texture.width}x{texture.height}");
                }
            }
        }, "选择一张 28x24 的图片png", "image/png");

    }
}

