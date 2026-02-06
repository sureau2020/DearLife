//using UnityEngine;
//using System.Collections.Generic;
//using System.IO;

//public class FurnitureLoader : MonoBehaviour
//{
//    private Dictionary<string, GameObject> furniturePrefabCache = new();
//    private Dictionary<string, GameObject> decorPrefabCache = new();
//    private Dictionary<string, Sprite> spriteCache = new();
    
//    public GameObject LoadFurniturePrefab(string furnitureId)
//    {
//        if (furniturePrefabCache.TryGetValue(furnitureId, out var cachedPrefab))
//            return cachedPrefab;
        
//        var furnitureData = FurnitureDatabase.Instance.GetFurnitureData(furnitureId);
//        if (furnitureData == null) return null;
        
//        GameObject prefab = null;
        
//        // 检查是否有直接的Sprite引用（ScriptableObject）
//        if (furnitureData.sprite != null)
//        {
//            prefab = CreatePrefabFromSprite(defaultFurniturePrefab, furnitureData.sprite);
//        }
//        // 检查是否有自定义图片路径（用户自定义）
//        else if (!string.IsNullOrEmpty(furnitureData.spritePath))
//        {
//            var sprite = LoadSpriteFromPath(furnitureData.spritePath);
//            if (sprite != null)
//            {
//                prefab = CreatePrefabFromSprite(defaultFurniturePrefab, sprite);
//            }
//        }
//        // 最后检查prefab路径（legacy支持）
//        else if (!string.IsNullOrEmpty(furnitureData.prefabPath))
//        {
//            prefab = Resources.Load<GameObject>(furnitureData.prefabPath);
//        }
        
//        if (prefab != null)
//            furniturePrefabCache[furnitureId] = prefab;
        
//        return prefab;
//    }
    
//    public GameObject LoadDecorPrefab(string decorId)
//    {
//        if (decorPrefabCache.TryGetValue(decorId, out var cachedPrefab))
//            return cachedPrefab;
        
//        var decorData = FurnitureDatabase.Instance.GetDecorData(decorId);
//        if (decorData == null) return null;
        
//        GameObject prefab = null;
        
//        // 检查是否有直接的Sprite引用（ScriptableObject）
//        if (decorData.sprite != null)
//        {
//            prefab = CreatePrefabFromSprite(defaultDecorPrefab, decorData.sprite);
//        }
//        // 检查是否有自定义图片路径（用户自定义）
//        else if (!string.IsNullOrEmpty(decorData.spritePath))
//        {
//            var sprite = LoadSpriteFromPath(decorData.spritePath);
//            if (sprite != null)
//            {
//                prefab = CreatePrefabFromSprite(defaultDecorPrefab, sprite);
//            }
//        }
//        // 最后检查prefab路径（legacy支持）
//        else if (!string.IsNullOrEmpty(decorData.prefabPath))
//        {
//            prefab = Resources.Load<GameObject>(decorData.prefabPath);
//        }
        
//        if (prefab != null)
//            decorPrefabCache[decorId] = prefab;
        
//        return prefab;
//    }
    
//    private GameObject CreatePrefabFromSprite(GameObject basePrefab, Sprite sprite)
//    {
//        // 创建基础预制件的副本
//        var prefab = Instantiate(basePrefab);
        
//        // 设置Sprite
//        var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
//        if (spriteRenderer != null)
//        {
//            spriteRenderer.sprite = sprite;
//        }
        
//        prefab.SetActive(false);
//        DontDestroyOnLoad(prefab);
        
//        return prefab;
//    }
    
//    private Sprite LoadSpriteFromPath(string path)
//    {
//        if (spriteCache.TryGetValue(path, out var cachedSprite))
//            return cachedSprite;
        
//        if (File.Exists(path))
//        {
//            byte[] fileData = File.ReadAllBytes(path);
//            Texture2D texture = new Texture2D(2, 2);
            
//            if (texture.LoadImage(fileData))
//            {
//                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100);
//                spriteCache[path] = sprite;
//                return sprite;
//            }
//        }
        
//        return null;
//    }
//}