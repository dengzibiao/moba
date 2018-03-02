using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;

public class MaterialTextureForETC1_new
{

    private static string defaultWhiteTexPath_relative = "Assets/Default_Alpha.png";
    private static Texture2D defaultWhiteTex = null;

    [MenuItem("EffortForETC1/Depart RGB and Alpha Channel")]
    static void SeperateAllTexturesRGBandAlphaChannel()
    {
        Debug.Log("Start Departing.");
        if (!GetDefaultWhiteTexture())
        {
            return;
        }
        string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            if (!string.IsNullOrEmpty(path) && IsTextureFile(path) && !IsTextureConverted(path))   //full name  
            {
                SeperateRGBAandlphaChannel(path);
            }
        }
        AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file
        Debug.Log("Finish Departing.");
    }

    #region process texture

    static void SeperateRGBAandlphaChannel(string _texPath)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        SetTextureReadableEx(assetRelativePath);    //set readable flag and set textureFormat TrueColor
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file  
        if (!sourcetex)
        {
            Debug.LogError("Load Texture Failed : " + assetRelativePath);
            return;
        }

        TextureImporter ti = null;
        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + assetRelativePath);
            return;
        }
        if (ti == null)
        {
            return;
        }
        bool bGenerateMipMap = ti.mipmapEnabled;    //same with the texture import setting      

        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, bGenerateMipMap);
        rgbTex.SetPixels(sourcetex.GetPixels());

        Texture2D mipMapTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGBA32, true);  //Alpha Channel needed here
        mipMapTex.SetPixels(sourcetex.GetPixels());
        mipMapTex.Apply();
        Color[] colors2rdLevel = mipMapTex.GetPixels(1);   //Second level of Mipmap
        Color[] colorsAlpha = new Color[colors2rdLevel.Length];
        if (colors2rdLevel.Length != (mipMapTex.width + 1) / 2 * (mipMapTex.height + 1) / 2)
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
        File.WriteAllBytes(assetRelativePath, bytes);
        byte[] alphabytes = alphaTex.EncodeToPNG();
        string alphaTexRelativePath = GetAlphaTexPath(_texPath);
        File.WriteAllBytes(alphaTexRelativePath, alphabytes);

        ReImportAsset(assetRelativePath, rgbTex.width, rgbTex.height);
        ReImportAsset(alphaTexRelativePath, alphaTex.width, alphaTex.height);
        Debug.Log("Succeed Departing : " + assetRelativePath);
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
        importer.isReadable = false;  //increase memory cost if readable is true
        importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
        importer.textureType = TextureImporterType.Default;
        if (path.Contains("/UI/"))
        {
            importer.textureType = TextureImporterType.GUI;
        }
        AssetDatabase.ImportAsset(path);
    }


    static void SetTextureReadableEx(string _relativeAssetPath)    //set readable flag and set textureFormat TrueColor
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

    static bool GetDefaultWhiteTexture()
    {
        defaultWhiteTex = AssetDatabase.LoadAssetAtPath(defaultWhiteTexPath_relative, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file  
        if (!defaultWhiteTex)
        {
            Debug.LogError("Load Texture Failed : " + defaultWhiteTexPath_relative);
            return false;
        }
        return true;
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