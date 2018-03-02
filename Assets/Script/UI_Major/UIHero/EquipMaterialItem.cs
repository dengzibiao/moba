using UnityEngine;
using System.Collections;

public class EquipMaterialItem : GUIBase
{

    public GUISingleButton MaterialName;
    public UISprite MaterialIcon;
    public UISprite debris;
    public UILabel MaterialNum;
    public ItemNodeState INS;
    int localpos = 0;


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {
        MaterialName.onClick = ChechButton;
    }
    void ChechButton()
    {
        if (INS.syn_condition.Length > 0)
        {
            EquipDevelop.GetSingolton().equipIntensifyDlg.SetIntersifyData(EquipDevelop.GetSingolton().index + 1, INS.props_id);
                 }
        else
        {
            //如果没有合成材料显示出处面板
            //UIGoodsGetWayPanel.Instance.SetID(INS.props_id);
            //if (UILevel.instance.GetLevelData())
            //{
            //    Control.ShowGUI(GameLibrary.UIGoodsGetWayPanel);
            //}
            Control.ShowGUI(UIPanleID.UIGoodsGetWayPanel, EnumOpenUIType.DefaultUIOrSecond, false, INS.props_id);
        }
    }
    protected override void ShowHandler()
    {
        if (INS == null)
        {
            long equpid = playerData.GetInstance().selectHeroDetail.equipSite[EquipDevelop.GetSingolton().index + 1].id;
            INS = GameLibrary.Instance().ItemStateList[equpid];
        }
        MaterialName.spriteName = INS.icon_name;
        MaterialIcon.spriteName = UISign_in.GetspriteName(INS.grade);
        if (int.Parse(INS.props_id.ToString().Substring(0, 3)) == 103)
        {
            debris.gameObject.SetActive(true);
        }
        else
        {
            debris.gameObject.SetActive(false);
        }

        if (int.Parse(INS.props_id.ToString().Substring(0, 3)) == 102)
        {
            HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);
            EquipData ed;
            hd.equipSite.TryGetValue(HeroAndEquipNodeData.site, out ed);
            ItemNodeState ins = GameLibrary.Instance().ItemStateList[ed.id + 1];
            Debug.Log(localpos + "  ===>" + ed.id);
            if (ins != null)
            {
                if (localpos < ins.syn_condition.Length / 2)
                {
                    MaterialNum.text = playerData.GetInstance().GetItemCountById(ins.syn_condition[localpos, 0]) + "/" + ins.syn_condition[localpos, 1].ToString();

                }
                else
                {
                    Debug.Log(localpos + "  ===>" + ed.id);
                }
            }

        }
        else
        {
            Debug.Log(INS.props_id);
            MaterialNum.text = playerData.GetInstance().GetItemCountById(GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNums(INS.props_id.ToString())].syn_condition[localpos, 0]) + "/" + GameLibrary.Instance().ItemStateList[HeroAndEquipNodeData.Instance().GetReplaceNums(INS.props_id.ToString())].syn_condition[localpos, 1].ToString();
        }


    }
    public void InitData(ItemNodeState ins, int local)
    {
        INS = ins;
        localpos = local;
        HeroAndEquipNodeData.INSlist.Add(INS);
        ShowHandler();
    }
}
