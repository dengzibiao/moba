using UnityEngine;
using System.Collections;
public class WaterCamera : MonoBehaviour
{

    public Camera waterCamera;
    Material m;
    private RenderTexture mRenderTexture;
    void Start()
    {
        m = GetComponent<MeshRenderer>().material;
        mRenderTexture = waterCamera.targetTexture;
        m.SetTexture("_WaterTexture", mRenderTexture);
    }
}