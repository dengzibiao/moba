using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScreenAdaptor : MonoBehaviour
{
    public Vector2 _original;
    public RectTransform _rect;
    public bool isEnable = false;

    private Transform _myTransform;

    void Start()
    {
        _myTransform = this.gameObject.transform;
        Refersh();
    }

#if UNITY_EDITOR
    void Update()
    {
        Refersh();
    }
#endif

    private void Refersh()
    {
        if(_rect == null || _original.x == 0 || _original.y == 0)
        {
            return;
        }

        float scaleW = Screen.width / _original.x;
        float scaleH = Screen.height / _original.y;
        float scale, temp;
        float posX, posY;
        float sdx, sdy;

        if(isEnable)
        {
            if(scaleW < scaleH)
            {
                scale = scaleW;//缩小的比例

                temp = (1f - scale) * Screen.height;//1-scale：剩下的比例
                posX = (1f - scale) / 2 * Screen.width;
                posY = temp / 2f;

                sdx = _original.x - Screen.width;
                sdy = temp / scale;
            }
            else
            {
                scale = scaleH;

                temp = (1f - scale) * Screen.width;
                posY = (1f - scale) / 2 * Screen.height;
                posX = temp / 2f;

                sdy = _original.y - Screen.height;
                sdx = temp / scale;
            }
        }
        else
        {
            scale = 1;
            posX = 0;
            posY = 0;
            sdx = 0;
            sdy = 0;
        }

        _myTransform.localScale = new Vector3(scale, scale, 1);
        _myTransform.localPosition = new Vector3(posX, posY, 0);
        _rect.sizeDelta = new Vector2(sdx, sdy);
    }
}
