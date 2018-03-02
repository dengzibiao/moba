using UnityEngine;
using System.Collections;

public class ChangeColorGray
{
    // UISprite : 更换图集(正常和置灰图集切换)
    // UITexture：更换Shader为正常Shader和置灰使用的Shader

    private UIAtlas atlas = null;
    private Shader shader = null;

    private static ChangeColorGray instance = null;
    public static ChangeColorGray Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new ChangeColorGray();
            }
            return instance;
        }
    }

    /// <summary>
    /// 将UI整体
    /// 置灰操作
    /// </summary>
    public void ChangeUIToGray(GameObject go, bool isGray)
    {
        UISprite[] sprites = go.GetComponentsInChildren<UISprite>();
        UITexture[] textures = go.GetComponentsInChildren<UITexture>();

        for(int i = 0; i < sprites.Length; i++)
            ChangeSpriteColor(sprites[i], isGray);

        for(int i = 0; i < textures.Length; i++)
            ChangeTextureColor(textures[i], isGray);

        sprites = null;
        textures = null;
    }

    /// <summary>
    /// 将UISprite置灰操作
    /// </summary>
    public void ChangeSpriteColor(UISprite sp, bool isNormal)
    {
        //string n = sp.atlas.name;
        //n = n.Contains("Gray") ? n.Replace("Gray", "") : n;

        if(!isNormal)
        {
            //变灰
            //this.atlas = ResourceManager.Instance().GetUIAtlas(n + "Gray");
            sp.color = new Color(0f, 1f, 1f);
        }
        else
        {
            //正常
            //this.atlas = ResourceManager.Instance().GetUIAtlas(n);
            sp.color = new Color(1f, 1f, 1f);
        }

        //sp.atlas = atlas;
    }
    /// <summary>
    /// 将UITexture置灰操作
    /// </summary>
    public void ChangeTextureColor(UITexture texture, bool isGray)
    {

        if(!isGray)
        {
            //变灰
            this.shader = ResourceManager.Instance().GetShader("Unlit/ColorGray");
        }
        else
        {
            //正常
            this.shader = ResourceManager.Instance().GetShader("Unlit/" + texture.mainTexture.name);
        }

        texture.shader = shader;
    }

}
