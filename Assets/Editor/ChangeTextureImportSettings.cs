using UnityEngine;
using System.Collections;
using UnityEditor;

public class ChangeTextureImportSettings : ScriptableObject
{


    [MenuItem("Custom/Texture/Change Texture Format/Auto Compressed")]

    static void ChangeTextureFormat_AutoCompressed()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.AutomaticCompressed);

    }



    [MenuItem("Custom/Texture/Change Texture Format/Auto 16bit")]

    static void ChangeTextureFormat_Auto16Bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.Automatic16bit);

    }



    [MenuItem("Custom/Texture/Change Texture Format/Auto Truecolor")]

    static void ChangeTextureFormat_AutoTruecolor()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.AutomaticTruecolor);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGB Compressed DXT1")]

    static void ChangeTextureFormat_RGB_DXT1()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.DXT1);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGB Compressed DXT5")]

    static void ChangeTextureFormat_RGB_DXT5()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.DXT5);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGB 16 bit")]

    static void ChangeTextureFormat_RGB_16bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.RGB16);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGB 24 bit")]

    static void ChangeTextureFormat_RGB_24bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.RGB24);

    }



    [MenuItem("Custom/Texture/Change Texture Format/Alpha 8 bit")]

    static void ChangeTextureFormat_Alpha_8bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.Alpha8);

    }



    [MenuItem("Custom/Texture/Change Texture Format/ARGB 16 bit")]

    static void ChangeTextureFormat_RGBA_16bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.ARGB16);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGBA 32 bit")]

    static void ChangeTextureFormat_RGBA_32bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.RGBA32);

    }



    [MenuItem("Custom/Texture/Change Texture Format/ARGB 32 bit")]

    static void ChangeTextureFormat_ARGB_32bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.ARGB32);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGB PVRTC 2bit")]

    static void ChangeTextureFormat_RGB_PVRTC_2bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGB2);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGBA PVRTC 2bit")]

    static void ChangeTextureFormat_RGBA_PVRTC_2bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGBA2);

    }



    [MenuItem("Custom/Texture/Change Texture Format/RGB PVRTC 4bit")]

    static void ChangeTextureFormat_RGB_PVRTC_4bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGB4);

    }


    [MenuItem("Custom/Texture/Change Texture Format/RGBA PVRTC 4bit")]

    static void ChangeTextureFormat_RGBA_PVRTC_4bit()
    {

        SelectedChangeTextureFormatSettings(TextureImporterFormat.PVRTC_RGBA4);

    }


    [MenuItem("Custom/Texture/Change Texture Size/Change Max Texture Size/32")]

    static void ChangeTextureSize_32()
    {

        SelectedChangeMaxTextureSize(32);

    }


    [MenuItem("Custom/Texture/Change Texture Size/Change Max Texture Size/64")]

    static void ChangeTextureSize_64()
    {

        SelectedChangeMaxTextureSize(64);

    }


    [MenuItem("Custom/Texture/Change Texture Size/Change Max Texture Size/128")]

    static void ChangeTextureSize_128()
    {

        SelectedChangeMaxTextureSize(128);

    }



    [MenuItem("Custom/Texture/Change Texture Size/Change Max Texture Size/256")]

    static void ChangeTextureSize_256()
    {

        SelectedChangeMaxTextureSize(256);

    }


    [MenuItem("Custom/Texture/Change Texture Size/Change Max Texture Size/512")]

    static void ChangeTextureSize_512()
    {

        SelectedChangeMaxTextureSize(512);

    }


    [MenuItem("Custom/Texture/Change Texture Size/Change Max Texture Size/1024")]

    static void ChangeTextureSize_1024()
    {

        SelectedChangeMaxTextureSize(1024);

    }



    [MenuItem("Custom/Texture/Change Texture Size/Change Max Texture Size/2048")]

    static void ChangeTextureSize_2048()
    {

        SelectedChangeMaxTextureSize(2048);

    }



    [MenuItem("Custom/Texture/Change MipMap/Enable MipMap")]

    static void ChangeMipMap_On()
    {

        SelectedChangeMimMap(true);

    }



    [MenuItem("Custom/Texture/Change MipMap/Disable MipMap")]

    static void ChangeMipMap_Off()
    {

        SelectedChangeMimMap(false);

    }


    [MenuItem("Custom/Texture/Change Non Power of 2/None")]

    static void ChangeNPOT_None()
    {

        SelectedChangeNonPowerOf2(TextureImporterNPOTScale.None);

    }


    [MenuItem("Custom/Texture/Change Non Power of 2/ToNearest")]

    static void ChangeNPOT_ToNearest()
    {

        SelectedChangeNonPowerOf2(TextureImporterNPOTScale.ToNearest);

    }


    [MenuItem("Custom/Texture/Change Non Power of 2/ToLarger")]

    static void ChangeNPOT_ToLarger()
    {

        SelectedChangeNonPowerOf2(TextureImporterNPOTScale.ToLarger);

    }


    [MenuItem("Custom/Texture/Change Non Power of 2/ToSmaller")]

    static void ChangeNPOT_ToSmaller()
    {

        SelectedChangeNonPowerOf2(TextureImporterNPOTScale.ToSmaller);

    }


    [MenuItem("Custom/Texture/Change Is Readable/Enable")]

    static void ChangeIsReadable_Yes()
    {

        SelectedChangeIsReadable(true);

    }


    [MenuItem("Custom/Texture/Change Is Readable/Disable")]

    static void ChangeIsReadable_No()
    {

        SelectedChangeIsReadable(false);

    }


    static void SelectedChangeIsReadable(bool enabled)
    {



        Object[] textures = GetSelectedTextures();

        Selection.objects = new Object[0];

        foreach (Texture2D texture in textures)
        {

            string path = AssetDatabase.GetAssetPath(texture);

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            textureImporter.isReadable = enabled;

            AssetDatabase.ImportAsset(path);

        }

    }


    static void SelectedChangeNonPowerOf2(TextureImporterNPOTScale npot)
    {



        Object[] textures = GetSelectedTextures();

        Selection.objects = new Object[0];

        foreach(Texture2D texture in textures)
        {

            string path = AssetDatabase.GetAssetPath(texture);

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            textureImporter.npotScale = npot;

            AssetDatabase.ImportAsset(path);

        }

    }



    static void SelectedChangeMimMap(bool enabled)
    {



        Object[] textures = GetSelectedTextures();

        Selection.objects = new Object[0];

        foreach(Texture2D texture in textures)
        {

            string path = AssetDatabase.GetAssetPath(texture);

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            textureImporter.mipmapEnabled = enabled;

            AssetDatabase.ImportAsset(path);

        }

    }


    static void SelectedChangeMaxTextureSize(int size)
    {



        Object[] textures = GetSelectedTextures();

        Selection.objects = new Object[0];

        foreach(Texture2D texture in textures)
        {

            string path = AssetDatabase.GetAssetPath(texture);

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            textureImporter.maxTextureSize = size;

            AssetDatabase.ImportAsset(path);

        }

    }


    static void SelectedChangeTextureFormatSettings(TextureImporterFormat newFormat)
    {



        Object[] textures = GetSelectedTextures();

        Selection.objects = new Object[0];

        foreach(Texture2D texture in textures)
        {

            string path = AssetDatabase.GetAssetPath(texture);

            //Debug.Log("path: " + path);

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            textureImporter.textureFormat = newFormat;

            AssetDatabase.ImportAsset(path);

        }

    }



    static Object[] GetSelectedTextures()

    {

        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

    }

}

