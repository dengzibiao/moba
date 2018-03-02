using UnityEngine;
using System.Collections;

public class GoodsTips : MonoBehaviour
{

    public UISprite icon;
    public UISprite borderS;
    public new UILabel name;
    public UILabel des;
    public UILabel probability;
    public UILabel countL;
    public UISprite soulStone;

    public void RefreshUI(ItemNodeState item, int count)
    {
        gameObject.SetActive(true);
        ItemData.SetAngleMarking(soulStone, item.types);
        if (item.types == 6)
        {
            icon.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
        }
        else
        {
            icon.atlas = ResourceManager.Instance().GetUIAtlas("Prop");
        }
        icon.spriteName = item.icon_name;
        name.text = item.name;

        des.text = "可能获得";
        countL.text = "";
        borderS.spriteName = ItemData.GetFrameByGradeType((GradeType)item.grade);
    }

}