using UnityEngine;

public class MobaKoInfoItem : MonoBehaviour 
{
    public UISprite SpHead;
    public UILabel LaHeroName;
    public UILabel LaPlayerName;
    public UILabel LaLv;
    public UILabel LaKill;
    public UILabel LaAid;
    public UILabel LaDeath;
    public UILabel LaMorale;

    public void Refresh (CharacterData cd, bool isSelf = false)
    {
        SpHead.spriteName = cd.attrNode.icon_name + "_head";
        LaHeroName.text = cd.attrNode.name;
        LaPlayerName.text = isSelf ? playerData.GetInstance().selfData.playeName : cd.fakeMobaPlayerName;
        LaKill.text = "" + cd.mobaKillCount;
        LaAid.text = "" + cd.mobaAidCount;
        LaDeath.text = "" + cd.mobaDeathCount;
        LaMorale.text = cd.mobaMorale + "%";
        LaLv.text = "" + cd.lvl;
    }
}