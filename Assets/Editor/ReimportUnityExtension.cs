using UnityEditor;
using System.IO;

public static class ReimportUnityExtensionsAssemblies
{
    [MenuItem("Assets/Reimport UnityExtensions Assemblies", false, 100)]
    static void reimport()
    {
        var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/";
        var dlls = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        foreach (var dll in dlls)
        {
            AssetDatabase.ImportAsset(dll, ImportAssetOptions.ForceUpdate | ImportAssetOptions.DontDownloadFromCacheServer);
        }
    }
}
