using UnityEngine;

public class AccountItem : MonoBehaviour
{

    public UISprite border;
    public UISprite icon;
    public UILabel lvl;
    public UISlider exp;
    public UILabel itemName;

    ItemNodeState item;
    HeroData heroData;

    public void RefreshUIHero (long heroId, int addEXP)
    {
        exp.gameObject.SetActive(true);
        itemName.gameObject.SetActive(false);

        if (heroId == 0) return;
        heroData = playerData.GetInstance().GetHeroDataByID(heroId);
        if (null == heroData) return;

        lvl.text = heroData.lvl + "级";
        icon.spriteName = heroData.node.icon_name;
        exp.value = (float)heroData.exps / heroData.maxExps;
        exp.transform.FindChild("expLabel").GetComponent<UILabel>().text = "+" + addEXP;
        border.spriteName = playerData.GetInstance().GetHeroGrade(heroData.grade);
    }

    public void RefreshUIItem(long id, int count)
    {
        exp.gameObject.SetActive(false);
        itemName.gameObject.SetActive(true);

        if (!GameLibrary.Instance().ItemStateList.TryGetValue(id, out item)) return;

        if (item.types == 6)
            icon.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
        else
            icon.atlas = ResourceManager.Instance().GetUIAtlas("Prop");

        lvl.text = "" + count;
        icon.spriteName = item.icon_name;
        itemName.text = item.name;
        border.spriteName = ItemData.GetFrameByGradeType((GradeType)item.grade);
    }

}
