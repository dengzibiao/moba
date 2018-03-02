using UnityEngine;
using System.Collections;

public class SequenceFrame : MonoBehaviour
{
    //行，列，当前序列
    public int col = 0;
    public int row = 0;
    public int current = 0;

    //播放区段
    public int min = 0;
    public int max = 0;

    //是否播放动画
    public bool isAnim = false;

    //动画播放速度
    public int fps = 5;

    private Material mat;
    private float timer = 0;


    void Start()
    {
        this.mat = this.GetComponent<Renderer>().sharedMaterial;

        if(isAnim)
        {
            current = min;

            ResetTimer();
        }

        HandlerMaterial();
    }

    void Update()
    {
        if(isAnim)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                ResetTimer();
                current++;
                current = current % max;
                current = Mathf.Clamp(current, min, max);

                HandlerMaterial();
            }
        }
    }

    private void HandlerMaterial()
    {
        Vector2 framePosition = new Vector2();
        framePosition.x = current % col;
        framePosition.y = current / col;
        this.mat.SetTextureScale("_MainTex", new Vector2(1f / col, 1f / row));
        this.mat.SetTextureOffset("_MainTex", new Vector2(framePosition.x / col, 1 - (framePosition.y + 1) / row));
    }

    /// <summary>
    /// 计算播放间隔
    /// </summary>
    void ResetTimer()
    {
        timer = 1f / fps;
    }
}
