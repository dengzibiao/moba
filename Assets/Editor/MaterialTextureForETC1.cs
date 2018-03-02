using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 压缩工具，RGB和Alpha分离
/// </summary>
public class MaterialTextureForETC1
{


    private static Texture2D defaultWhiteTex = null;

    [MenuItem("Tool/Depart RGB and Alpha Channel")]
    static void SeperateAllTexturesRGBandAlphaChannel()
    {

        Object[] SelectAsset = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        foreach (var item in SelectAsset)
        {
            var path = AssetDatabase.GetAssetPath(item);

            if (!string.IsNullOrEmpty(path) && IsTextureFile(path) && !IsTextureConverted(path))
            {
                SeperateRGBAandlphaChannel(path);
            }

            Debug.Log("Succeed : " + item);

        }

        //刷新
        AssetDatabase.Refresh();

    }

    #region process texture  

    static void SeperateRGBAandlphaChannel(string _texPath)
    {

        string assetRelativePath = GetRelativeAssetPath(_texPath);

        //设置文件只读
        SetTextureReadableEx(assetRelativePath);

        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;

        if (!sourcetex)
        {
            Debug.LogError("Load Texture : " + sourcetex);
            return;
        }

        TextureImporter ti = null;

        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
        }
        catch
        {
            Debug.LogError("Load Texture : " + sourcetex);
            return;
        }
        if (ti == null)
        {
            return;
        }
        bool bGenerateMipMap = ti.mipmapEnabled;

        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, bGenerateMipMap);

        rgbTex.SetPixels(sourcetex.GetPixels());

        Texture2D mipMapTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGBA32, true);

        mipMapTex.SetPixels(sourcetex.GetPixels());

        mipMapTex.Apply();

        Color[] colors2rdLevel = mipMapTex.GetPixels(1);

        Color[] colorsAlpha = new Color[colors2rdLevel.Length];

        if (colors2rdLevel.Length != (mipMapTex.width) / 2 * (mipMapTex.height) / 2)
        {
            Debug.LogError("Size Error.");
            return;
        }

        bool bAlphaExist = false;

        for (int i = 0; i < colors2rdLevel.Length; ++i)
        {
            colorsAlpha[i].r = colors2rdLevel[i].a;
            colorsAlpha[i].g = colors2rdLevel[i].a;
            colorsAlpha[i].b = colors2rdLevel[i].a;

            if (!Mathf.Approximately(colors2rdLevel[i].a, 1.0f))
            {
                bAlphaExist = true;
            }
        }

        Texture2D alphaTex = null;

        if (bAlphaExist)
        {
            alphaTex = new Texture2D((sourcetex.width + 1) / 2, (sourcetex.height + 1) / 2, TextureFormat.RGB24, bGenerateMipMap);
        }
        else
        {
            alphaTex = new Texture2D(defaultWhiteTex.width, defaultWhiteTex.height, TextureFormat.RGB24, false);
        }

        alphaTex.SetPixels(colorsAlpha);

        rgbTex.Apply();

        alphaTex.Apply();

        byte[] bytes = rgbTex.EncodeToPNG();

        assetRelativePath = GetRGBTexPath(_texPath);

        File.WriteAllBytes(assetRelativePath, bytes);

        byte[] alphabytes = alphaTex.EncodeToPNG();

        string alphaTexRelativePath = GetAlphaTexPath(_texPath);

        File.WriteAllBytes(alphaTexRelativePath, alphabytes);

        ReImportAsset(assetRelativePath, rgbTex.width, rgbTex.height);

        ReImportAsset(alphaTexRelativePath, alphaTex.width, alphaTex.height);

    }

    static void ReImportAsset(string path, int width, int height)
    {
        try
        {
            AssetDatabase.ImportAsset(path);
        }
        catch
        {
            Debug.LogError("Import Texture failed: " + path);
            return;
        }

        TextureImporter importer = null;
        try
        {
            importer = (TextureImporter)TextureImporter.GetAtPath(path);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + path);
            return;
        }
        if (importer == null)
        {
            return;
        }
        importer.maxTextureSize = Mathf.Max(width, height);
        importer.anisoLevel = 0;
        importer.isReadable = false;
        importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
        importer.textureType = TextureImporterType.Default;
        if (path.Contains("/UI/"))
        {
            importer.textureType = TextureImporterType.GUI;
        }
        AssetDatabase.ImportAsset(path);
    }


    static void SetTextureReadableEx(string _relativeAssetPath)
    {
        TextureImporter ti = null;
        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + _relativeAssetPath);
            return;
        }
        if (ti == null)
        {
            return;
        }
        ti.isReadable = true;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;      //this is essential for departing Textures for ETC1. No compression format for following operation.
        AssetDatabase.ImportAsset(_relativeAssetPath);
    }

    #endregion

    #region string or path helper    

    static bool IsTextureFile(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
    }

    static bool IsTextureConverted(string _path)
    {
        return _path.Contains("_RGB.") || _path.Contains("_Alpha.");
    }

    static string GetRGBTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_RGB.");
    }

    static string GetAlphaTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_Alpha.");
    }

    static string GetTexPath(string _texPath, string _texRole)
    {
        string dir = System.IO.Path.GetDirectoryName(_texPath);
        string filename = System.IO.Path.GetFileNameWithoutExtension(_texPath);
        string result = dir + "/" + filename + _texRole + "png";
        return result;
    }

    static string GetRelativeAssetPath(string _fullPath)
    {
        _fullPath = GetRightFormatPath(_fullPath);
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    static string GetRightFormatPath(string _path)
    {
        return _path.Replace("\\", "/");
    }

    #endregion
}