using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;


public enum decType
{
    Equip = 1, Material, MaterialChip, GoldProp, ExpProp, SoulStone, HeroCard, Rune, RecoverProp, SweptVolume, JewelBox
}

public class UIgoodstips : GUIBase
{

    public UIAtlas Propatlas;
    public UIAtlas UIHeroHeadatlas;
    public GUISingleLabel goodsName;
    public UISprite goodsImg;
    public UISprite kuang;
    public UILabel dec;
    public UILabel goodsNum;
    public GUISingleButton box;
    private string goodsname;
    private string decs;
    private string goodsimg;
    private string kuangs;
    private long ID;
    private Transform debris;
    private ItemData itemdata;
    public enum goodsType
    {
        gold = 1,
        exp = 2,
        jewel = 3,

    }
    private static UIgoodstips instances;
    public static UIgoodstips Instances
    {
        get { return instances; }
        set { instances = value; }
    }
    public UIgoodstips()
    {
        instances = this;
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
    protected override void Init()
    {
        box = transform.Find("box").GetComponent<GUISingleButton>();
        debris = transform.Find("Debris");
        box.onClick = Hide;
    }
    protected override void ShowHandler()
    {
        if (itemdata != null)
        {
            ShowItemData();
        }
        else
        {
            goodsName.text = goodsname;
            dec.text = decs;
            kuang.spriteName = kuangs;

            if (goodsimg == "zuanshi")
            {
                goodsImg.atlas = Propatlas;
                goodsImg.spriteName = goodsimg;
                debris.gameObject.SetActive(false);//钻石不显示灵魂石
                goodsNum.text = playerData.GetInstance().baginfo.diamond.ToString();
            }
            else
            {
                int goodsIDNum = int.Parse(ID.ToString().Substring(0, 3));
                if (goodsIDNum == 107 || goodsIDNum == 106)
                {
                    goodsImg.atlas = UIHeroHeadatlas;
                    goodsImg.spriteName = goodsimg;
                    if (goodsIDNum == 106)//106时显示灵魂石
                    {
                        debris.gameObject.SetActive(true);
                    }
                    else { debris.gameObject.SetActive(false); }
                    if (GameLibrary.Instance().ItemStateList.ContainsKey(ID))
                    {
                        if (GameLibrary.Instance().ItemStateList[ID].types == (int)ItemType.HeroCard)
                        {
                            goodsNum.text = MountAndPetNodeData.Instance().GetHeroCount(ID).ToString();
                        }
                        else
                        {
                            goodsNum.text = GoodsDataOperation.GetInstance().GetItemCountById(ID).ToString();
                        }
                    }
                }
                else
                {
                    goodsImg.atlas = Propatlas;
                    goodsImg.spriteName = goodsimg;
                    debris.gameObject.SetActive(false);
                    if (GameLibrary.Instance().ItemStateList.ContainsKey(ID))
                    {
                        if (GameLibrary.Instance().ItemStateList[ID].types == (int)ItemType.Pet)
                        {
                            goodsNum.text = MountAndPetNodeData.Instance().GetMountOrPetCount(ID, MountAndPet.Pet).ToString();
                        }
                        else if (GameLibrary.Instance().ItemStateList[ID].types == (int)ItemType.Mount)
                        {
                            goodsNum.text = MountAndPetNodeData.Instance().GetMountOrPetCount(ID, MountAndPet.Mount).ToString();
                        }
                        else
                        {
                            goodsNum.text = GoodsDataOperation.GetInstance().GetItemCountById(ID).ToString();
                        }
                       
                    }

                }
            }

        }


    }
    public void ShowItemData()
    {
        if (itemdata == null) return;
        goodsImg.atlas = itemdata.UiAtlas;
        //goodsName.text = itemdata.Name;
        goodsName.text = GoodsDataOperation.GetInstance().JointNameColour(itemdata.Name, itemdata.GradeTYPE);
        dec.text = itemdata.Describe;
        if (itemdata.GoodsType == MailGoodsType.DiamomdType)
        {
            goodsNum.text = playerData.GetInstance().baginfo.diamond.ToString() + "";
            goodsImg.spriteName = "zuanshi";
        }
        if (itemdata.GoodsType == MailGoodsType.GoldType)
        {
            goodsNum.text = playerData.GetInstance().baginfo.gold.ToString() + "";
            goodsImg.spriteName = "jinbi";
        }
        if (itemdata.GoodsType == MailGoodsType.ExE)
        {
            goodsNum.text = itemdata.Exe + "";
            goodsImg.spriteName = "zhandui-exp";
        }
        if (itemdata.GoodsType == MailGoodsType.PowerType)
        {
            goodsNum.text = itemdata.Power + "";
            goodsImg.spriteName = "tili";
        }
        if (itemdata.GoodsType == MailGoodsType.HeroExp)
        {
            goodsNum.text = itemdata.HeroExp + "";
            goodsImg.spriteName = "exp";
        }
        if (itemdata.GoodsType == MailGoodsType.XuanshangGold)
        {
            goodsNum.text = itemdata.XuanshangGold + "";
            goodsImg.spriteName = "xuanshangbi";
        }
        if (itemdata.GoodsType == MailGoodsType.ItemType)
        {
            if (itemdata.Types == (int)ItemType.Pet)
            {
                goodsNum.text = MountAndPetNodeData.Instance().GetMountOrPetCount(itemdata.Id,MountAndPet.Pet).ToString();
            }
            else if (itemdata.Types == (int)ItemType.Mount)
            {
                goodsNum.text = MountAndPetNodeData.Instance().GetMountOrPetCount(itemdata.Id, MountAndPet.Mount).ToString();
            }
            else if (itemdata.Types == (int)ItemType.HeroCard)
            {
                goodsNum.text = MountAndPetNodeData.Instance().GetHeroCount(itemdata.Id).ToString();
            }
            else
            {
                goodsNum.text = GoodsDataOperation.GetInstance().GetItemCountById(itemdata.Id).ToString();
            }
            dec.text = GoodsDataOperation.GetInstance().ConvertGoodsDes(itemdata);
            if (GameLibrary.Instance().ItemStateList.ContainsKey(itemdata.Id))
            {
                goodsImg.spriteName = GameLibrary.Instance().ItemStateList[itemdata.Id].icon_name;
            }
        }
        if (itemdata.Types == 6)
        {
            debris.gameObject.SetActive(true);
        }
        else
        {
            debris.gameObject.SetActive(false);
        }
        switch (itemdata.GradeTYPE)
        {
            case GradeType.Gray:
                kuang.spriteName = "hui";
                break;
            case GradeType.Green:
                kuang.spriteName = "lv";
                break;
            case GradeType.Blue:
                kuang.spriteName = "lan";
                break;
            case GradeType.Purple:
                kuang.spriteName = "zi";
                break;
            case GradeType.Orange:
                kuang.spriteName = "cheng";
                break;
            case GradeType.Red:
                kuang.spriteName = "hong";
                break;
            default:
                break;
        }

    }
    public void Hide()
    {
        itemdata = null;
        Control.HideGUI(UIPanleID.UIgoodstips);
    }
    public void Setgoods(ItemNodeState ItemNode, long ID)
    {
        this.goodsname = GoodsDataOperation.GetInstance().JointNameColour(ItemNode.name, GetGradeType(ItemNode.grade));
        this.decs = GoodsDataOperation.GetInstance().GetConvertGoodsDes(ItemNode, ID);
        this.goodsimg = ItemNode.icon_name;
        this.kuangs = GetspriteName(ItemNode.grade);
        this.ID = ID;

        if (ItemNode.name != null && ItemNode.describe != null && ItemNode.icon_name != null && ID != null)
        {
            Show();
        }
    }
    public GradeType GetGradeType(int grade)
    {
        if (grade == (int)GradeType.Blue)
        {
            return GradeType.Blue;
        }
        else if (grade == (int)GradeType.Gray)
        {
            return GradeType.Gray;
        }
        else if (grade == (int)GradeType.Green)
        {
            return GradeType.Green;
        }
        else if (grade == (int)GradeType.Orange)
        {
            return GradeType.Orange;
        }
        else if (grade == (int)GradeType.Purple)
        {
            return GradeType.Purple;
        }
        else if (grade == (int)GradeType.Red)
        {
            return GradeType.Red;
        }
        return 0;
    }
    public void SetjewelImg(goodsType type)
    {
        if (goodsType.jewel == type)
        {
            this.goodsname = "钻石";
            this.decs = "";
            this.goodsimg = "zuanshi";
            this.kuangs = "cheng";//钻石默认边框
        }
        else if (goodsType.gold == type)
        {
            this.goodsname = "金币";
            this.decs = "";
            this.goodsimg = "jinbi";
            this.kuangs = "zi";
        }
        else if (goodsType.exp == type)
        {
            //到时候再加吧，现在什么都还没有呢。
        }
        if (goodsname != null && decs != null && goodsimg != null)
        {
            Show();
        }
    }
    public string GetspriteName(int Gradetype)
    {
        switch (Gradetype)
        {
            case 1:
                return "hui";
                break;
            case 2:
                return "lv";
                break;
            case 4:
                return "lan";
                break;
            case 7:
                return "zi";
                break;
            case 11:
                return "cheng";
                break;
            case 16:
                return "hong";
                break;
            default:
                break;
        }
        return "hui";
    }
    public void SetItemData(ItemData data)
    {
        this.itemdata = data;
    }
}
