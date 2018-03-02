using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class UIHeroSkill : GUIBase
{

    // public UILabel CountLabel;

    public UILabel TimeLabel;
    public UILabel ReplyLabel;
    public UILabel UltimateSkill;
    public UILabel des;
    private HeroData hd = null;
    public List<ItemSkillList> itemskillList = new List<ItemSkillList>();

    //技能id列表
    List<long> skillIDList = new List<long>();

    //英雄技能数据
    List<SkillData> skilldata = new List<SkillData>();

    string waitMinute;
    string waitSecond;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void ShowHandler()
    {
        base.ShowHandler();

        //skillMultList.InSize(4, 1);

        if (null == PropertyManager.Instance.OnSkillCD)
        {
            PropertyManager.Instance.OnSkillCD += RefreshTime;
        }

        //  CountLabel.text = playerData.GetInstance().skillPoints.ToString();

        skillIDList.Clear();
        skilldata.Clear();

        long[] skillid = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[hd.id].skill_id;

        for (int i = skillid.Length - 4; i < skillid.Length; i++)
        {
            skillIDList.Add(skillid[i]);
        }
        Dictionary<long, SkillNode> skillList = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList;

        for (int i = 0; i < 4; i++)
        {
            SkillData data = new SkillData();
            data.name = skillList[skillIDList[i]].skill_name;
            data.icon = skillList[skillIDList[i]].skill_icon;//GameLibrary.skillLevel[skillIDList[i]]
            if (FSDataNodeTable<UpGradeSkillConsume>.GetSingleton().DataNodeList.ContainsKey(hd.skill[skillIDList[i]]))
            {
                data.goldCount = (UInt32)FSDataNodeTable<UpGradeSkillConsume>.GetSingleton().DataNodeList[hd.skill[skillIDList[i]]].skillsike[i];//skillVO.GetVO(skillIDList[i]).skill_value;
            }
            data.skillId = skillIDList[i];
            data.des = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[skillIDList[i]].des;
            data.level = hd.skill[skillIDList[i]];
            skilldata.Add(data);
        }

        skilldata[0].isLock = false;
        if (hd.lvl >= 5)
        {
            skilldata[1].isLock = false;
        }
        else
        {
            //skilldata[1].needLevel = "[FF0000]需要英雄等级5";
        }
        if (hd.lvl >= 10)
        {
            skilldata[2].isLock = false;
        }
        else
        {
            //skilldata[2].needLevel = "[FF0000]需要英雄等级10";
        }
        if (hd.lvl >= 15)
        {
            skilldata[3].isLock = false;
        }
        else
        {
            //skilldata[3].needLevel = "[FF0000]需要英雄等级15";
        }
        Globe.Obj[hd.id] = skilldata;

        for (int i = 0; i < itemskillList.Count; i++)
        {
            itemskillList[i].RefreshItem(skilldata[i]);
        }

        //if (Globe.Obj[hd.id].ToArray() != null)
        //{
        //    skillMultList.Info(Globe.Obj[hd.id].ToArray());
        //}
        UltimateSkill.text = skillList[skillIDList[3]].skill_name;
        des.text = skillList[skillIDList[3]].des;
    }

    public void RefreshSkill(HeroData hd)
    {
        if(this.hd!= hd) this.hd = hd;else return;

    }

    public void RefreshTime(float time)
    {

        if (time == -999)
        {
            PropertyManager.Instance.OnSkillCD -= RefreshTime;
            //  CountLabel.text = playerData.GetInstance().skillPoints.ToString();
            ReplyLabel.gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf) return;

        if (!ReplyLabel.gameObject.activeSelf)
            ReplyLabel.gameObject.SetActive(true);

        waitMinute = (time / 60) > 9 ? ((int)(time / 60)).ToString("0") : "0" + ((int)(time / 60)).ToString("0");
        waitSecond = (time % 60) > 9 ? (time % 60).ToString("0") : "0" + (time % 60).ToString("0");

        //  CountLabel.text = playerData.GetInstance().skillPoints.ToString();
        TimeLabel.text = waitMinute + ":" + waitSecond;

    }

}

public class SkillData
{
    public string needLevel;
    public bool isLock = true;
    public int clickCount = 0;
    public long skillId;
    public string name;
    public int level;
    public string icon;
    public UInt32 goldCount;
    public string des;

    static SkillData instance;
    public static SkillData GetInstance()
    {
        if (instance == null)
            instance = new SkillData();
        return instance;
    }

}