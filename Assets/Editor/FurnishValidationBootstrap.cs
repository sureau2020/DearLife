using System;
using System.IO;
using UnityEditor;
[InitializeOnLoad]
public static class FurnishValidationBootstrap {
 static FurnishValidationBootstrap() { EditorApplication.delayCall += Run; }
 static void Run() {
  if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) { EditorApplication.delayCall += Run; return; }
  if (!File.Exists("Temp/FurnishValidation/request")) return;
  File.Delete("Temp/FurnishValidation/request");
  try { CustomFurnishEditorBuilder.Build(); CustomFurnishValidation.Run(); }
  catch (Exception e) { File.WriteAllText("Temp/FurnishValidation/result.txt", e.ToString()); }
 }
}
