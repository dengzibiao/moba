/*
文件名（File Name）:   GUISingleSpriteGroup.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class GUISingleSpriteGroup : GUIComponentBase
{

    private GUISingleSprite[] sprites;
    protected override void Init()
    {
        base.Init();

        this.sprites = this.GetComponentsInChildren<GUISingleSprite>(true);
    }

    public GUISingleSprite[] GetSpriteList()
    {
        return sprites;
    }
    /// <summary>
    /// 显示方法,显示当前个数隐藏其他
    /// </summary>
    /// <param name="count">个数</param>
    public void IsShow(int count)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (i + 1 <= count)
            {
                sprites[i].Show();
            }
            else
            {
                sprites[i].Hide();
            }

        }
    }
    /// <summary>
    /// 显示方法主要用星星显示带底图的
    /// </summary>
    /// <param name="bgSpriteName">灰色</param>
    /// <param name="changeName">亮色</param>
    /// <param name="count">当前星级</param>
    public void IsShow(string bgSpriteName, string changeName, int count)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (i + 1 <= count)
            {
                sprites[i].spriteName = changeName;
            }
            else
            {
                sprites[i].spriteName = bgSpriteName;
            }
        }
    }
    /// <summary>
    /// 显示和替换的方法
    /// </summary>
    /// <param name="bgSpriteName">默认图片</param>
    /// <param name="changeName">需要更改的图片</param>
    /// <param name="index">当前应改变的index</param>
    /// <param name="count">提前可多放置，想显示图片的总数，如果页数增加了也不怕</param>
    public void IsShow(string bgSpriteName, string changeName, int index, int count)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (i + 1 <= count)
            {
                sprites[i].Show();
                if (i == index)
                {
                    sprites[i].spriteName = changeName;
                }
                else
                {
                    sprites[i].spriteName = bgSpriteName;
                }
            }
            else
            {
                sprites[i].Hide();
            }
        }

    }
}
