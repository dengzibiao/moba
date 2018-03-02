using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AdvancedPanel : MonoBehaviour
{

    UIButton closeBtn;
    UILabel advFront;
    UILabel advPost;
    UILabel addNumber;
    UISprite borderFront;
    UISprite borderPost;
    UISprite iconFront;
    UISprite iconPost;

    UILabel nameFront;
    UILabel namePost;
    UILabel levelFront;
    UILabel levelPost;
    UILabel qualityFront;
    UILabel qualityPost;


    List<UISprite> starFront = new List<UISprite>();
    List<UISprite> starPost = new List<UISprite>();

    PlayerInfo play = null;

    HeroNode node;

    bool isOne = false;

    void Awake()
    {
        closeBtn = transform.Find("CloseBtn").GetComponent<UIButton>();
        borderFront = transform.Find("AdvancedFront/Border").GetComponent<UISprite>();
        borderPost = transform.Find("AdvancedPost/Border").GetComponent<UISprite>();
        iconFront = transform.Find("AdvancedFront/Icon").GetComponent<UISprite>();
        iconPost = transform.Find("AdvancedPost/Icon").GetComponent<UISprite>();

        for (int i = 1; i <= 5; i++)
        {
            starFront.Add(transform.Find("AdvancedFront/Star" + i).GetComponent<UISprite>());
            starPost.Add(transform.Find("AdvancedPost/Star" + i).GetComponent<UISprite>());
        }

        nameFront = transform.Find("AdvancedFront/NameLabel").GetComponent<UILabel>();
        namePost = transform.Find("AdvancedPost/NameLabel").GetComponent<UILabel>();
        levelFront = transform.Find("AdvancedFront/LevelLabel").GetComponent<UILabel>();
        levelPost = transform.Find("AdvancedPost/LevelLabel").GetComponent<UILabel>();
        qualityFront = transform.Find("AdvancedFront/QualityLabel").GetComponent<UILabel>();
        qualityPost = transform.Find("AdvancedPost/QualityLabel").GetComponent<UILabel>();

    }


    void Start()
    {
        UIEventListener.Get(closeBtn.gameObject).onClick += OnCloseBtn;

        Globe.heroInfoDic.TryGetValue((int)Globe.selectHero.hero_id, out play);

        Globe.allHeroDic.TryGetValue(Globe.selectHero.hero_id, out node);

        //for (int i = 0; i < vo.init_star; i++)
        //{
        //    starFront[i].enabled = true;
        //    starPost[i].enabled = true;
        //} 

        //for (int i = vo.init_star; i < starFront.Count; i++)
        //{
        //    starFront[i].enabled = false;
        //    starPost[i].enabled = false;
        //}

        //iconFront.spriteName = Globe.selectHero.image;
        //iconPost.spriteName = Globe.selectHero.image;

        //nameFront.text = Globe.selectHero.name;
        //namePost.text = Globe.selectHero.name;

        //levelFront.text = "Lv." + play.Level;
        //levelPost.text = "Lv." + play.Level;


        UpdateInfo();

        UpdateBorder(borderFront, qualityFront);

        play.Quality++;

        UpdateBorder(borderPost, qualityPost);

        isOne = true;

    }

    void OnEnable()
    {
        if (isOne)
        {

            Globe.heroInfoDic.TryGetValue(Globe.selectHero.hero_id, out play);

            Globe.allHeroDic.TryGetValue(Globe.selectHero.hero_id, out node);

            UpdateInfo();

            UpdateBorder(borderFront, qualityFront);

            play.Quality++;

            UpdateBorder(borderPost, qualityPost);
        }
    }

    /// <summary>
    /// 更新数据信息
    /// </summary>
    void UpdateInfo()
    {

        print(node.name);

        for (int i = 0; i < node.init_star; i++)
        {
            starFront[i].enabled = true;
            starPost[i].enabled = true;
        }

        for (int i = node.init_star; i < starFront.Count; i++)
        {
            starFront[i].enabled = false;
            starPost[i].enabled = false;
        }

       iconFront.spriteName = node.original_painting;
       iconPost.spriteName = node.original_painting;

        nameFront.text = node.name;
        namePost.text = node.name;

        levelFront.text = "Lv." + play.Level;
        levelPost.text = "Lv." + play.Level;
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    /// <param name="go"></param>
    private void OnCloseBtn(GameObject go)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 更新边框
    /// </summary>
    void UpdateBorder(UISprite border, UILabel quality)
    {

        if (play.Quality == 0)
        {
            border.spriteName = "baikuang";
            quality.text = "";
        }
        else if (play.Quality == 1 || play.Quality == 2)
        {
            border.spriteName = "lvkuang";
            quality.text = "+" + (play.Quality - 1);
        }
        else if (play.Quality == 3 || play.Quality == 4 || play.Quality == 5)
        {
            border.spriteName = "lankuang";
            quality.text = "+" + (play.Quality - 3);
        }
        else if (play.Quality == 6 || play.Quality == 7 || play.Quality == 8 || play.Quality == 9)
        {
            border.spriteName = "zikuang";
            quality.text = "+" + (play.Quality - 6);
        }
        else if (play.Quality == 10 || play.Quality == 11 || play.Quality == 12 || play.Quality == 13)
        {
            border.spriteName = "chengkuang";
            quality.text = "+" + (play.Quality - 10);
        }
        else if (play.Quality == 14 || play.Quality == 15 || play.Quality == 16)
        {
            border.spriteName = "hongkuang";
            quality.text = "+" + (play.Quality - 14);
        }

        if (play.Quality == 1 || play.Quality == 3 || play.Quality == 6 || play.Quality == 10 || play.Quality == 14)
        {
            quality.text = "";
        }

    }


}