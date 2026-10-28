using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

// Owns saved images and runtime sprites. UI and room rendering only borrow them.
public sealed class CustomFurnishLibrary : IDisposable
{
    public sealed class Entry
    {
        public CustomFurnishData Data { get; }
        public Sprite Sprite { get; }
        internal Entry(CustomFurnishData data, Sprite sprite) { Data = data; Sprite = sprite; }
    }

    private readonly Dictionary<string, Entry> entries = new();
    private readonly string root;
    public IEnumerable<Entry> Entries => entries.Values;
    public event Action<Entry> Added;

    public CustomFurnishLibrary(string directory = null)
    {
        root = directory ?? Path.Combine(Application.persistentDataPath, "CustomFurnish");
        Load();
    }

    public Entry Get(string id) => id != null && entries.TryGetValue(id, out var entry) ? entry : null;

    private void Load()
    {
        if (!Directory.Exists(root)) return;
        try
        {
            foreach (string directory in Directory.GetDirectories(root, "custom_*"))
            {
                Texture2D texture = null;
                try
                {
                    string id = Path.GetFileName(directory);
                    if (!IsValidId(id)) continue;
                    var data = JsonConvert.DeserializeObject<CustomFurnishData>(File.ReadAllText(Path.Combine(directory, "data.json")));
                    if (data == null || data.id != id || data.Validate() != null)
                        throw new InvalidDataException("素材数据无效");
                    byte[] bytes = File.ReadAllBytes(Path.Combine(directory, "image.png"));
                    if (bytes.Length > 2 * 1024 * 1024) throw new InvalidDataException("素材图片文件过大");
                    // Our saved files are PNG. Check IHDR before allocating the decoded image.
                    if (!HasPngDimensions(bytes, data.width, data.height))
                        throw new InvalidDataException("素材图片尺寸与数据不一致");
                    texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (!texture.LoadImage(bytes) || texture.width != data.width || texture.height != data.height)
                        throw new InvalidDataException("无法读取素材图片");
                    entries.Add(id, CreateEntry(data, texture));
                    texture = null;
                }
                catch (Exception e) { Debug.LogWarning($"跳过损坏的装修素材 {Path.GetFileName(directory)}: {e.Message}"); }
                finally { if (texture != null) Release(texture); }
            }
        }
        catch (Exception e) { Debug.LogWarning($"读取装修素材库失败: {e.Message}"); }
    }

    public OperationResult<Entry> Save(CustomFurnishData draft, Texture2D image)
    {
        if (draft == null || image == null) return OperationResult<Entry>.Fail("请先选择图片");
        string error = draft.Validate();
        if (error != null) return OperationResult<Entry>.Fail(error);
        if (image.width != draft.width || image.height != draft.height)
            return OperationResult<Entry>.Fail("图片尺寸与编辑数据不一致");

        string staging = null;
        Texture2D ownedTexture = null;
        Sprite sprite = null;
        Entry entry;
        try
        {
            var data = JsonConvert.DeserializeObject<CustomFurnishData>(JsonConvert.SerializeObject(draft));
            data.id = "custom_" + Guid.NewGuid().ToString("N");
            byte[] png = image.EncodeToPNG();
            if (png == null || png.Length == 0) throw new IOException("无法编码图片");
            ownedTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!ownedTexture.LoadImage(png)) throw new IOException("无法复制图片");
            entry = CreateEntry(data, ownedTexture);
            sprite = entry.Sprite;
            Directory.CreateDirectory(root);
            staging = Path.Combine(root, ".pending_" + data.id);
            Directory.CreateDirectory(staging);
            File.WriteAllBytes(Path.Combine(staging, "image.png"), png);
            File.WriteAllText(Path.Combine(staging, "data.json"), JsonConvert.SerializeObject(data, Formatting.Indented));
            Directory.Move(staging, Path.Combine(root, data.id));
            staging = null;
            entries.Add(data.id, entry);
        }
        catch (Exception e)
        {
            if (sprite != null) Release(sprite);
            if (ownedTexture != null) Release(ownedTexture);
            if (staging != null)
            {
                try { Directory.Delete(staging, true); }
                catch (Exception cleanup) { Debug.LogWarning(cleanup.Message); }
            }
            return OperationResult<Entry>.Fail("保存素材失败：" + e.Message);
        }
        // Disk commit has succeeded; a consumer error must not turn this into a failed save.
        if (Added != null)
            foreach (Action<Entry> handler in Added.GetInvocationList())
                try { handler(entry); } catch (Exception e) { Debug.LogException(e); }
        return OperationResult<Entry>.Complete(entry);
    }

    private static Entry CreateEntry(CustomFurnishData data, Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(data.anchorX / (float)data.width, data.anchorY / (float)data.height),
            CustomFurnishData.PixelsPerCell, 0, SpriteMeshType.FullRect);
        sprite.name = data.id;
        return new Entry(data, sprite);
    }

    private static bool IsValidId(string id) => id.StartsWith("custom_", StringComparison.Ordinal)
        && id.Length == 39 && Guid.TryParseExact(id.Substring(7), "N", out _);

    private static bool HasPngDimensions(byte[] bytes, int width, int height)
    {
        if (bytes.Length < 24 || bytes[0] != 137 || bytes[1] != 80 || bytes[2] != 78 || bytes[3] != 71
            || bytes[12] != 73 || bytes[13] != 72 || bytes[14] != 68 || bytes[15] != 82) return false;
        return ReadBigEndian(bytes, 16) == width && ReadBigEndian(bytes, 20) == height;
    }

    private static int ReadBigEndian(byte[] b, int i) => (b[i] << 24) | (b[i + 1] << 16) | (b[i + 2] << 8) | b[i + 3];

    private static void Release(UnityEngine.Object value)
    {
        if (Application.isPlaying) UnityEngine.Object.Destroy(value);
        else UnityEngine.Object.DestroyImmediate(value);
    }

    public void Dispose()
    {
        Added = null;
        foreach (var entry in entries.Values)
        {
            Release(entry.Sprite.texture);
            Release(entry.Sprite);
        }
        entries.Clear();
    }
}
