using UnityEngine;
using Tianyu;

public class HeroAttrNode : CharacterAttrNode
{
    public int break_gold;
    public int grade;//英雄品级  1：白，2：绿，3：蓝，4：紫，5：橙:，6：红；
    public int break_lv;//进阶需求等级
    public long next_grade;//进阶结果
    public int anger;//怒气点上
    public double[] equipment;//能穿戴的装备 以[0,0,0,0,0]的形式依次配置

    public HeroNode heroNode;

    public override void parseJson(object jd)
    {
        base.parseJson(jd);
        id = long.Parse(item["hero_id"].ToString());
        break_gold = int.Parse(item["break_gold"].ToString());

        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            heroNode = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[id];

            icon_name = heroNode.icon_name;
            model = heroNode.model;
            modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(model);

            skill_id = heroNode.skill_id;
            skills = new SkillNode[skill_id.Length];
            for (int i = 0; i < skills.Length; i++)
            {
                skills[i] = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[skill_id[i]];
                if (skills[i].site != 0) skillNodeDict.Add(skills[i].site, skills[i]);
            }

            grade = int.Parse(item["grade"].ToString());
            next_grade = long.Parse(item["next_grade"].ToString());
            anger = int.Parse(item["anger"].ToString());
            break_lv= int.Parse(item["break_lv"].ToString());
            int[] nodelist = (int[])item["equipment"];
            equipment = new double[nodelist.Length];

            for (int m = 0; m < nodelist.Length; m++)
            {
                equipment[m] = nodelist[m];
            }
        }
    }
    public float EquiplistHpadd()
    {
        float hpadd = 0;
        ItemEquipNode equipnode = null;
        foreach (long equipid in equipment)
        {
            equipnode = FSDataNodeTable<ItemEquipNode>.GetSingleton().DataNodeList[equipid];
            if (equipnode != null)
            {
                hpadd += equipnode.hp;
            }
        }
        return hpadd;
    }
}

