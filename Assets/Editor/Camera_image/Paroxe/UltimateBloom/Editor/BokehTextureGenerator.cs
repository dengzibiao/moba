using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

public class CameraDirtTextureGenerator : EditorWindow 
{


    Texture2D m_DestinationTexture = null;

    [SerializeField]
    int m_PassCount;

    
    [SerializeField]
    BokehGenerationPass[] m_BokehPasses = new BokehGenerationPass[5];

    [Serializable]
    class BokehGenerationPass
    {
        [SerializeField]
        public Texture2D m_BokehTexture;
        [SerializeField]
        public float m_MinimumSize = 100.0f;
        [SerializeField]
        public float m_MaximumSize = 200.0f;
        [SerializeField]
        public float m_Density = 0.1f;
        [SerializeField]
        public float m_MinIntensity = 0.1f;
        [SerializeField]
        public float m_MaxIntensity = 0.3f;
        [SerializeField]
        public float m_BlurRadius = 1;
        [SerializeField]
        public float m_VignettePower = 0.5f;
        [SerializeField]
        public float m_ChromaticAberration = 20.0f;
        [SerializeField]
        public float m_HueVariation = 0.3f;
        [SerializeField]
        public bool m_RandomRotation = false;
    }

    List<Vector3>[] m_Positions = new List<Vector3>[5];
    float[] m_MinimumDistance = new float[5];

    Vector2 m_ScrollPos;

