using System.Collections.Generic;

public class CHeroSend : CSendBase
{
    public CHeroSend(ClientSendDataMgr mgr) : base(mgr)
	{
    }

    public void SendGetHero(C2SMessageType type)
    {
        NormalSend(MessageID.common_player_hero_list_req, type);
    }

    public void SendQuitGame()
    {
        NormalSend(MessageID.login_player_quit_game_notify, C2SMessageType.Active);
    }

    public void SendGetHeroInfo(long id, C2SMessageType type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//英雄ID
        PackNormalKvpAndSend(MessageID.common_player_hero_info_req, newpacket, type);
    }

    public void SendAssignFightHero(int types, object hero, C2SMessageType type = C2SMessageType.Active)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", types);//队列类型 类型 1普通副本；2竞技场防守；3活动副本一；4活动副本二；5活动副本三；6活动副本四；7活动副本五
        newpacket.Add("arg2", hero);//队列 出战英雄列表 {"1":111,"2":222,"3":333,"4":444,"5":0,"6":0}
        PackNormalKvpAndSend(MessageID.common_assign_fight_hero_req, newpacket, type);
    }

    public void SendSoulStoneChangeHero(long id, int star, long itemId, int amount)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);        //英雄ID
        newpacket.Add("arg2", star);        //置换英雄星级
        newpacket.Add("arg3", itemId);    //魂石ID
        newpacket.Add("arg4", amount);    //魂石数量
        PackNormalKvpAndSend(MessageID.common_soulgem_exchange_hero_req, newpacket);
    }

    public void SendUpgradeHeroStar(long id, int star, long itemId, int amount)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);        //英雄ID
        newpacket.Add("arg2", star);        //英雄星级待升星级
        newpacket.Add("arg3", itemId);    //魂石ID
        newpacket.Add("arg4", amount);    //魂石数量
        PackNormalKvpAndSend(MessageID.common_upgrade_hero_star_req, newpacket);
    }

    public void SendUpGradeHE(long id, Dictionary<string,int> site, int cGold, C2SMessageType type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//英雄ID
        newpacket.Add("arg2", site);//装备位置 进化装备位置 1武器；2头盔；3胸甲；4腿甲；5护手；6鞋子{"1":3,"2":2}
        //newpacket.Add("upLvl", upLvl);//装备升级数量
        newpacket.Add("arg3", cGold);//花费金币数
        PackNormalKvpAndSend(MessageID.common_upgrade_hero_equipment_req, newpacket, type);
    }

    public void SendHeroEMon(long id, int site)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//英雄ID
        newpacket.Add("arg2", site);//装备位置 进化装备位置 1武器；2头盔；3胸甲；4腿甲；5护手；6鞋子
        PackNormalKvpAndSend(MessageID.common_hero_equipment_volve_req, newpacket);
    }

    public void SendHeroECom(long itemId, int amount)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", itemId);//物品ID 材料ID
        newpacket.Add("arg2", amount);//合成数量 材料数量
        PackNormalKvpAndSend(MessageID.common_hero_equipment_compound_req, newpacket);
    }

    public void SendHeroAdvanced(long id)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", id);//英雄ID
        PackNormalKvpAndSend(MessageID.common_upgrade_hero_grade_req, newpacket);
    }

    public void SendEquipRunes(long heroID, int site, long itemID, int types)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", heroID);
        newpacket.Add("arg2", site);//装备位置 1武器；2头盔；3胸甲；4腿甲；5护手；6鞋子
        newpacket.Add("arg3", itemID);
        newpacket.Add("arg4", types);//操作类型 1穿戴；2卸载
        PackNormalKvpAndSend(MessageID.common_equip_hero_runes_req, newpacket);
    }

    public void SendRunesCompounes(long itemID, int count)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", itemID);
        newpacket.Add("arg2", count);
        PackNormalKvpAndSend(MessageID.common_compound_hero_runes_req, newpacket);
    }

    public void SendDrugUpgrade(long heroid,int types, C2SMessageType type = C2SMessageType.Active)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", heroid);//英雄ID
        newpacket.Add("arg2", types);//升级类型
        PackNormalKvpAndSend(MessageID.common_upgrade_hero_level_req, newpacket);
    }
}