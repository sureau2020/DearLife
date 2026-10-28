using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Runs without opening scenes or touching the player's actual save files.
public static class CustomFurnishValidation
{
    private static int checks;
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("FAILED: " + label);
        checks++;
        Debug.Log("CUSTOM_FURNISH_CHECK: " + label);
    }

    [MenuItem("Tools/DearLife/Validate Custom Furnish")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Run outside Play Mode");
        checks = 0;
        string root = Path.GetFullPath("Temp/FurnishValidation/" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var originalManager = GameManager.Instance;
        var holder = new GameObject("FurnishValidation");
        holder.hideFlags = HideFlags.HideAndDontSave;
        holder.SetActive(false);
        var manager = holder.AddComponent<GameManager>();
        var database = new FurnitureDatabase(Path.Combine(root, "library"));
        var texture = new Texture2D(33, 31, TextureFormat.RGBA32, false);
        var redirected = new List<(FieldInfo field, object value)>();
        try
        {
            Property(typeof(GameManager), "Instance", null, manager);
            Property(typeof(GameManager), "FurnitureDatabase", manager, database);
            Property(typeof(GameManager), "TileDataBase", manager, new TileDataBase());
            Redirect(typeof(SaveManager), "SaveDataFolder", root, redirected);
            Redirect(typeof(SaveManager), "GridMapFilePath", Path.Combine(root, "GridMap.json"), redirected);
            Redirect(typeof(LoaderManager), "GridMapFilePath", Path.Combine(root, "GridMap.json"), redirected);

            var data = new CustomFurnishData { displayName = "测试家具", width = 500, height = 500 };
            Check(data.Validate() == null && data.GridSize() == new Vector2Int(32, 32), "500px accepted, partial cells rounded up");
            data.width = 501;
            Check(data.Validate() != null, "501px rejected");
            data.width = 33; data.height = 31; data.anchorX = 1; data.anchorY = 15;
            Check(data.GridSize() == new Vector2Int(2, 1), "offset anchor grid dimensions");
            data.anchorX = 33;
            Check(data.Validate() != null, "outside anchor rejected");
            data.anchorX = 1;
            data.blockedCells.Add(new CustomFurnishCell(2, 0));
            Check(data.Validate() != null, "out of range collision rejected");
            data.blockedCells.Clear();
            data.kind = CustomFurnishKind.Decor;
            data.blockedCells.Add(new CustomFurnishCell(0, 0));
            Check(data.Validate() != null, "decor rejects collision");
            data.kind = CustomFurnishKind.Background;
            data.blockedCells.Add(new CustomFurnishCell(1, 0));
            Check(data.Validate() != null, "background requires a walkable cell");
            data.blockedCells.RemoveAt(1);
            var background = database.CustomItems.Save(data, texture);
            Check(background.Success, "save background PNG and data");
            string bgId = background.Data.Data.id;
            Check(background.Data.Sprite.pivot == new Vector2(1, 15) && background.Data.Sprite.pixelsPerUnit == 16, "sprite anchor and scale preserved");
            data.blockedCells.Clear();
            Check(background.Data.Data.blockedCells.Count == 1, "saved data owns snapshot");
            data.kind = CustomFurnishKind.Furniture;
            var allWhite = database.CustomItems.Save(data, texture);
            Check(allWhite.Success && database.GetFurnitureData(allWhite.Data.Data.id) != null, "all-white furniture registered");
            data.blockedCells.Add(new CustomFurnishCell(0, 0));
            var red = database.CustomItems.Save(data, texture);
            Check(red.Success, "collision furniture saved");
            data.kind = CustomFurnishKind.Decor; data.blockedCells.Clear();
            var decor = database.CustomItems.Save(data, texture);
            Check(decor.Success && database.GetDecorData(decor.Data.Data.id) != null, "decor registered");

            var map = new GridMap();
            map.ChangeFloor(bgId);
            Check(!map.CanWalk(Vector2Int.zero) && map.CanWalk(Vector2Int.right) && !map.HasFloor(new Vector2Int(2, 0)), "background mask and bounds");
            var whiteData = database.GetFurnitureData(allWhite.Data.Data.id);
            Check(map.PlaceFurniture(Vector2Int.right, whiteData) && map.CanWalk(Vector2Int.right), "all-white furniture does not block");
            var instance = map.GetFurniture(Vector2Int.right);
            Check(instance != null && whiteData.GetMaxOccupiedY() == 0, "all-white furniture remains selectable and sortable");
            Check(!map.CanPlaceFurniture(whiteData, Vector2Int.right, "other"), "all-white anchor remains independently selectable");
            var redData = database.GetFurnitureData(red.Data.Data.id);
            map.RemoveFurniture(instance);
            Check(map.PlaceFurniture(Vector2Int.right, redData) && !map.CanWalk(Vector2Int.right), "red cell blocks movement");
            map.RemoveFurniture(map.GetFurniture(Vector2Int.right));
            Check(map.CanWalk(Vector2Int.right), "removing furniture restores walking");
            map.PlaceFurniture(Vector2Int.right, whiteData);
            Check(SaveManager.SaveGridMap(map).Success, "map saved to isolated test path");
            var restored = LoaderManager.LoadGridMap();
            Check(restored.Success && restored.Data.CustomBackgroundId == bgId && restored.Data.CanWalk(Vector2Int.right)
                && !restored.Data.CanWalk(Vector2Int.zero) && restored.Data.GetFurniture(Vector2Int.right) != null,
                "map save/load restores background, mask and all-white instance");
            var oldData = JsonConvert.DeserializeObject<SaveManager.GridMapSaveData>("{}");
            Check(oldData.CustomBackgroundId == null && map.RestoreCustomBackground(null), "old saves default to builtin room");
            Check(!map.RestoreCustomBackground("missing"), "missing background detected");

            using (var reloaded = new CustomFurnishLibrary(Path.Combine(root, "library")))
                Check(reloaded.Entries.Count() == 4 && reloaded.Get(bgId).Data.anchorY == 15, "library restart restores four assets");
            File.WriteAllText(Path.Combine(root, "library", red.Data.Data.id, "data.json"), "broken");
            using (var partial = new CustomFurnishLibrary(Path.Combine(root, "library")))
                Check(partial.Entries.Count() == 3 && partial.Get(bgId) != null, "damaged item does not break library");
            File.WriteAllText(Path.Combine(root, "not-a-directory"), "test");
            using (var failed = new CustomFurnishLibrary(Path.Combine(root, "not-a-directory")))
                Check(!failed.Save(data, texture).Success && !failed.Entries.Any(), "disk failure never registers unsaved item");
            ValidatePreview(texture);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CustomFurnishEditorBuilder.PrefabPath);
            Check(prefab != null && prefab.GetComponent<CustomFurnishEditor>() != null, "prefab created");
            var serialized = new SerializedObject(prefab.GetComponent<CustomFurnishEditor>());
            foreach (string field in new[] { "title", "itemName", "status", "modeLabel", "pickButton", "modeButton", "saveButton", "cancelButton", "preview" })
                Check(serialized.FindProperty(field).objectReferenceValue != null, "prefab wire: " + field);
            CapturePreview(prefab, database.CustomItems);
            File.WriteAllText("Temp/FurnishValidation/result.txt", "PASS " + checks + " checks\n" + DateTime.Now.ToString("s"));
            Debug.Log("CUSTOM_FURNISH_VALIDATION_PASS: " + checks);
        }
        catch (Exception e)
        {
            File.WriteAllText("Temp/FurnishValidation/result.txt", e.ToString());
            Debug.LogException(e);
            throw;
        }
        finally
        {
            foreach (var item in redirected) item.field.SetValue(null, item.value);
            Property(typeof(GameManager), "Instance", null, originalManager);
            Property(typeof(GameManager), "FurnitureDatabase", manager, null);
            database.DisposeCustomItems();
            UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(holder);
        }
    }

    private static void CapturePreview(GameObject prefab, CustomFurnishLibrary library)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var target = new RenderTexture(720, 1280, 24);
        Texture2D capture = null;
        RenderTexture previous = RenderTexture.active;
        try
        {
            var cameraObject = new GameObject("PreviewCamera", typeof(Camera));
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 640;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.targetTexture = target;
            camera.scene = scene;
            var canvasObject = new GameObject("PreviewCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            SceneManager.MoveGameObjectToScene(canvasObject, scene);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            var ui = UnityEngine.Object.Instantiate(prefab, canvas.transform);
            var editor = ui.GetComponent<CustomFurnishEditor>();
            editor.Initialize(CustomFurnishKind.Furniture, library, null, null);
            var picture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath("a19bfa0325167144688c8bf5b057e6a3"));
            ui.GetComponentInChildren<CustomFurnishPreview>().SetImage(picture, CustomFurnishKind.Furniture);
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            capture = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            capture.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            capture.Apply();
            File.WriteAllBytes("Temp/FurnishValidation/editor-preview.png", capture.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active = previous;
            if (capture != null) UnityEngine.Object.DestroyImmediate(capture);
            EditorSceneManager.ClosePreviewScene(scene);
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    private static void ValidatePreview(Texture2D texture)
    {
        var root = new GameObject("PreviewValidation", typeof(RectTransform), typeof(Canvas));
        root.hideFlags = HideFlags.HideAndDontSave;
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var events = new GameObject("PreviewEvents", typeof(EventSystem));
        events.hideFlags = HideFlags.HideAndDontSave;
        try
        {
            var image = new GameObject("Image", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            image.transform.SetParent(root.transform, false);
            var preview = new GameObject("Preview", typeof(RectTransform), typeof(CustomFurnishPreview)).GetComponent<CustomFurnishPreview>();
            preview.transform.SetParent(root.transform, false);
            preview.rectTransform.sizeDelta = new Vector2(350, 330);
            preview.Configure(image);
            preview.SetImage(texture, CustomFurnishKind.Furniture);
            Canvas.ForceUpdateCanvases();
            var local = new Vector2(-160, -150); // Image bottom-left(-165,-155), pixel(0.5,0.5).
            var pointer = new PointerEventData(events.GetComponent<EventSystem>()) {
                position = RectTransformUtility.WorldToScreenPoint(null, preview.rectTransform.TransformPoint(local)),
                button = PointerEventData.InputButton.Left
            };
            preview.OnPointerClick(pointer);
            Check(preview.GetBlockedCells().Count == 1, "preview white to red");
            preview.OnPointerClick(pointer);
            Check(preview.GetBlockedCells().Count == 0, "preview red to white");
            preview.OnPointerClick(pointer);
            preview.EditAnchor = true;
            pointer.position = RectTransformUtility.WorldToScreenPoint(null, preview.rectTransform.TransformPoint(new Vector2(-65, -55)));
            preview.OnDrag(pointer);
            Check(preview.Anchor == new Vector2Int(10, 10) && preview.GetBlockedCells().Count == 0, "moving anchor clears mask");
            preview.SetImage(texture, CustomFurnishKind.Decor);
            preview.EditAnchor = false;
            preview.OnPointerClick(pointer);
            Check(preview.GetBlockedCells().Count == 0, "decor preview has no collision editing");
        }
        finally { UnityEngine.Object.DestroyImmediate(root); UnityEngine.Object.DestroyImmediate(events); }
    }

    private static void Property(Type type, string name, object target, object value)
        => type.GetProperty(name).SetValue(target, value);
    private static void Redirect(Type type, string name, string path, List<(FieldInfo, object)> originals)
    {
        var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
        originals.Add((field, field.GetValue(null)));
        field.SetValue(null, path);
        if ((string)field.GetValue(null) != path) throw new Exception("Test save path was not redirected");
    }
}
