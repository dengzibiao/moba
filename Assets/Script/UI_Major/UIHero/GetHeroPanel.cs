using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 获取英雄面板
/// </summary>

public class GetHeroPanel : MonoBehaviour
{

    public static GetHeroPanel instance;


    UISprite border;                //边框
    UISprite typeS;                 //类型
    UISprite icon;                  //Icon
    UILabel nameLabel;              //名字
    UISprite mask;                  //遮罩

    //星星
    List<UISprite> startList = new List<UISprite>();

    public HeroNode hero;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {

        

        UIEventListener.Get(mask.gameObject).onClick += OnMaskClick;

        //Init();
    }

    /// <summary>
    /// 初始信息
    /// </summary>
    public void Init()
    {

        if (null == typeS)
        {
            border = UnityUtil.FindCtrl<UISprite>(gameObject, "Border");
            typeS = UnityUtil.FindCtrl<UISprite>(gameObject, "Type");
            icon = UnityUtil.FindCtrl<UISprite>(gameObject, "Icon");
            nameLabel = UnityUtil.FindCtrl<UILabel>(gameObject, "NameLabel");

            for (int i = 1; i <= 5; i++)
            {
                startList.Add(transform.Find("Star" + i).GetComponent<UISprite>());
            }

            mask = UnityUtil.FindCtrl<UISprite>(gameObject, "Mask");
        }

        switch (hero.attribute)
        {
            case 1:
                typeS.spriteName = "li";
                break;
            case 2:
                typeS.spriteName = "zhi";
                break;
            case 3:
                typeS.spriteName = "min";
                break;
        }

        icon.spriteName = hero.original_painting;

        nameLabel.text = hero.name;

        for (int i = 0; i < hero.init_star; i++)
        {
            startList[i].spriteName = "xingxing";
        }

        for (int i = hero.init_star; i < 5; i++)
        {
            startList[i].spriteName = "xing-hui";
        }

    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    /// <param name="go"></param>
    private void OnMaskClick(GameObject go)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public void ShowGetHeroPanel(HeroNode hero)
    {
        this.hero = hero;
        gameObject.SetActive(true);
    }

}