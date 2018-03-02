using UnityEngine;
using System;
using ScionEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace ScionGUI
{
	[Serializable]
	public class ScionWindow : EditorWindow
	{
		const string ScionVersion = "1.2";

		const int width = 500;
		const int height = 350;
		
		[MenuItem("Window/Scion")]
		public static void Initialize()
		{
			EditorWindow editorWindow = EditorWindow.GetWindow<ScionWindow>();
			editorWindow.titleContent = new GUIContent("Scion");
			editorWindow.minSize = new Vector2(width, height);
			editorWindow.Show();
		}

		private static Texture2D s_ScionHeader;
		private static Texture2D ScionHeader 
		{
			get
			{
				if (s_ScionHeader == null) s_ScionHeader = ScionGUIUtil.LoadPNG("SCIONLogo1000x300");
				return s_ScionHeader;
			}
		}
		
		private void OnDisable()
		{
			GUI.FocusControl(null);
		}

		[SerializeField] private Gradient gradient = new Gradient();
		private SerializedObject thisSerialized;
		private SerializedProperty serializedGradient;

		private void OnEnable()
		{
			thisSerialized = new UnityEditor.SerializedObject(this);
			serializedGradient = thisSerialized.FindProperty("gradient");
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
		
		private void OnGUI()
		{
			DrawHeader();
			DrawVersionInfo();
			DrawColorGrading();
			DrawLensFlareColor();
		}
		
//		private ScionPostProcess activeContext { get; set; }
//		private void UpdateSelected()
//		{
//			GameObject selectedGO = Selection.activeGameObject;
//			if (selectedGO == null) { activeContext = null; return; }
//
//			ScionPostProcess selectedScion = selectedGO.GetComponent<ScionPostProcess>();
//			if (selectedScion == null) { activeContext = null; return; }
//
//			if (activeContext != selectedScion)
//			{
//				activeContext = selectedScion;
//			}
//		}

		private float headerWidth { get { return width; } }
		private float headerHeight { get { return width * 3.0f / 10.0f; } }

		private void DrawHeader()
		{
			Rect headerRect = new Rect(0, 0, headerWidth, headerHeight);
			if (ScionHeader != null) GUI.DrawTexture(headerRect, ScionHeader, ScaleMode.ScaleToFit, false);
			GUILayout.Space(headerHeight + 5);
		}

		private void DrawVersionInfo()
		{			
			GUILayout.Label("Version " + ScionVersion, EditorStyles.centeredGreyMiniLabel);
		}
		
		private Texture2D inputLUT;
		private string inputPath = "";
		private bool linearInput;
		private const string DefaultHelpMessage = "Supply a color grading lookup texture and choose the program it is compatible with";
		private string helpBoxMessage = DefaultHelpMessage;
		private MessageType messageType = MessageType.None;
		private ColorGradingCompatibility compatibilityMode;

		private void ResetHelpBox()
		{
			helpBoxMessage = DefaultHelpMessage;
			messageType = MessageType.None;
		}

		private void DrawColorGrading()
		{			
			GUILayout.Label("Color Grading", EditorStyles.largeLabel);
			DrawColorGradingInput();
			DrawColorGradingConvertButton();

			if (helpBoxMessage == "") helpBoxMessage = DefaultHelpMessage;
			EditorGUILayout.HelpBox(helpBoxMessage, messageType, true);
		}
		
		private void DrawColorGradingInput()
		{
			GUILayout.Label("Input Lookup Texture", EditorStyles.boldLabel);
			
			Texture2D newInput = EditorGUILayout.ObjectField(inputLUT, typeof(Texture2D), false) as Texture2D;
			if (newInput != inputLUT)
			{
				inputLUT = newInput;
				if (newInput != null) inputPath = AssetDatabase.GetAssetPath(inputLUT);
				else inputPath = "";

				ResetHelpBox();
			}			
			
			GUILayout.Label("Compatibility", EditorStyles.boldLabel);
			compatibilityMode = (ColorGradingCompatibility)EditorGUILayout.EnumPopup(compatibilityMode);
		}
		
		private void DrawColorGradingConvertButton()
		{
			GUILayout.Space(5);
			if (GUILayout.Button("Convert") == false) return;

			if (inputLUT == null) 
			{
				messageType = MessageType.Error;
				helpBoxMessage = "Please supply an input lookup texture";
				return;
			}

			TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(inputPath);
			Texture2D converted = ColorGrading.Convert(inputLUT, compatibilityMode, importer.linearTexture);

			//Remove file extension and add "_Scion.png"
			string newPath = inputPath.Substring(0, inputPath.LastIndexOf(".")) + "_Scion.png";
			SaveLookupTexture(newPath, converted);

			helpBoxMessage = "Saved converted lookup texture: " + newPath;
			messageType = MessageType.Info;
			Debug.Log(helpBoxMessage);
		}

		private void SaveLookupTexture(string relativePath, Texture2D lut)
		{
			string fullPath = Application.dataPath.Remove(Application.dataPath.Length - 6) + relativePath;
			byte[] textureBytes = lut.EncodeToPNG();
			
			//try { File.WriteAllBytes(fullPath, textureBytes); }
			//catch (Exception e)
			//{
			//	Debug.LogError("Error saving lookup texture: " + e.StackTrace);
			//	return;
			//}

			AssetDatabase.Refresh();
			TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);
			importer.textureType = TextureImporterType.Default;
			importer.spriteImportMode = SpriteImportMode.None;
			importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			importer.anisoLevel = 0;
			importer.mipmapEnabled = false;
			importer.maxTextureSize = lut.width;
			importer.linearTexture = true;
			importer.filterMode = FilterMode.Bilinear;
			importer.wrapMode = TextureWrapMode.Repeat;
			importer.isReadable = false;
			AssetDatabase.ImportAsset(relativePath);
			AssetDatabase.Refresh();
		}

		private string lensFlareColorPath = "LensColorTexture";
		
		private void DrawLensFlareColor()
		{			
			GUILayout.Space(20);
			GUILayout.Label("Lens Flare Color", EditorStyles.largeLabel);
			DrawLensFlareColorInput();
			DrawLensFlareColorButton();
		}
		
		private void DrawLensFlareColorInput()
		{
			lensFlareColorPath = EditorGUILayout.TextField(new GUIContent("File Name:"), lensFlareColorPath);
			EditorGUILayout.PropertyField(serializedGradient);
			thisSerialized.ApplyModifiedProperties();

//			GUI.changed = false;
//			targetPlayer.speed = EditorGUILayout.Slider ("Speed", targetPlayer.speed, 0, 100);
			//			if (GUI.changed)
		}
		
		private void DrawLensFlareColorButton()
		{
			GUILayout.Space(5);
			if (GUILayout.Button("Save Texture") == false) return;

			Texture2D lensColorTexture = new Texture2D(256, 1, TextureFormat.ARGB32, false);
			Color[] colors = new Color[lensColorTexture.width * lensColorTexture.height];
			for (int i = 0; i < colors.Length; i++)
			{
				float time = i / (float)(colors.Length-1.0f);
				colors[i] = gradient.Evaluate(time);
			}
			lensColorTexture.SetPixels(colors);
			lensColorTexture.Apply(false, false);

			SaveLensColorTexture(lensFlareColorPath + ".png", lensColorTexture);
		}
		
		private void SaveLensColorTexture(string relativePath, Texture2D lensColor)
		{
			string fullPath = Application.dataPath + "/" + relativePath;
			byte[] textureBytes = lensColor.EncodeToPNG();
			
			//try { File.WriteAllBytes(fullPath, textureBytes); }
			//catch (Exception e)
			//{
			//	Debug.LogError("Error saving lens color texture: " + e.StackTrace);
			//	return;
			//}

			relativePath = "Assets/" + relativePath;
			
			AssetDatabase.Refresh();
			TextureImporter importer 	= (TextureImporter)TextureImporter.GetAtPath(relativePath);
			importer.textureType 		= TextureImporterType.Default;
			importer.spriteImportMode 	= SpriteImportMode.None;
			importer.textureFormat 		= TextureImporterFormat.AutomaticCompressed;
			importer.anisoLevel 		= 0;
			importer.mipmapEnabled 		= false;
			importer.maxTextureSize 	= lensColor.width;
			importer.filterMode 		= FilterMode.Bilinear;
			importer.wrapMode 			= TextureWrapMode.Clamp;
			importer.isReadable 		= false;
			AssetDatabase.ImportAsset(relativePath);
			AssetDatabase.Refresh();

			Debug.Log ("Saved Lens Color Texture: " + fullPath);
		}
	}
}
#endif
