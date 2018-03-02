using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class BokehRenderer
{
    Texture2D m_CurrentTexture;
    //Mesh[] m_FlareMeshes = null;
    Material m_FlareMaterial;

    int m_CurrentWidth;
    int m_CurrentHeight;
    float m_CurrentRelativeScaleX;
    float m_CurrentRelativeScaleY;

    public BokehRenderer()
    {
        
    }

    public void RebuildMeshIfNeeded(int width, int height, float spriteRelativeScaleX, float spriteRelativeScaleY, ref Mesh[] meshes)
    {
        if (m_CurrentWidth == width && m_CurrentHeight == height && m_CurrentRelativeScaleX == spriteRelativeScaleX && m_CurrentRelativeScaleY == spriteRelativeScaleY && meshes != null)
            return;

        if (meshes != null)
            foreach (Mesh m in meshes)
            {
                GameObject.DestroyImmediate(m, true);
            }
        meshes = null;

        BuildMeshes(width, height, spriteRelativeScaleX, spriteRelativeScaleY, ref meshes);

    }

    public void BuildMeshes(int width, int height, float spriteRelativeScaleX, float spriteRelativeScaleY, ref Mesh[] meshes)
    {
        int maxQuads = 65000 / 6;
        int totalQuads = width * height;
        int meshCount = Mathf.CeilToInt((1.0f * totalQuads) / (1.0f * maxQuads));
        meshes = new Mesh[meshCount];
        int currentQuads = totalQuads;


        m_CurrentWidth = width;
        m_CurrentHeight = height;
        m_CurrentRelativeScaleX = spriteRelativeScaleX;
        m_CurrentRelativeScaleY = spriteRelativeScaleY;
        int currentPixel = 0;

        for (int m = 0; m < meshCount; ++m)
        {
            Mesh currentMesh = new Mesh();
            currentMesh.hideFlags = HideFlags.HideAndDontSave;

            int nbQuads = currentQuads;
            if (currentQuads > maxQuads)
                nbQuads = maxQuads;
            currentQuads -= nbQuads;

            Vector3[] vertices = new Vector3[nbQuads * 4];
            int[] triangles = new int[nbQuads * 6];
            Vector2[] uv0 = new Vector2[nbQuads * 4];
            Vector2[] uv1 = new Vector2[nbQuads * 4];
            Vector3[] normals = new Vector3[nbQuads * 4];
            Color[] colors = new Color[nbQuads * 4];

            float spriteWidth = m_CurrentRelativeScaleX * width;
            float spriteHeigth = m_CurrentRelativeScaleY * height;


            for (int i = 0; i < nbQuads; ++i)
            {
                int x = currentPixel % width;
                int y = (currentPixel - x) / width;
                SetupSprite(i, x, y, vertices, triangles, uv0, uv1, normals, colors, new Vector2((float)x / (float)width, 1.0f - ((float)y / (float)height)), spriteWidth * 0.5f, spriteHeigth * 0.5f);
                currentPixel++;
            }

            currentMesh.vertices = vertices;
            currentMesh.triangles = triangles;
            currentMesh.colors = colors;
            currentMesh.uv = uv0;
            currentMesh.uv2 = uv1;
            currentMesh.normals = normals;
            currentMesh.RecalculateBounds();
            currentMesh.UploadMeshData(true);
            meshes[m] = currentMesh;
        }
    }

    public void Clear(ref Mesh[] meshes)
    {
        if (meshes != null)
            foreach (Mesh m in meshes)
            {
                GameObject.DestroyImmediate(m, true);
            }
        meshes = null;
    }

    public void SetTexture(Texture2D texture)
    {
        m_CurrentTexture = texture;
        m_FlareMaterial.SetTexture("_MainTex", m_CurrentTexture);
    }

    public void SetMaterial(Material flareMaterial)
    {
        m_FlareMaterial = flareMaterial;
    }

    public void RenderFlare(RenderTexture brightPixels, RenderTexture destination, float intensity, ref Mesh[] meshes)
    {

        RenderTexture lastActive = RenderTexture.active;

        RenderTexture.active = destination;
        GL.Clear(true, true, Color.black);

        Matrix4x4 proj = Matrix4x4.Ortho(0, m_CurrentWidth, 0, m_CurrentHeight, -1.0f, 1.0f);

        m_FlareMaterial.SetMatrix("_FlareProj", proj);
        m_FlareMaterial.SetTexture("_BrightTexture", brightPixels);
        m_FlareMaterial.SetFloat("_Intensity", intensity);

        if (m_FlareMaterial.SetPass(0))
        {
            //Debug.Log("MeshCount=" + m_FlareMeshes.Length);

            for (int i = 0; i < meshes.Length; ++i )
                Graphics.DrawMeshNow(meshes[i], Matrix4x4.identity);
        }
        else
        {
            Debug.LogError("Can't render flare mesh");
        }

        RenderTexture.active = lastActive;

    }

    public void SetupSprite(int idx, int x, int y, Vector3[] vertices, int[] triangles, Vector2[] uv0, Vector2[] uv1, Vector3[] normals, Color[] colors, Vector2 targetPixelUV, float halfWidth, float halfHeight)
    {
        int vIdx = idx * 4;
        int tIdx = idx * 6;

        triangles[tIdx + 0] = vIdx + 0;
        triangles[tIdx + 1] = vIdx + 2;
        triangles[tIdx + 2] = vIdx + 1;

        triangles[tIdx + 3] = vIdx + 2;
        triangles[tIdx + 4] = vIdx + 3;
        triangles[tIdx + 5] = vIdx + 1;

        vertices[vIdx + 0] = new Vector3((-halfWidth + x), (-halfHeight + y), 0);
        vertices[vIdx + 1] = new Vector3((halfWidth + x), (-halfHeight + y), 0);
        vertices[vIdx + 2] = new Vector3((-halfWidth + x), (halfHeight + y), 0);
        vertices[vIdx + 3] = new Vector3((halfWidth + x), (halfHeight + y), 0);

        Vector2 p = targetPixelUV;

        colors[vIdx + 0] = new Color((-halfWidth / m_CurrentWidth + p.x), (-halfHeight*-1/ m_CurrentHeight + p.y), 0, 0);
        colors[vIdx + 1] = new Color((halfWidth / m_CurrentWidth + p.x), (-halfHeight * -1 / m_CurrentHeight + p.y), 0, 0);
        colors[vIdx + 2] = new Color((-halfWidth / m_CurrentWidth + p.x), (halfHeight * -1 / m_CurrentHeight + p.y), 0, 0);
        colors[vIdx + 3] = new Color((halfWidth / m_CurrentWidth + p.x), (halfHeight * -1 / m_CurrentHeight + p.y), 0, 0);

        normals[vIdx + 0] = -Vector3.forward;
        normals[vIdx + 1] = -Vector3.forward;
        normals[vIdx + 2] = -Vector3.forward;
        normals[vIdx + 3] = -Vector3.forward;

        uv0[vIdx + 0] = new Vector2(0, 0);
        uv0[vIdx + 1] = new Vector2(1.0f, 0);
        uv0[vIdx + 2] = new Vector2(0, 1.0f);
        uv0[vIdx + 3] = new Vector2(1.0f, 1.0f);

        uv1[vIdx + 0] = targetPixelUV;
        uv1[vIdx + 1] = targetPixelUV;
        uv1[vIdx + 2] = targetPixelUV;
        uv1[vIdx + 3] = targetPixelUV;
    }

    

}