    [MenuItem("Window/Bokeh Texture Generator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CameraDirtTextureGenerator));
    }


    GUIStyle m_Background1;
    GUIStyle m_Background2;
    GUIStyle m_Background3;
    GUIStyle m_Background4;
    void OnEnable()
    {
        m_Background1 = new GUIStyle();
        m_Background1.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.1f));
        m_Background2 = new GUIStyle();
        m_Background2.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.0f));
        m_Background3 = new GUIStyle();
        m_Background3.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.05f));
        m_Background4 = new GUIStyle();
        m_Background4.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 0.0f, 1.0f));

        
    }

    void OnDisable()
    {
        GameObject.DestroyImmediate(m_Background1.normal.background);
        GameObject.DestroyImmediate(m_Background2.normal.background);
        GameObject.DestroyImmediate(m_Background3.normal.background);
        GameObject.DestroyImmediate(m_Background4.normal.background);
    }

    void CreateAndSetTexture()
    {
        Texture2D tmp = new Texture2D(m_Width, m_Height);
        var bytes = tmp.EncodeToPNG();
        File.WriteAllBytes("Assets/UB_BokehTexture.png", bytes);
        AssetDatabase.ImportAsset("Assets/UB_BokehTexture.png");

        m_DestinationTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/UB_BokehTexture.png", typeof(Texture2D));
    }

    int m_Width = 1280;
    int m_Height = 720;

    void OnGUI()
    {
        Undo.RecordObject(this, "Lens Bokeh Texture Generator");

        GUILayout.Label("SETTINGS", EditorStyles.boldLabel);

        m_DestinationTexture = (Texture2D)EditorGUILayout.ObjectField("Destination Texture", m_DestinationTexture, typeof(Texture2D), false);

        if (m_DestinationTexture == null)
        {
            GUILayout.Label("Please set or create a destination texture.");
            GUILayout.BeginHorizontal();
            m_Width = Mathf.Clamp(EditorGUILayout.IntField("Width", m_Width),10,  4096);
            m_Height = Mathf.Clamp(EditorGUILayout.IntField("Height", m_Height), 10, 4096);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Create and set Texture"))
            {
                CreateAndSetTexture();
            }
        }
        else
        {
            m_PassCount = Mathf.Clamp(EditorGUILayout.IntField("Layer Count", m_PassCount), 1, 5);
            GUILayout.Space(20.0f);
            GUILayout.Label("LAYERS", EditorStyles.boldLabel);
            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos);
            for (int i = 0; i < m_PassCount; ++i)
            {
                GUILayout.BeginVertical(i % 2 == 0 ? m_Background3 : m_Background1);
                BokehGenerationPass p = m_BokehPasses[i];
                p.m_BokehTexture = (Texture2D)EditorGUILayout.ObjectField("Sprite Texture", p.m_BokehTexture, typeof(Texture2D), false);
                p.m_MinimumSize = DoSlider("Minimum Size", p.m_MinimumSize, 20.0f, 400.0f);
                p.m_MaximumSize = DoSlider("Maximum Size", p.m_MaximumSize, 20.0f, 400.0f);
                p.m_BlurRadius = DoSlider("Blur Radius", p.m_BlurRadius, 0.0f, 10.0f);
                p.m_VignettePower = DoSlider("Vignette Power", p.m_VignettePower, 0.0f, 10.0f);
                p.m_HueVariation = DoSlider("Hue Variation", p.m_HueVariation, 0.0f, 0.8f);
                p.m_MinIntensity = DoSlider("Minimum Intensity", p.m_MinIntensity, 0.0f, 1.0f);
                p.m_MaxIntensity = DoSlider("Maximum Intensity", p.m_MaxIntensity, 0.0f, 1.0f);
                p.m_Density = DoSlider("Density", p.m_Density, 0.0f, 1.0f);
                p.m_ChromaticAberration = DoSlider("Chromatic Aberration", p.m_ChromaticAberration, 0.0f, 50.0f);
                p.m_RandomRotation = EditorGUILayout.Toggle("Random Rotation", p.m_RandomRotation);
                GUILayout.Space(20.0f);
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();




            if (GUILayout.Button("Generate Bokeh Texture"))
            {
                GenerateTexture();
            }

            if (m_DestinationTexture != null)
            {
                GUILayout.BeginVertical(m_Background4);

                float w = m_DestinationTexture.width;
                if (m_DestinationTexture.width > 300.0f)
                    w = 300.0f;

                float h = ((float)m_DestinationTexture.height / (float)m_DestinationTexture.width) * w;

                Rect r = GUILayoutUtility.GetRect(w, h);
                

                r.position = new Vector2(r.width * 0.5f - w*0.5f, r.position.y);

                r.height = h;
                r.width = w;

                //Debug.Log("r=" + h);
                EditorGUI.DrawPreviewTexture(r, m_DestinationTexture);

                GUILayout.EndVertical();
            }

            
        }

        

        if (GUI.changed)
        {
            EditorUtility.SetDirty(this);
        }
    }

    float DoSlider(string label, float value, float min, float max)
    {
        float v = value;
        EditorGUILayout.BeginHorizontal();
        v = Mathf.Clamp(EditorGUILayout.FloatField(label, v), min, max);
        v = GUILayout.HorizontalSlider(v, min, max);
        EditorGUILayout.EndHorizontal();

        return v;
    }

    Material m_MixerMaterial = null;

    void GenerateTexture()
    {
        for (int i = 0; i < m_Positions.Length; ++i)
        {
            m_Positions[i] = new List<Vector3>();
            m_MinimumDistance[i] = 1.0f;
        }

        Material bokehMaterial = new Material(Shader.Find("Hidden/Ultimate/BokehTexture"));
        bokehMaterial.hideFlags = HideFlags.HideAndDontSave;

        Material blurMaterial = new Material(Shader.Find("Hidden/Ultimate/Sampling"));
        blurMaterial.hideFlags = HideFlags.HideAndDontSave;

        Material miscMaterial = new Material(Shader.Find("Hidden/Ultimate/BokehMisc"));
        miscMaterial.hideFlags = HideFlags.HideAndDontSave;

        Material mixerMaterial = new Material(Shader.Find("Hidden/Ultimate/BloomMixer"));
        mixerMaterial.hideFlags = HideFlags.HideAndDontSave;

        m_MixerMaterial = mixerMaterial;

        RenderTexture accumulation = null;
        for (int i = 0; i < m_PassCount; ++i)
        {
            RenderTexture current = GenerateTexture(i, bokehMaterial, blurMaterial, miscMaterial, m_DestinationTexture.width, m_DestinationTexture.height, m_BokehPasses[i]);
            if (accumulation == null && current != null)
                accumulation = current;
            else if (current != null)
            {
                RenderTextureAdditive(current, accumulation, 1.0f);
                RenderTexture.ReleaseTemporary(current);
            }
        }


        string path = AssetDatabase.GetAssetPath(m_DestinationTexture);

        Texture2D newTexture = new Texture2D(m_DestinationTexture.width, m_DestinationTexture.height);

        RenderTexture.active = accumulation;
        newTexture.ReadPixels(new Rect(0, 0, newTexture.width, newTexture.height), 0, 0);
        newTexture.Apply();

        var bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);

        GameObject.DestroyImmediate(bokehMaterial);
        GameObject.DestroyImmediate(miscMaterial);
        GameObject.DestroyImmediate(mixerMaterial);

        
    }

    private RenderTexture GenerateTexture(int idx, Material material, Material blurMaterial, Material miscMaterial,int width, int height, BokehGenerationPass p)
    {
        RenderTexture rtA = RenderTexture.GetTemporary(width, height);
        //RenderTexture lastActive = RenderTexture.active;

        material.mainTexture = p.m_BokehTexture;

        RenderTexture.active = rtA;
        GL.Clear(true, true, Color.black);
        

        //GL.LoadPixelMatrix(0,width,0,height);
        //GL.LoadPixelMatrix(0, width, 0, height);
        Matrix4x4 proj = Matrix4x4.Ortho(0, width, 0, height, -1.0f, 1.0f);

        material.SetMatrix("_MeshProjectionMatrix", proj);

        int nbPixel = width * height;
        int nbQuad = (int)(p.m_Density * 0.0004f * nbPixel);
        material.SetFloat("_Intensity", UnityEngine.Random.Range( p.m_MinIntensity, p.m_MaxIntensity) );

        float cellSize = Mathf.Max( Mathf.Sqrt(nbQuad), 0.1f);

        float xStep = (float)width / cellSize;
        float yStep = (float)height / cellSize;

        for (float i = 0; i < width; i += xStep)
        {
            for (float j = 0; j < height; j += yStep)
            {

                Color tint = new Color(UnityEngine.Random.Range(1.0f - p.m_HueVariation, 1.0f), UnityEngine.Random.Range(1.0f - p.m_HueVariation, 1.0f), UnityEngine.Random.Range(1.0f - p.m_HueVariation, 1.0f));
                material.SetColor("_Tint", tint);
                material.SetPass(0);
                float size = UnityEngine.Random.Range(p.m_MinimumSize, p.m_MaximumSize);

                //float x = UnityEngine.Random.Range(-1f, 1f);
                //float y = UnityEngine.Random.Range(-1f, 1f);


                float x = (i + UnityEngine.Random.Range(0, xStep)) / width * 2.0f - 1.0f;
                float y = (j + UnityEngine.Random.Range(0, yStep)) / height * 2.0f - 1.0f;

                float dist = new Vector2(x, y).magnitude;
                float sizeMul = Mathf.Min(Mathf.Pow(dist, p.m_VignettePower), 1.0f);


                float s = sizeMul > 0.5f ? size * sizeMul : 0.0f;
                float angle = UnityEngine.Random.Range(0.0f, p.m_RandomRotation ? 360.0f : 0.0f);
                Vector3 nDiv = new Vector3(1.0f / width, 1.0f / height, 0.0f);
                Vector3 tl = RotateZ(new Vector3(-s, -s, 0.0f), angle);
                Vector3 tr = RotateZ(new Vector3(s, -s, 0f), angle);
                Vector3 br = RotateZ(new Vector3(s, s, 0f), angle);
                Vector3 bl = RotateZ(new Vector3(-s, s, 0f), angle);

                tl = new Vector3(tl.x * nDiv.x, tl.y * nDiv.y, 0.0f);
                tr = new Vector3(tr.x * nDiv.x, tr.y * nDiv.y, 0.0f);
                br = new Vector3(br.x * nDiv.x, br.y * nDiv.y, 0.0f);
                bl = new Vector3(bl.x * nDiv.x, bl.y * nDiv.y, 0.0f);


                DrawQuad(x, y, tl, tr, br, bl);

            }
        }

            if (material.SetPass(0))
            {

                /*for (int i = 0; i < nbQuad; ++i)
                {
                    Color tint = new Color(UnityEngine.Random.Range(1.0f - p.m_HueVariation, 1.0f), UnityEngine.Random.Range(1.0f - p.m_HueVariation, 1.0f), UnityEngine.Random.Range(1.0f - p.m_HueVariation, 1.0f));
                    material.SetColor("_Tint", tint);
                    material.SetPass(0);
                    float size = UnityEngine.Random.Range(p.m_MinimumSize, p.m_MaximumSize);

                    //float x = UnityEngine.Random.Range(-1f, 1f);
                    //float y = UnityEngine.Random.Range(-1f, 1f);

                    Vector3 pos = PickPosition(idx);
                    float x = pos.x;
                    float y = pos.y;

                    float dist = new Vector2(x, y).magnitude;
                    float sizeMul = Mathf.Min(Mathf.Pow(dist, p.m_VignettePower), 1.0f);


                    float s = sizeMul > 0.5f ? size * sizeMul : 0.0f;
                    float angle = UnityEngine.Random.Range(0.0f, p.m_RandomRotation? 360.0f : 0.0f);
                    Vector3 nDiv = new Vector3(1.0f / width, 1.0f / height, 0.0f);
                    Vector3 tl = RotateZ(new Vector3(-s, -s, 0.0f), angle);
                    Vector3 tr = RotateZ(new Vector3(s, -s, 0f), angle);
                    Vector3 br = RotateZ(new Vector3(s, s, 0f), angle);
                    Vector3 bl = RotateZ(new Vector3(-s, s, 0f), angle);

                    tl = new Vector3(tl.x * nDiv.x, tl.y * nDiv.y, 0.0f);
                    tr = new Vector3(tr.x * nDiv.x, tr.y * nDiv.y, 0.0f);
                    br = new Vector3(br.x * nDiv.x, br.y * nDiv.y, 0.0f);
                    bl = new Vector3(bl.x * nDiv.x, bl.y * nDiv.y, 0.0f);


                    DrawQuad(x, y, tl , tr , br , bl );
                }*/

            }

        RenderTexture rtB = RenderTexture.GetTemporary(width, height);
        for (int i = 0; i < 1; ++i)
        {
            RenderTexture.active = rtB;
            blurMaterial.SetTexture("_AdditiveTexture", Texture2D.blackTexture);
            blurMaterial.SetVector("_OffsetInfos", new Vector4(1.0f / width * p.m_BlurRadius, 0, 0, 0));
            blurMaterial.SetVector("_Tint", Color.white);
            blurMaterial.SetFloat("_Intensity", 1.0f);
            Graphics.Blit(rtA, rtB, blurMaterial, 2);

            blurMaterial.SetVector("_OffsetInfos", new Vector4(0, 1.0f / height * p.m_BlurRadius, 0, 0));
            Graphics.Blit(rtB, rtA, blurMaterial, 4);
        }

        // Chromatic Aberration
        miscMaterial.SetFloat("_ChromaticAberration", p.m_ChromaticAberration);
        Graphics.Blit(rtA, rtB, miscMaterial, 0);

        /*RenderTexture.active = rtB;
        Texture2D texture = new Texture2D(width, height);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();*/

        RenderTexture.ReleaseTemporary(rtA);
        //RenderTexture.ReleaseTemporary(rtB);
        //RenderTexture.active = lastActive;

        return rtB;
    }

    Vector3 PickPosition(int idx)
    {
        float minDistance = m_MinimumDistance[idx];
        List<Vector3> positions = m_Positions[idx];

        int nbTry = 20000;
        bool ok = false;

        while (!ok)
        {
            float x = UnityEngine.Random.Range(-1f, 1f);
            float y = UnityEngine.Random.Range(-1f, 1f);

            Vector3 pos = new Vector3(x, y, 0.0f);

            bool foundNearPos = false;
            foreach (Vector3 cPos in positions)
            {
                if (Vector3.Distance(cPos, pos) < minDistance)
                {
                    minDistance -= 0.1f;
                    m_MinimumDistance[idx] = minDistance;
                    foundNearPos = true;
                    break;
                }
            }

            if (!foundNearPos)
            {
                return pos;
            }

            nbTry--;
            if (nbTry < 0)
                break;
        }


       
        return Vector3.zero;
    }

    public Vector3 RotateZ(Vector3 v, float angle)
    {
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);

        float tx = v.x;
        float ty = v.y;

        Vector3 r = new Vector3();
        r.x = (cos * tx) - (sin * ty);
        r.y = (cos * ty) + (sin * tx);

        return r;
    }

    void DrawQuad(float x, float y, Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl)
    {


        GL.Begin(GL.QUADS); // Quad

        GL.MultiTexCoord2(0, 1 - 0f, 0f);
        GL.Vertex3(tl.x + x, tl.y + y, 0f);

        GL.MultiTexCoord2(0, 1 - 1f, 0f);
        GL.Vertex3(tr.x + x, tr.y + y, 0f);

        GL.MultiTexCoord2(0, 1 - 1f, 1f);
        GL.Vertex3(br.x + x, br.y + y, 0f);

        GL.MultiTexCoord2(0, 1 - 0f, 1f);
        GL.Vertex3(bl.x + x, bl.y + y, 0f);

        GL.End();
    }

   /* void DrawQuad(float x, float y, float halfWidth, float halfHeight)
    {

        float angle = UnityEngine.Random.Range(0.0f, 360.0f);


        Vector3 tl = RotateZ(new Vector3(-halfWidth, -halfHeight, 0.0f), 0.0f);
        Vector3 tr = RotateZ( new Vector3(halfWidth, -halfHeight, 0f), 0.0f);
        Vector3 br = RotateZ(new Vector3(halfWidth, halfHeight, 0f), 0.0f);
        Vector3 bl = RotateZ(new Vector3(-halfWidth, halfHeight, 0f), 0.0f); 

        GL.Begin(GL.QUADS); // Quad

        GL.MultiTexCoord2(0, 1-0f, 0f);
        GL.Vertex3(tl.x + x, tl.y + y, 0f);

        GL.MultiTexCoord2(0, 1 - 1f, 0f);
        GL.Vertex3(tr.x + x, tr.y + y, 0f);

        GL.MultiTexCoord2(0, 1 - 1f, 1f);
        GL.Vertex3(br.x + x, br.y + y, 0f);

        GL.MultiTexCoord2(0, 1 - 0f, 1f);
        GL.Vertex3(bl.x + x, bl.y + y, 0f);

        GL.End();
    }*/


    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.hideFlags = HideFlags.HideAndDontSave;
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    void RenderTextureAdditive(RenderTexture source, RenderTexture destination, float intensity)
    {
        RenderTexture tmpTexture = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
        Graphics.Blit(destination, tmpTexture);

        m_MixerMaterial.SetTexture("_ColorBuffer", tmpTexture);
        m_MixerMaterial.SetFloat("_Intensity", intensity);

        Graphics.Blit(source, destination, m_MixerMaterial, 0);

        RenderTexture.ReleaseTemporary(tmpTexture);
    }

}
