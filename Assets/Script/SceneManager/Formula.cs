public enum AttrType
{
    power = 0,//力量
    intelligence,//智力
    agility,//敏捷
    hp,//生命
    attack,//攻击
    armor,//护甲
    magic_resist,//魔抗
    critical,//暴击
    dodge,//闪避
    hit_ratio,//命中
    armor_penetration,//护甲穿透
    magic_penetration,//魔法穿透
    suck_blood,//吸血
    tenacity,//韧性

    movement_speed,//初始移动速
    attack_speed,//初始攻击速度
    striking_distance,//目标距
    hp_regain//生命恢复
}

public enum MainAttr
{
    power,
    intelligence,
    agility
}
/// <summary>
/// 客户端需要计算的公式
/// </summary>
public class Formula
{
    public const int ATTR_COUNT = 14;//目前英雄是14个属性

    /// <summary>
    /// 增加指定类型的属性值
    /// </summary>
    public static void AddAttrWith(ref float[] attrs, AttrType attrType, float val)
    {
        attrs[(int)attrType] += val;
    }
    /// <summary>
    /// 设置指定类型的属性值
    /// </summary>
    public static void SetAttrTo(ref float[] attrs, AttrType attrType, float newVal)
    {
        if ((int)attrType < attrs.Length)
        {
            attrs[(int)attrType] = newVal;
        }
        else
        {
           
        }
    }
    /// <summary>
    /// 获取指定类型的属性值
    /// </summary>
    public static float GetAttr(float[] attrs, AttrType attrType)
    {
        if ((int)attrType < attrs.Length)
        {
            return attrs[(int)attrType];
        }
		return 0;
    }

    /// <summary>
    /// 获取指定类型的主属性值（力敏智三者之一） |
    /// 计算公式：基础值+星级成长系数*英雄等级+装备加成+buff值
    /// </summary>
    public static float GetAProperty(HeroData hd, AttrType type)
    {
        HeroAttrNode heroAttr = (HeroAttrNode)hd.attrNode;
        float baseAtttrVal = 0f;
        if (type == AttrType.power)
            baseAtttrVal = heroAttr.power;
        else if (type == AttrType.intelligence)
            baseAtttrVal = heroAttr.intelligence;
        else if (type == AttrType.agility)
            baseAtttrVal = heroAttr.agility;
        return baseAtttrVal + heroAttr.heroNode.GetStarGrowUpRate((int)type, hd.star) * hd.lvl + hd.equipTotalAttrs[(int)type] + GetAttr(hd.buffAttrs, type);
    }

    /// <summary>
    /// 计算指定类型的二级属性基础值 |
    /// 计算公式：基础值 +装备加成+buff值
    /// </summary>
    static float GetBProperty(HeroData hd, AttrType type)
    {
        return GetAttr(hd.attrNode.base_Propers, type) + hd.equipTotalAttrs[(int)type];
    }

    /// <summary>
    /// 计算指定类型的二级属性最终值（二级属性包括生命、攻击、护甲、魔抗、暴击、护甲穿透、魔法穿透、韧性、闪避、命中、吸血） |
    /// 属性分服务器和本地两套，服务器属性从serverAttrs里取，本地属性根据配表自行计算 | 
    /// 英雄属性计算公式：基础值+对应主属性值*属性加成率 |
    /// 怪物属性计算公式：基础值+等级成长*等级+buff值
    /// </summary>
    public static float GetSingleAttribute(CharacterData characterData, AttrType type)
    {
        if (characterData is HeroData)
        {
            HeroData hd = (HeroData)characterData;
            HeroAttrNode heroAttrNode = (HeroAttrNode)characterData.attrNode;
            switch (type)
            {
                case AttrType.hp:
                    return CalcServerOrLocalAttr(hd, type, AttrType.power, 23.5f);
                case AttrType.attack:
                    // 攻击力=(主属性+敏捷*0.5)*9+（初始值+装备）
                    float servMainAttr = GetAttr(hd.serverAttrs, type) + 9 * (GetAttr(hd.buffAttrs, (AttrType)(heroAttrNode.heroNode.attribute - 1)) + 0.5f * GetAttr(hd.buffAttrs, AttrType.agility)) + GetAttr(hd.buffAttrs, type);
                    float localMainAttr = 9 * (GetAProperty(hd, (AttrType)(heroAttrNode.heroNode.attribute - 1)) + 0.5f * GetAProperty(hd, AttrType.agility)) + GetBProperty(hd, type);
                    return characterData.useServerAttr ? servMainAttr : localMainAttr + GetAttr(hd.buffAttrs, type);
                case AttrType.armor:
                    return CalcServerOrLocalAttr(hd, type, AttrType.agility, 3.6f);
                case AttrType.magic_resist:
                    return CalcServerOrLocalAttr(hd, type, AttrType.intelligence, 6.6f);
                case AttrType.critical:
                    return CalcServerOrLocalAttr(hd, type, AttrType.agility, 2.25f);
                case AttrType.armor_penetration:
                    return CalcServerOrLocalAttr(hd, type, AttrType.agility, 1.8f);
                case AttrType.magic_penetration:
                    return CalcServerOrLocalAttr(hd, type, AttrType.intelligence, 3.3f);
                case AttrType.tenacity:
                    return CalcServerOrLocalAttr(hd, type, AttrType.power, 1.25f);
                default:
                    return characterData.useServerAttr ? (GetAttr(hd.serverAttrs, type) + GetAttr(hd.buffAttrs, type)) : (GetBProperty(hd, type) + GetAttr(hd.buffAttrs, type));
            }
        }
        else
        {
            if (characterData != null)//野外打怪会报错
            {
                MonsterAttrNode maNode = (MonsterAttrNode)characterData.attrNode;
                MonsterData monsterData = (MonsterData)characterData;
                return GetAttr(maNode.attrLvRates, type) * (monsterData.lvlRate != 0 ? monsterData.lvlRate : characterData.lvl) + GetAttr(maNode.base_Propers, type) + GetAttr(characterData.buffAttrs, type);
            }
            return 0;
        }
    }

    static float CalcServerOrLocalAttr(HeroData hd, AttrType type, AttrType relateAttrType, float relateAttrRatio)
    {
        float baseVal = 0f;
        if (hd.useServerAttr)
        {
            baseVal = GetAttr(hd.serverAttrs, type) + relateAttrRatio * GetAttr(hd.buffAttrs, relateAttrType);
        }
        else
        {
            baseVal = GetBProperty(hd, type) + relateAttrRatio * GetAProperty(hd, relateAttrType);
        }
        return baseVal + GetAttr(hd.buffAttrs, type);
    }

    /// <summary>
    /// 计算指定类型属性的比率（闪避、暴击和吸血）
    /// </summary>
    public static float GetSingleAttributeRate(CharacterData charData, AttrType type, int clvl = 1)
    {
        return GetSingleAttribute(charData, type) / ((GetSingleAttribute(charData, type) + (5 * clvl + 5) * 80));
    }
}
