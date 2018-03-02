using UnityEngine;
using System.Collections;

public class HeroInfoData : MonoBehaviour
{

    UIButton introductionBtn;

    void Start()
    {
        introductionBtn = UnityUtil.FindCtrl<UIButton>(gameObject, "IntroductionBtn");

        EventDelegate.Set(introductionBtn.onClick, OnIntroductionBtn);

    }

    void Update()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    void OnIntroductionBtn()
    {
        print(1);
    }

}