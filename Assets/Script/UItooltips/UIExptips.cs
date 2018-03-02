using UnityEngine;
using System.Collections;

public class UIExptips : GUIBase
{
    public GUISingleButton box;
    public GUISingleButton BackpackBtn;
    public GUISingleButton ShopBtn;

    protected override void Init()
    {
        ShopBtn.onClick = ShopBtnClock;
        box.onClick = boxBtnClock;
        BackpackBtn.onClick = BackpackBtnClock;
    }


    protected override void ShowHandler()
    {

    }
    void ShopBtnClock()
    {
        HeroPosEmbattle.instance.HideModel();
        //Control.HideGUI(GameLibrary.UIExptips);
        //Control.HideGUI(GameLibrary.UI_HeroDetail);
        //Control.ShowGUI(GameLibrary.UIShop);
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    void BackpackBtnClock()
    {
        HeroPosEmbattle.instance.HideModel();
        //Control.HideGUI(GameLibrary.UIExptips);
        //Control.HideGUI(GameLibrary.UI_HeroDetail);
        //Control.ShowGUI(GameLibrary.UIKnapsack);
    }
    void boxBtnClock()
    {

        //Control.HideGUI(GameLibrary.UIExptips);
    }
}
