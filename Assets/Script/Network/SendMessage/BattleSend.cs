using System.Collections.Generic;

public class CBattleSend : CSendBase
{
    public CBattleSend(ClientSendDataMgr mgr) : base(mgr)
    {

    }

    public void SendQueryArenaList(int arg1)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", arg1);
        PackNormalKvpAndSend(MessageID.pve_query_arena_list_req, newpacket, C2SMessageType.ActiveWait);
    }
	//向服务器同步玩家数据
	public void SenPlayerData ()
	{
		Dictionary<string,object> newpacket = new Dictionary<string, object> ();
		newpacket.Add ("msg", "123456");
		PackNormalKvpAndSend (MessageID.player_relay_req,newpacket,C2SMessageType.Active);
	}

    public void SendInitArenaFighting(int arg1, object arg2)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", arg1);
        newpacket.Add("arg2", arg2);
        PackNormalKvpAndSend(MessageID.pve_init_arena_fight_req, newpacket, C2SMessageType.ActiveWait);
    }

    public void SendStarArenaFighting(int arg1)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", arg1);
        PackNormalKvpAndSend(MessageID.pve_start_arena_fight_req, newpacket, C2SMessageType.Active);
    }

    public void SendArenaSettlement(int arg1, int arg2, string arg3)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", arg1);
        newpacket.Add("arg2", arg2);
        newpacket.Add("arg3", arg3);
        PackNormalKvpAndSend(MessageID.pve_arena_settlement_req, newpacket, C2SMessageType.Active);
    }

    public void SendArenaReloadCD(int arg1)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", arg1);
        PackNormalKvpAndSend(MessageID.pve_arena_reload_cd_req, newpacket, C2SMessageType.ActiveWait);
    }

    public void SendQueryWorldMap()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        PackNormalKvpAndSend(MessageID.pve_worldmap_list_req, newpacket, C2SMessageType.Active);
    }

    public void SendQueryDungeonList(List<int> mapId, int types)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", types);
        PackNormalKvpAndSend(MessageID.pve_dungeon_list_req, newpacket, C2SMessageType.Active);
    }

    public void SendIntoDungeon(int mapId, int dungeonId, int types, object hero)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", dungeonId);
        newpacket.Add("types", types);
        newpacket.Add("arg3", hero);
        PackNormalKvpAndSend(MessageID.pve_into_dungeon_req, newpacket, C2SMessageType.ActiveWait);
    }

    public void SendStartFight(int mapId, int dungeonId)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", dungeonId);
        PackNormalKvpAndSend(MessageID.pve_start_dungeon_fight_req, newpacket, C2SMessageType.Active);
    }

    public void SendFightSettlement(int mapId, int dungeonId, int types, int[] star, int st)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", dungeonId);
        newpacket.Add("arg5", types);
        newpacket.Add("arg4", star);
        newpacket.Add("arg3", st);
        PackNormalKvpAndSend(MessageID.pve_dungeon_settlement_req, newpacket, C2SMessageType.ActiveWait);
    }

    public void SendDrawDungeonBoxReward(int mapId, int dungeonId, int types, int star)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", dungeonId);
        newpacket.Add("arg3", types);
        newpacket.Add("arg4", star);
        PackNormalKvpAndSend(MessageID.pve_draw_dungeon_box_reward_req, newpacket, C2SMessageType.ActiveWait);
    }

    public void SendFlashDungeonFight(int mapId, int dungeonId, int types, int times)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", dungeonId);
        newpacket.Add("arg3", types);
        newpacket.Add("arg4", times);
        PackNormalKvpAndSend(MessageID.pve_flash_dungeon_req, newpacket, C2SMessageType.Active);
    }

    public void SendResetEliteDungeon(int mapId, int dungeonId)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", dungeonId);
        PackNormalKvpAndSend(MessageID.pve_reset_elite_dungeon_req, newpacket, C2SMessageType.ActiveWait);
    }
    // 非商店购买某物品
    public void SendBuySomeone(int itemId, int amount)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", itemId);
        newpacket.Add("arg2", amount);
        PackNormalKvpAndSend(MessageID.common_buy_someone_req, newpacket, C2SMessageType.PASVWait);
    }

    //活动
    public void SendQueryEventList()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        PackNormalKvpAndSend(MessageID.pve_eventdungeon_list_req, newpacket, C2SMessageType.Active);
    }

    public void SendIntoEventDungeon(int dungeonId, object hero)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", dungeonId);
        newpacket.Add("arg2", hero);
        PackNormalKvpAndSend(MessageID.pve_into_eventdungeon_req, newpacket, C2SMessageType.ActiveWait);
    }

    public void SendStartEventFight(int dungeonId)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", dungeonId);
        PackNormalKvpAndSend(MessageID.pve_start_eventdungeon_req, newpacket, C2SMessageType.Active);
    }

    public void SendEventFightSettlement(int dungeonId, int types, int[] star, int st)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", dungeonId);
        newpacket.Add("arg2", types);
        newpacket.Add("arg3", st);
        newpacket.Add("arg4", star);
        PackNormalKvpAndSend(MessageID.pve_eventdungeon_settlement_req, newpacket, C2SMessageType.Active);
    }

    public void SendEventFlashDungeonFight(int mapId, int dungeonId, int times, object hero)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", mapId);
        newpacket.Add("arg2", dungeonId);
        newpacket.Add("arg3", times);
        newpacket.Add("arg4", hero);
        PackNormalKvpAndSend(MessageID.pve_eventdungeon_flash_req, newpacket, C2SMessageType.Active);
    }

    public void SendApplicationFightReq(int types)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", types);
        PackNormalKvpAndSend(MessageID.pvp_application_fight_req, newpacket, C2SMessageType.Active);
    }

    public void SendGetHerosBattleAttr (int[] heroIds)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", heroIds);
        PackNormalKvpAndSend(MessageID.common_specified_hero_attrib_req, newpacket);
    }
    public void Sendpve_init_moba_fight_req()
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
      //  newpacket.Add("arg1", heroIds);
        PackNormalKvpAndSend(MessageID.pve_init_moba_fight_req, newpacket);
    }
}
