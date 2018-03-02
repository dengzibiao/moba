using UnityEngine;
using System.Collections;

public class AdaptSize : MonoBehaviour
{

    /// <summary>
    /// 调用方法：Camera.main.fieldOfView = getCameraFOV(60);
    /// 实现3D物体的自适应显示
    /// </summary>
    /// <param name="currentFOV">
    /// 当前摄像机范围
    /// </param>
    /// <returns></returns>
    static public float GetCameraFOV(float currentFOV)
    {
        UIRoot root = GameObject.FindObjectOfType<UIRoot>();
        float scale = System.Convert.ToSingle(root.manualHeight / 640f);
        return currentFOV * scale;
    }

    /// <summary>
    /// 调用方法：Camera.main.orthographicSize = UIStretch_3D.getCameraSize(6.1f);
    /// 实现2D图片的自适应显示
    /// </summary>
    /// <param name="currentSize">
    /// 当前摄像机尺寸
    /// </param>
    /// <returns></returns>
    static public float GetCameraSize(float currentSize)
    {
        float rate = 960f / 640f;
        float scale = System.Convert.ToSingle(Screen.height) / Screen.width / rate;
        return currentSize * scale;
    }

    /// <summary>
    /// 设置UI界面的自适应缩放，已经锚点的配合，才可完成
    /// </summary>
    /// <returns></returns>
    static public void SetUISize(int sw, int sh)
    {
        int ManualWidth = sw;
        int ManualHeight = sh;
        UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot>();

        if(uiRoot != null)
        {
            if(System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
            {
                uiRoot.manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
            }
            else
            {
                uiRoot.manualHeight = ManualHeight;
            }
        }
    }
}
