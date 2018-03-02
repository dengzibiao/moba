using Tianyu;

public class MonsterAttrNode : CharacterAttrNode
{
    public float lv_hp;
    public float lv_attack;
    public float lv_armor;
    public float lv_resist;
    public float lv_critical;
    public float lv_dodge;
    public float lv_ratio;
    public float lv_armorpenetration;
    public float lv_magicpenetration;
    public float lv_suckblood;
    public float lv_tenacity;
    public string effect_sign;

    public float[] attrLvRates;

    public override void parseJson(object jd)
    {
        base.parseJson(jd);

        id = long.Parse(item["monster_id"].ToString());

        types = int.Parse(item["types"].ToString());
        describe = item["describe"].ToString();
        info = item["info"].ToString();
        icon_name = item["icon_name"].ToString();
        model = int.Parse(item["model"].ToString());
        modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(model);
        released = int.Parse(item["released"].ToString());
        is_icon = int.Parse(item["is_icon"].ToString());

        if (null != item["skill_id"] && item["skill_id"] is int[])
        {
            int[] node = item["skill_id"] as int[];
            if (node != null)
            {
                skill_id = new long[node.Length];
                skills = new SkillNode[node.Length];
                for (int i = 0; i < node.Length; i++)
                {
                    skill_id[i] = long.Parse(node[i].ToString());
                    skills[i] = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[skill_id[i]];
                    if (skills[i].site != 0) skillNodeDict.Add(skills[i].site, skills[i]);
                }
            }
        }

        lv_hp = float.Parse(item["lv_hp"].ToString());
        lv_attack = float.Parse(item["lv_attack"].ToString());
        lv_armor = float.Parse(item["lv_armor"].ToString());
        lv_resist = float.Parse(item["lv_resist"].ToString());
        lv_critical = float.Parse(item["lv_critical"].ToString());
        lv_dodge = float.Parse(item["lv_dodge"].ToString());
        lv_ratio = float.Parse(item["lv_ratio"].ToString());
        lv_armorpenetration = float.Parse(item["lv_armorpenetration"].ToString());
        lv_magicpenetration = float.Parse(item["lv_magicpenetration"].ToString());
        lv_suckblood = float.Parse(item["lv_suckblood"].ToString());
        lv_tenacity = float.Parse(item["lv_tenacity"].ToString());

        attrLvRates = new float[Formula.ATTR_COUNT] { 0f, 0f, 0f, lv_hp, lv_attack, lv_armor, lv_resist, lv_critical, lv_dodge, lv_ratio, lv_armorpenetration, lv_magicpenetration, lv_suckblood, lv_tenacity };

        model_size = float.Parse(item["model_size"].ToString());
        effect_sign = item["effect_sign"].ToString();

    }
}
