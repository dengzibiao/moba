using System;


class MessageID
{
    //	0x0000 - 0x1FFF	发往前端，由前端处理的协议
    //	0x2000 - 0x3FFF	发往后端，由后端处理的协议
    // ------------------------------------------------------------------------------

    public const UInt32 server_start = 0x2000;
    public const UInt32 max_message_id = 0x4000;
    // ------------------------------------------------------------------------------


    public const UInt32 client_disconnect_notify = 10;

    public const UInt32 lua_general_message_req = 0x3fff;

    // ------------------------------------------------------------------------------
    //	通用协议
    public const UInt32 c_general_3i2s = 0x11;
    public const UInt32 s_general_3i2s = server_start + c_general_3i2s;

    //	ping
    public const UInt32 player_ping_ret = 0x13;
    public const UInt32 player_ping_req = server_start + player_ping_ret;

    public const UInt32 c_notify = 0x1F;
    public const UInt32 s_notify = server_start + c_notify;

    // ------------------------------------------------------------------------------
    //	进入场景
    public const UInt32 c_player_enter_scene = 0x20;
    public const UInt32 s_player_enter_scene = server_start + c_player_enter_scene;

    public const UInt32 c_player_leave_scene = 0x21;
    public const UInt32 s_player_leave_scene = server_start + c_player_leave_scene;

    public const UInt32 c_player_change_scene = 0x22;
    public const UInt32 s_player_change_scene = server_start + c_player_change_scene;

    // ------------------------------------------------------------------------------
    //	player及npc移动相关信息
    public const UInt32 player_load_scene_finished = server_start + 0x100;  // 
    public const UInt32 S_player_load_scene_finiReq = 0x100;

    //玩家行走同步给服务器
    public const UInt32 player_walk = server_start + 0x101;
    //注册场景元素
    public const UInt32 unregiste_player_walk_info = server_start + 0x102;
    //删除添加场景元素
    public const UInt32 delete_walk_object = 0x103;
    public const UInt32 add_walk_object = 0x104;
    //更新玩家信息，
    public const UInt32 c_update_player_info = 0x105;
	//更新场景元素信息
    public const UInt32 c_update_map_element_info = 0x106;

    //向服务器同步玩家朝向
    public const UInt32 s_update_player_orientation = server_start + 0x107;

    //服务器加载完数据向客户端通知消息
    public const UInt32 s_loadfinish =  0x108;

    // ------------------------------------------------------------------------------
    //	排行榜相关信息
    public const UInt32 common_ranklist_ret = 0x110;
    public const UInt32 common_ranklist_req = server_start + common_ranklist_ret;

    // ------------------------------------------------------------------------------
    //	任务相关协议

    //	点击npc
    public const UInt32 common_click_npc_req = server_start + 0x120;

    //	打开任务对话框
    public const UInt32 common_open_mission_dialog_ret = 0x121;
    public const UInt32 common_open_mission_dialog_req = server_start + common_open_mission_dialog_ret;

    //	请求任务列表
    public const UInt32 common_mission_list_ret = 0x122;
    public const UInt32 common_mission_list_req = server_start + common_mission_list_ret;

    //	任务完成情况  
    public const UInt32 common_mission_complete_list_ret = 0x123;
    public const UInt32 common_mission_complete_list_req = server_start + common_mission_complete_list_ret;

    //	暂无用
    public const UInt32 update_mission_info = 0x124;
    public const UInt32 update_mission_complete = 0x125;

    //	奖励对话框
    public const UInt32 common_notice_reward_dialogue_common_ret = 0x126;

    //	通用返回协议
    public const UInt32 common_notice_common_ret = 0x127;

    //	完成副本任务：返回notice_common = 0x127
    public const UInt32 common_mission_dungeon_complete_req = server_start + 0x128;

    //	完成采集任务返回：notice_common = 0x127
    public const UInt32 common_mission_collect_complete_req = server_start + 0x129;

    //	完成杀怪任务返回：notice_common = 0x127
    public const UInt32 common_mission_killmonster_complete_req = server_start + 0x12A;

    //	完成移动送信任务返回：notice_common = 0x127
    public const UInt32 common_mission_move_complete_req = server_start + 0x12B;

    //	通用返回协议(ex)
    public const UInt32 common_notice_common_ex_req = 0x12C;
    // ------------------------------------------------------------------------------
    //	领取活跃度奖励箱子(回：0x126)//日常任务领取奖励

    public const UInt32 common_get_reward_props_req = server_start + 0x12D;
    //	日常任务执行()
    // playerId
    // account
    // 返回:0x122<大列表>
    public const UInt32 common_ask_daily_mission_req = server_start + 0x155;//请求列表
    public const UInt32 common_ask_daily_mission_ret = 0x155;//返回日常列表

    // 日常任务奖励宝箱信息 8495
    // playerId
    // account
    public const UInt32 common_mission_box_info_req = 0x2000 + 0x12F;
    public const UInt32 common_mission_box_info_ret = 0x12F;
    // ------------------------------------------------------------------------------
    //悬赏任务
 
    // playerId
    // account
    // 返回:0x150
    public const UInt32 common_ask_offer_reward_mission_req = server_start + 0x154;//请求列表
    public const UInt32 common_offer_reward_mission_list_ret = 0x150;//返回列表
    // playerId
    // account
    //arg1任务ID
    //arg2type[1接受任务2放弃任务3立即完成]
    public const UInt32 common_offer_reward_mission_operation_req = server_start + 0x151;//执行任务
    public const UInt32 common_offer_reward_mission_new_list_req = server_start + 0x150;//刷新列表

    // ------------------------------------------------------------------------------
    //	称号相关协议
    public const UInt32 common_title_list_ret = 0x140;//称号列表
    public const UInt32 common_title_list_req = server_start + common_title_list_ret;

    //	穿、卸称号
    public const UInt32 common_title_wear_or_takeoff_ret = 0x141;
    public const UInt32 common_title_wear_or_takeoff_req = server_start + common_title_wear_or_takeoff_ret;

    // ------------------------------------------------------------------------------
    //	攻击、技能相关
    public const UInt32 c_player_attack_ret = 0x130;
    public const UInt32 s_player_attack_req = server_start + c_player_attack_ret;

    //	失血
    public const UInt32 c_lose_blood_ret = 0x131;
    public const UInt32 s_lose_blood_req = server_start + c_lose_blood_ret;

    //	死亡
    public const UInt32 c_someone_dead_ret = 0x132;
    //  public const UInt32 s_someone_dead_req = server_start + c_someone_dead_ret;

    //	怪物攻击
    public const UInt32 c_monster_attack_ret = 0x133;
    //向服务器同步玩家血量
    public const UInt32 s_set_player_hp = server_start + 0x134;

    //玩家复活消息
    public const UInt32 c_player_revive_ret = 0x135;
    public const UInt32 s_player_revive_req = server_start + c_player_revive_ret;

    public const UInt32 c_Req_Taskinfo = 0x153;


    // ----------------------------------------------------------------------------
    // Login服务器消息定义
    // ----------------------------------------------------------------------------

    // ----------------------------------------------------------------------------
    // playerId
    // account
    public const UInt32 login_check_account_req = server_start + 0x200; // 验证帐号
    public const UInt32 login_check_account_ret = 0x200;

    // ----------------------------------------------------------------------------
    // playerId
    // account
    public const UInt32 login_create_player_req = 0x3e9; // 创建角色
    public const UInt32 login_create_player_ret = 0x3ea;

    // ----------------------------------------------------------------------------
    // playerId
    // account
    public const UInt32 login_player_login_req = 0x3eb; // 角色登录
    public const UInt32 login_player_login_ret = 0x3ec;

    //public const UInt32 login_check_account_req = server_start + 0x401; // 验证帐号 9217
    //public const UInt32 login_check_account_ret = 0x401;


    //public const UInt32 login_create_player_req = server_start + 0x402; // 创建角色 9218
    //public const UInt32 login_create_player_ret = 0x402;

    //获取角斗场列表
    ///   Battle_C2D_QueryArenaList = 4701,
    ///   Battle_S2C_QueryArenaListResult = 4702,


    //public const UInt32 login_player_login_req = server_start + 0x403; // 角色登录 9219
    //public const UInt32 login_player_login_ret = 0x403;








    // ----------------------------------------------------------------------------
    // 修改角色名称 9220
    // playerId
    // account
    // name -> arg1 新名称
    public const UInt32 login_change_name_req = server_start + 0x404;
    public const UInt32 login_change_name_ret = 0x404;

    // ----------------------------------------------------------------------------
    // 修改头像框 9221
    // playerId
    // account
    // photo->arg1 新头像
    // frame->arg2 新头像框
    public const UInt32 login_change_photo_frame_req = server_start + 0x405;
    public const UInt32 login_change_photo_frame_ret = 0x405;

    // ----------------------------------------------------------------------------
    // 读取指定用户信息到Redis中 9222
    // playerId
    // account
    public const UInt32 login_load_player_info_req = server_start + 0x406;
    public const UInt32 login_load_player_info_ret = 0x406;
    //----------------------------------------------------------------------------------
// 强制用户退出游戏消息通知
    public const UInt32 login_force_quit_game_notify = 0x407; 

    // ----------------------------------------------------------------------------
    // 客户端发送离开游戏通知 9223
    // playerId
    // account
    public const UInt32 login_player_quit_game_notify = server_start + 0x408;


    // ----------------------------------------------------------------------------
    // Common服务器消息定义
    // ----------------------------------------------------------------------------


    // ----------------------------------------------------------------------------
    // 获取背包列表 9473
    // playerId
    // account
    public const UInt32 common_backpack_list_req = server_start + 0x501;
    public const UInt32 common_backpack_list_ret = 0x501;

    // ----------------------------------------------------------------------------
    // 出售物品 9474
    // playerId
    // account
    // item -> arg1 出售物品列表 [[1000010,"casdfkaj_sjfksjd_cfgoo",9]..[1000011,"cascg0aj_sjfksjd_cfgoo",2]]
    public const UInt32 common_sell_item_req = server_start + 0x502;
    public const UInt32 common_sell_item_ret = 0x502;

    // ----------------------------------------------------------------------------
    // 使用物品 9475	暂无用
    // playerId
    // account
    public const UInt32 common_use_item_req = server_start + 0x503;
    public const UInt32 common_use_item_ret = 0x503;

    // ----------------------------------------------------------------------------
    // 购买体力或活力 9476
    // playerId
    // account
    // types -> arg1 购买类型 1买体力；2买活力
    // times -> arg2 购买次数
    public const UInt32 common_buy_action_point_req = server_start + 0x504;
    public const UInt32 common_buy_action_point_ret = 0x504;

    // ----------------------------------------------------------------------------
    // 获取商店列表 9477
    // playerId
    // account
    // types -> arg1 商店类型 1杂货商店；2精英商店；3神秘商店；4远征商店；5竞技场商店；6工会商店
    // level -> arg2 当前战队等级
    public const UInt32 common_shop_goods_list_req = server_start + 0x505;
    public const UInt32 common_shop_goods_list_ret = 0x505;

    // ----------------------------------------------------------------------------
    // 购买商品 9478
    // playerId
    // account
    // types -> arg1 商店类型 1杂货商店；2精英商店；3神秘商店；4远征商店；5竞技场商店；6工会商店
    // itemId -> arg2 商品ID
    public const UInt32 common_buy_shop_goods_req = server_start + 0x506;
    public const UInt32 common_buy_shop_goods_ret = 0x506;

    // ----------------------------------------------------------------------------
    // 更新商品列表 9479
    // playerId
    // account
    // types -> arg1 商店类型 1杂货商店；2精英商店；3神秘商店；4远征商店；5竞技场商店；6工会商店
    // level -> arg2 当前战队等级
    // dr -> arg3 
    public const UInt32 common_refresh_shop_goods_req = server_start + 0x507;
    public const UInt32 common_refresh_shop_goods_ret = 0x507;

    // ----------------------------------------------------------------------------
    // 抽奖 9480
    // playerId
    // account
    // ct -> arg1 抽奖类型 0免费；1正常扣费
    // at -> arg2 抽奖个数
    // vt -> arg3 抽奖类型 1金币抽；2钻石抽；3魂匣抽
    public const UInt32 common_lucky_gamble_req = server_start + 0x508;
    public const UInt32 common_lucky_gamble_ret = 0x508;

    // ----------------------------------------------------------------------------
    // 抽奖热点英雄列表 9481
    // playerId
    // account
    public const UInt32 common_lucky_gamble_list_req = server_start + 0x509;
    public const UInt32 common_lucky_gamble_list_ret = 0x509;


    // ----------------------------------------------------------------------------
    // 更新物品列表及数量 9482
    // playerId
    // account
    public const UInt32 common_update_item_list_req = server_start + 0x50a;
    public const UInt32 common_update_item_list_ret = 0x50a; // 1290

    // ----------------------------------------------------------------------------
    // 非商店购买某物品 9483
    // playerId
    // account
    // itemId -> arg1
    // amount -> arg2
    public const UInt32 common_buy_someone_req = server_start + common_buy_someone_ret;
    public const UInt32 common_buy_someone_ret = 0x50b; // 1291


    // ----------------------------------------------------------------------------
    // 获取用户英雄列表 9488
    // playerId
    // account
    public const UInt32 common_player_hero_list_req = server_start + 0x510;
    public const UInt32 common_player_hero_list_ret = 0x510;
    //任务_击杀怪物
    //  Task_C2S_SkillMonster = 8490,


    // ----------------------------------------------------------------------------
    // 获取英雄详细信息 9489
    // playerId
    // account
    // heroId -> arg1 英雄ID
    public const UInt32 common_player_hero_info_req = server_start + 0x511;
    public const UInt32 common_player_hero_info_ret = 0x511;

    // ----------------------------------------------------------------------------
    // 指定出战英雄 9490
    // playerId
    // account
    // types -> arg1 类型 1普通副本；2竞技场防守；3活动副本一；4活动副本二；5活动副本三；6活动副本四；7活动副本五；8角斗场进攻
    // hero -> arg2 出战英雄列表 {"1":111,"2":222,"3":333,"4":444,"5":0,"6":0}
    public const UInt32 common_assign_fight_hero_req = server_start + 0x512;
    public const UInt32 common_assign_fight_hero_ret = 0x512;

    // ----------------------------------------------------------------------------
    // 魂石置换英雄 9491
    // playerId
    // account
    // heroId -> arg1 置换英雄ID
    // star -> arg2 置换英雄星级
    // itemId -> arg3 置换物品ID
    // count -> arg4 置换物品数量
    public const UInt32 common_soulgem_exchange_hero_req = server_start + 0x513;
    public const UInt32 common_soulgem_exchange_hero_ret = 0x513;

    //任务_到指定地方是否弹出道具框  它的回复协议是跑马灯
    // Task_C2S_GetToTargetPosShowItem = 8491,======================


    // ----------------------------------------------------------------------------
    // 英雄升星 9492
    // playerId
    // account
    // heroId -> arg1 升星英雄ID
    // star -> arg2 待升级至星级数
    // itemId -> arg3 升星物品ID
    // count -> arg4 升星物品数量
    public const UInt32 common_upgrade_hero_star_req = server_start + 0x514;
    public const UInt32 common_upgrade_hero_star_ret = 0x514;

    // ----------------------------------------------------------------------------
    // 升级英雄装备 9493
    // playerId
    // account
    // heroId -> arg1 升级装备英雄ID
    // site -> arg2 升级装备位置 1武器；2头盔；3胸甲；4腿甲；5护手；6鞋子 {"1":3,"2":4,"3":0,"4":1,"5":0,"6":5}
    // cGold -> arg3 升级消耗金币数量
    public const UInt32 common_upgrade_hero_equipment_req = server_start + 0x515;
    public const UInt32 common_upgrade_hero_equipment_ret = 0x515;

    // ----------------------------------------------------------------------------
    // 英雄装备进化 9494
    // playerId
    // account
    // heroId -> arg1 进化装备英雄ID
    // site -> arg2 进化装备位置 1武器；2头盔；3胸甲；4腿甲；5护手；6鞋子
    public const UInt32 common_hero_equipment_volve_req = server_start + 0x516;
    public const UInt32 common_hero_equipment_volve_ret = 0x516;

    // ----------------------------------------------------------------------------
    // 英雄装备材料合成 9495
    // playerId
    // account
    // itemId -> arg1 材料ID
    // amount -> arg2 材料数量
    public const UInt32 common_hero_equipment_compound_req = server_start + 0x517;
    public const UInt32 common_hero_equipment_compound_ret = 0x517;

    // ----------------------------------------------------------------------------
    // 英雄进阶 9496
    // playerId
    // account
    // heroId -> arg1 进阶英雄ID
    public const UInt32 common_upgrade_hero_grade_req = server_start + 0x518;
    public const UInt32 common_upgrade_hero_grade_ret = 0x518;

    // ----------------------------------------------------------------------------
    // 获取英雄技能列表 9497
    // playerId
    // account
    // heroId -> arg1 英雄ID
    public const UInt32 common_hero_skill_list_req = server_start + 0x519;
    public const UInt32 common_hero_skill_list_ret = 0x519;

    // ----------------------------------------------------------------------------
    // 英雄技能升级 9498
    // playerId
    // account
    // heroId -> arg1 英雄ID
    // item -> arg2 升级技能列表
    public const UInt32 common_upgrade_hero_skill_req = server_start + 0x51a;
    public const UInt32 common_upgrade_hero_skill_ret = 0x51a;

    // ----------------------------------------------------------------------------
    // 技能点重置 9499
    // playerId
    // account
    public const UInt32 common_reset_hero_skill_req = server_start + 0x51b;
    public const UInt32 common_reset_hero_skill_ret = 0x51b;

    // ----------------------------------------------------------------------------
    // 获取符文列表 9500
    // playerId
    // account
    public const UInt32 common_hero_runes_list_req = server_start + 0x51c;
    public const UInt32 common_hero_runes_list_ret = 0x51c;

    // ----------------------------------------------------------------------------
    // 装备符文 9501
    // playerId
    // account
    // heroId -> arg1 英雄ID
    // site -> arg2 装备位置 1武器；2头盔；3胸甲；4腿甲；5护手；6鞋子
    // itemId -> arg3 物品ID
    // types -> arg4 操作类型 1穿戴；2卸载
    public const UInt32 common_equip_hero_runes_req = server_start + 0x51d;
    public const UInt32 common_equip_hero_runes_ret = 0x51d;

    // ----------------------------------------------------------------------------
    // 合成符文 9502
    // playerId
    // account
    // itemId -> arg1 物品ID
    // amount -> arg2 个数
    public const UInt32 common_compound_hero_runes_req = server_start + 0x51e;
    public const UInt32 common_compound_hero_runes_ret = 0x51e;

    // ----------------------------------------------------------------------------
    // 英雄使用经验药水 9503
    // playerId
    // account
    // heroId -> arg1 英雄ID
    // level -> arg2 升级至等级数
    // itemId -> arg3 使用物品ID
    // amount -> arg4 使用物品个数
    public const UInt32 common_upgrade_hero_level_req = server_start + 0x51f;
    public const UInt32 common_upgrade_hero_level_ret = 0x51f;

    // ----------------------------------------------------------------------------
    // 获取英雄装备列表 9504
    // playerId
    // account
    // heroId -> arg1 英雄ID
    public const UInt32 common_hero_equipment_list_req = server_start + 0x520;
    public const UInt32 common_hero_equipment_list_ret = 0x520;

    // 英雄升级通知
    public const UInt32 common_hero_levelup_notify = 0x521;

    // 战队升级通知
    public const UInt32 common_player_levelup_notify = 0x522;

    // ----------------------------------------------------------------------------
    // 获取推荐好友列表 9520
    // playerId
    // account
    public const UInt32 common_recommended_friend_list_req = server_start + 0x530;
    public const UInt32 common_recommended_friend_list_ret = 0x530;

    // ----------------------------------------------------------------------------
    // 获取好友列表 9521
    // playerId
    // account
    public const UInt32 common_player_friend_list_req = server_start + 0x531;
    public const UInt32 common_player_friend_list_ret = 0x531;

    // ----------------------------------------------------------------------------
    // 搜索好友 9522
    // playerId
    // account
    // destId -> arg1 目标ID
    public const UInt32 common_search_friend_req = server_start + 0x532;
    public const UInt32 common_search_friend_ret = 0x532;

    // ----------------------------------------------------------------------------
    // 添加好友请求 9523
    // playerId
    // account
    // destId -> arg1 目标ID
    // destAccount -> arg2 目标帐号ID
    public const UInt32 common_add_friend_req = server_start + 0x533;
    public const UInt32 common_add_friend_ret = 0x533;

    // ----------------------------------------------------------------------------
    // 删除好友 9524
    // playerId
    // account
    // destId -> arg1 目标ID
    // types -> 操作类型 1删除好友；2删除黑名单
    public const UInt32 common_delete_friend_req = server_start + 0x534;
    public const UInt32 common_delete_friend_ret = 0x534;

    // ----------------------------------------------------------------------------
    // 获取好友功能状态列表(获取申请列表或黑名单) 9525
    // playerId
    // account
    // types -> arg1 操作类型 1好友申请列表；2好友列表，3仇人列表4最近联系
    public const UInt32 common_friend_function_listreq = server_start + 0x535;
    public const UInt32 common_friend_function_listret = 0x535;

    // ----------------------------------------------------------------------------
    // 同意添加好友 9526
    // playerId
    // account
    // destId -> arg1 目标ID
    // types -> arg2 操作类型 1同意；2拒绝添加好友
    // destAccount -> arg3 目标帐号ID
    public const UInt32 common_allow_add_friend_req = server_start + 0x536;
    public const UInt32 common_allow_add_friend_ret = 0x536;

    public const UInt32 common_player_friend_operation_notify = 0x537;

    // ----------------------------------------------------------------------------
    // 获取邮件列表 9536
    // playerId
    // account
    // start -> arg1 开始位置
    // count -> arg2 获取个数
    public const UInt32 common_player_mail_list_req = server_start + 0x540;
    public const UInt32 common_player_mail_list_ret = 0x540;

    // ----------------------------------------------------------------------------
    // 读取某邮件详细信息 9537
    // playerId
    // account
    // mailId -> arg1 邮件ID
    public const UInt32 common_read_mail_req = server_start + 0x541;
    public const UInt32 common_read_mail_ret = 0x541;

    // ----------------------------------------------------------------------------
    // 获取某邮件附件中物品 9538
    // playerId
    // account
    // mailId -> arg1 邮件ID
    public const UInt32 common_distill_mail_item_req = server_start + 0x542;
    public const UInt32 common_distill_mail_item_ret = 0x542;

    // ----------------------------------------------------------------------------
    // 删除邮件 9539
    // playerId
    // account
    // mailId -> arg1 邮件ID
    public const UInt32 common_delete_mail_req = server_start + 0x543;
    public const UInt32 common_delete_mail_ret = 0x543;

    // ----------------------------------------------------------------------------
    // 获取新邮件个数 9540
    // playerId
    // account
    public const UInt32 common_new_mail_count_req = server_start + 0x544;
    public const UInt32 common_new_mail_count_ret = 0x544;

    // ----------------------------------------------------------------------------
    // 批量更改邮件已读状态 9541
    // playerId
    // account
    // list -> arg1 邮件ID列表 {23,567,980}
    public const UInt32 common_change_mail_newf_req = server_start + 0x545;
    public const UInt32 common_change_mail_newf_ret = 0x545;

    // 新邮件通知 9542
    public const UInt32 common_have_new_mail_notify = server_start + 0x546;

    // ----------------------------------------------------------------------------
    // 定时进餐 9552
    // playerId
    // account
    public const UInt32 common_timeing_dining_req = server_start + 0x550;
    public const UInt32 common_timeing_dining_ret = 0x550;

    // ----------------------------------------------------------------------------
    // 每日签到列表 9553
    // playerId
    // account
    // types -> arg1 获取类型 1取每日签到信息；2取全部信息
    public const UInt32 common_everyday_sign_list_req = server_start + 0x551;
    public const UInt32 common_everyday_sign_list_ret = 0x551;

    // ----------------------------------------------------------------------------
    // 每日签到 9554
    // playerId
    // account
    public const UInt32 common_everyday_sign_req = server_start + 0x552;
    public const UInt32 common_everyday_sign_ret = 0x552;

    // ----------------------------------------------------------------------------
    // 每日签到累计奖励 9555
    // playerId
    // account
    // idx -> arg1 领取累计奖励位置
    public const UInt32 common_everyday_sign_reward_req = server_start + 0x553;
    public const UInt32 common_everyday_sign_reward_ret = 0x553;

    // ----------------------------------------------------------------------------
    // 每日签到补签 9556
    // playerId
    // account
    public const UInt32 common_everyday_sign_again_req = server_start + 0x554;
    public const UInt32 common_everyday_sign_again_ret = 0x554;
    //返回时间戳例161214(时间)01(签到次数)0000(签到4个宝箱状态)00(补签的次数)
    // ----------------------------------------------------------------------------
    // 领取等级奖励 9557
    // playerId
    // account
    // idx -> arg1 领取奖励位置
    public const UInt32 common_player_level_reward_req = server_start + 0x555;
    public const UInt32 common_player_level_reward_ret = 0x555;

    // ----------------------------------------------------------------------------
    // 领取在线奖励 9558
    // playerId
    // account
    public const UInt32 common_draw_online_reward_req = server_start + 0x556;
    public const UInt32 common_draw_online_reward_ret = 0x556;

    // ----------------------------------------------------------------------------
    // 新手日登陆奖励 9559
    // playerId
    // account
    public const UInt32 common_newbie_reward_req = server_start + 0x557;
    public const UInt32 common_newbie_reward_ret = 0x557;

    // ----------------------------------------------------------------------------
    // 获取金手指使用次数 9568
    // playerId
    // account
    public const UInt32 common_lucky_draw_count_req = server_start + 0x560;
    public const UInt32 common_lucky_draw_count_ret = 0x560;

    // ----------------------------------------------------------------------------
    // 使用金手指次数 9569
    // playerId
    // account
    // useTimes -> arg1 使用次数 1一次；4四次
    public const UInt32 common_use_lucky_draw_req = server_start + 0x561;
    public const UInt32 common_use_lucky_draw_ret = 0x561;

    // ----------------------------------------------------------------------------
    // 客户端聊天信息通知 9584
    // playerId
    // account
    // types -> arg1 聊天类型 1世界聊天；2工会聊天；3私聊；4附近聊天；5队伍聊天；系统聊天
    // c -> arg2 聊天内容
    public const UInt32 common_player_chat_msg_req = server_start + 0x570;
    public const UInt32 common_player_chat_msg_ret = 0x570;
    // 9585
    public const UInt32 common_server_chat_msg_notify = server_start + 0x571;


    // ----------------------------------------------------------------------------
    // 获取已创建工会列表 9776
    // playerId
    // account
    // types->arg1
    //
    public const UInt32 union_query_uncion_list_ret = 0x630;
    public const UInt32 union_query_uncion_list_req = server_start + union_query_uncion_list_ret;

    // ----------------------------------------------------------------------------
    // 搜索工会 9777
    // playerId
    // account
    // unionId->arg1
    // 
    public const UInt32 union_search_someone_ret = 0x631;
    public const UInt32 union_search_someone_req = server_start + union_search_someone_ret;

    // ----------------------------------------------------------------------------
    // 创建工会 9778
    // playerId
    // account
    // name->arg1 工会名称
    // desc->arg2 工会宣言
    // icon->arg3 工会旗帜
    //
    public const UInt32 union_create_someone_ret = 0x632;
    public const UInt32 union_create_someone_req = server_start + union_create_someone_ret;

    // ----------------------------------------------------------------------------
    // 解散工会 9779
    // playerId
    // account
    //
    public const UInt32 union_disband_someone_ret = 0x633;
    public const UInt32 union_disband_someone_req = server_start + union_disband_someone_ret;

    // ----------------------------------------------------------------------------
    // 申请加入工会 9780
    // playerId
    // account
    // unionId->arg1
    //
    public const UInt32 union_application_join_ret = 0x634;
    public const UInt32 union_application_join_req = server_start + union_application_join_ret;

    // ----------------------------------------------------------------------------
    // 获取工会申请列表 9781
    // playerId
    // account
    //
    public const UInt32 union_query_application_list_ret = 0x635;
    public const UInt32 union_query_application_list_req = server_start + union_query_application_list_ret;

    // ----------------------------------------------------------------------------
    // 批准某人进入工会 9782
    // playerId
    // account
    // destId->arg1
    //
    public const UInt32 union_approve_application_ret = 0x636;
    public const UInt32 union_approve_application_req = server_start + union_approve_application_ret;

    // ----------------------------------------------------------------------------
    // 退出工会 9783
    // playerId
    // account
    //
    public const UInt32 union_exits_someone_ret = 0x637;
    public const UInt32 union_exits_someone_req = server_start + union_exits_someone_ret;

    // ----------------------------------------------------------------------------
    // 踢出某位会员 9784
    // playerId
    // account
    // destId->arg1
    //
    public const UInt32 union_kickout_someone_ret = 0x638;
    public const UInt32 union_kickout_someone_req = server_start + union_kickout_someone_ret;

    // ----------------------------------------------------------------------------
    // 工会会长传位给某人 9785
    // playerId
    // account
    // destId->arg1 目标ID
    //
    public const UInt32 union_change_someone_position_ret = 0x639;
    public const UInt32 union_change_someone_position_req = server_start + union_change_someone_position_ret;

    // ----------------------------------------------------------------------------
    // 获取当前工会会员列表 9786
    // playerId
    // account
    // unionId->arg1 工会ID，此数值为空则显示查询人所在工会信息
    //
    public const UInt32 union_query_all_member_ret = 0x63a;
    public const UInt32 union_query_all_member_req = server_start + union_query_all_member_ret;
    // ret : {ret = 0, desc = "", item = { {id=角色ID,nm=角色昵称,up=工会职位 1会员 5会长},{} }}

    // ----------------------------------------------------------------------------
    // 编辑工会信息 9787
    // playerId
    // account
    // unionId->arg1 工会ID
    // motto->arg2 工会宣言
    // icon->arg3 工会旗帜 暂无用
    //
    public const UInt32 union_change_some_info_ret = 0x63b;
    public const UInt32 union_change_some_info_req = server_start + union_change_some_info_ret;

    // ----------------------------------------------------------------------------
    // 获取某工会详细信息 9788
    // playerId
    // account
    // unionId->arg1 工会ID，此数值为空则显示查询人所在工会信息
    //
    public const UInt32 union_query_detailed_info_ret = 0x63c;
    public const UInt32 union_query_detailed_info_req = server_start + union_query_detailed_info_ret;
    // ret : {ret = 0, desc = "", ui = 工会ID, un = 工会名称, mo = 工会宣言, 
    //        ic = 工会旗帜, ci = 会长ID, cn = 会长昵称, lv = 工会等级, 
    //		  ct = 创建时间, mp = 最大人数, cp = 当前人数}


    // ----------------------------------------------------------------------------
    // 某用户工会信息改变通知 1597
    public const UInt32 union_information_changed_notify = 0x63d;
    //notify : {tp=类型 1加入工会 2退出工会 3权限改变,ui=工会ID,un=工会名称,up=工会职位}



    // ----------------------------------------------------------------------------
    // 获取已有宠物或坐骑列表 9808
    // playerId
    // account
    // types->arg1 获取类型 1宠物 2坐骑
    public const UInt32 pet_query_list_ret = 0x650;
    public const UInt32 pet_query_list_req = server_start + pet_query_list_ret;

    // ----------------------------------------------------------------------------
    // 改变宠物或坐骑的状态 9809
    // playerId
    // account
    // types->arg1 获取类型 1宠物 2坐骑
    // petId->arg2 宠物或坐骑ID
    // status->arg3 状态 1使用 0卸载
    //
    public const UInt32 pet_change_status_ret = 0x651;
    public const UInt32 pet_change_status_req = server_start + pet_change_status_ret;

    // ----------------------------------------------------------------------------
    // 更新宠物信息 9810
    // playerId
    // account
    public const UInt32 pet_update_pet_list_ret = 0x652; // 1618
    public const UInt32 pet_update_pet_list_req = server_start + pet_update_pet_list_ret;
    // ret:{ret=0,desc="",item={{id=宠物ID,uid=uuid,lv=等级,st=状态}..{}}}

    // ----------------------------------------------------------------------------
    // 更新坐骑信息 9811
    // playerId
    // account
    public const UInt32 pet_update_mounts_list_ret = 0x653; // 1619
    public const UInt32 pet_update_mounts_list_req = server_start + pet_update_mounts_list_ret;
    // ret:{ret=0,desc="",item={{id=坐骑ID,uid=uuid,lv=等级,st=状态}..{}}}

    // ----------------------------------------------------------------------------
    // 设置默认的宠物或坐骑 9812
    // playerId
    // account
    // types->arg1 获取类型 1宠物 2坐骑
    // petId->arg2 宠物或坐骑ID
    //
    public const UInt32 pet_set_defend_status_ret = 0x654;
    public const UInt32 pet_set_defend_status_req = server_start + pet_set_defend_status_ret;



    // ----------------------------------------------------------------------------
    // Battle服务器消息定义
    // ----------------------------------------------------------------------------

    // ----------------------------------------------------------------------------
    // 获取竞技场列表 9600
    // playerId
    // account
    // types -> arg1 竞技场类型
    // hero -> arg2 出战英雄列表
    public const UInt32 pve_search_moba_list_req = server_start + 0x580;
    public const UInt32 pve_search_moba_list_ret = 0x580;

    // ----------------------------------------------------------------------------
    // 初始化竞技场战斗 9601  暂无用
    // playerId
    // account
    // destId -> arg1 目标ID
    public const UInt32 pve_init_moba_fight_req = server_start + 0x581;
    public const UInt32 pve_init_moba_fight_ret = 0x581;

    // ----------------------------------------------------------------------------
    // 竞技场战斗 9602  暂无用
    // playerId
    // account
    // destId -> arg1 目标ID
    public const UInt32 pve_start_moba_fight_req = server_start + 0x582;
    public const UInt32 pve_start_moba_fight_ret = 0x582;

    // ----------------------------------------------------------------------------
    // 竞技场战斗结算 9603
    // playerId
    // account
    // types -> arg1 类型 1成功；2失败
    public const UInt32 pve_moba_settlement_req = server_start + 0x583;
    public const UInt32 pve_moba_settlement_ret = 0x583;

    // ----------------------------------------------------------------------------
    // 竞技场胜利抽奖 9604
    // playerId
    // account
    // dn -> arg1 抽奖列表 {1,2,3}
    public const UInt32 pve_draw_moba_reward_req = server_start + 0x584;
    public const UInt32 pve_draw_moba_reward_ret = 0x584;

    // ----------------------------------------------------------------------------
    // 获取角斗场列表 9616
    // playerId
    // account
    // types -> arg1 预留字段，暂无用
    public const UInt32 pve_query_arena_list_req = server_start + 0x590;
    public const UInt32 pve_query_arena_list_ret = 0x590;

    // ----------------------------------------------------------------------------
    // 初始化角斗场战斗 9617
    // playerId
    // account
    // destId -> arg1 目标ID
    // hero -> arg2 出阵英雄
    public const UInt32 pve_init_arena_fight_req = server_start + 0x591;
    public const UInt32 pve_init_arena_fight_ret = 0x591;

    // ----------------------------------------------------------------------------
    // 角斗场战斗 9618
    // playerId
    // account
    // types -> arg1 目标ID
    public const UInt32 pve_start_arena_fight_req = server_start + 0x592;
    public const UInt32 pve_start_arena_fight_ret = 0x592;

    // ----------------------------------------------------------------------------
    // 角斗场战斗结算 9619
    // playerId
    // account
    // destId -> arg1 目标ID
    // win -> arg2 战斗结果 1赢 -1输
    // cc -> arg3 战斗日志
    public const UInt32 pve_arena_settlement_req = server_start + 0x593;
    public const UInt32 pve_arena_settlement_ret = 0x593;

    // ----------------------------------------------------------------------------
    // 角斗场CD时间或次数重置 9620
    // playerId
    // account
    // types -> arg1 重置类型 1时间 2次数
    public const UInt32 pve_arena_reload_cd_req = server_start + pve_arena_reload_cd_ret;
    public const UInt32 pve_arena_reload_cd_ret = 0x594;

    // ----------------------------------------------------------------------------
    // 获取世界地图列表 9728
    // playerId
    // account
    public const UInt32 pve_worldmap_list_req = server_start + 0x600;
    public const UInt32 pve_worldmap_list_ret = 0x600;

    // ----------------------------------------------------------------------------
    // 获取副本列表 9729
    // playerId
    // account
    // mapId -> arg1 地图ID
    // types -> arg2 副本类型 1普通副本；2精英副本
    public const UInt32 pve_dungeon_list_req = server_start + 0x601;
    public const UInt32 pve_dungeon_list_ret = 0x601;

    // ----------------------------------------------------------------------------
    // 进入副本请求 9730
    // playerId
    // account
    // mapId -> arg1 地图ID
    // dungeonId -> arg2 副本ID
    // hero -> arg3 出战英雄列表 {"1":111,"2":222,"3":333,"4":444,"5":0,"6":0}
    public const UInt32 pve_into_dungeon_req = server_start + 0x602;
    public const UInt32 pve_into_dungeon_ret = 0x602;

    // ----------------------------------------------------------------------------
    // 开始副本战斗 9731
    // playerId
    // account
    // mapId -> arg1 地图ID
    // dungeonId -> arg2 副本ID
    public const UInt32 pve_start_dungeon_fight_req = server_start + 0x603;
    public const UInt32 pve_start_dungeon_fight_ret = 0x603;

    // ----------------------------------------------------------------------------
    // 副本战斗结算 9732
    // playerId
    // account
    // mapId -> arg1 地图ID
    // dungeonId -> arg2 副本ID
    // st -> arg3 结算类型 1单次战斗结算；2战斗结束结算；9战斗失败结算
    // star -> arg4 获得星数
    public const UInt32 pve_dungeon_settlement_req = server_start + 0x604;
    public const UInt32 pve_dungeon_settlement_ret = 0x604;

    // ----------------------------------------------------------------------------
    // 领取副本列表箱子奖励 9733
    // playerId
    // account
    // mapId -> arg1 地图ID
    // dungeonId -> arg2 副本ID
    // types -> arg3 副本类型 1普通副本；2精英副本
    // star -> arg4 领取相应星数奖励
    public const UInt32 pve_draw_dungeon_box_reward_req = server_start + 0x605;
    public const UInt32 pve_draw_dungeon_box_reward_ret = 0x605;

    // ----------------------------------------------------------------------------
    // 扫荡副本 9734
    // playerId
    // account
    // mapId -> arg1 地图ID
    // dungeonId -> arg2 副本ID
    // types -> arg3 副本类型 1普通副本；2精英副本
    // times -> arg4 扫荡次数
    public const UInt32 pve_flash_dungeon_req = server_start + 0x606;
    public const UInt32 pve_flash_dungeon_ret = 0x606;

    // ----------------------------------------------------------------------------
    // 获取活动副本列表 9735
    // playerId
    // account
    public const UInt32 pve_eventdungeon_list_req = server_start + 0x607;
    public const UInt32 pve_eventdungeon_list_ret = 0x607;

    // ----------------------------------------------------------------------------
    // 进入活动副本请求 9736
    // playerId
    // account
    // dungeonId -> arg1 活动副本ID
    // hero -> arg2 出战英雄列表 {"1":111,"2":222,"3":333,"4":444,"5":0,"6":0}
    public const UInt32 pve_into_eventdungeon_req = server_start + 0x608;
    public const UInt32 pve_into_eventdungeon_ret = 0x608;

    // ----------------------------------------------------------------------------
    // 开始活动副本战斗 9737
    // playerId
    // account
    // dungeonId -> arg1 活动副本ID
    public const UInt32 pve_start_eventdungeon_req = server_start + 0x609;
    public const UInt32 pve_start_eventdungeon_ret = 0x609;

    // ----------------------------------------------------------------------------
    // 活动副本战斗结算 9738
    // playerId
    // account
    // dungeonId -> arg1 活动副本ID
    // types -> arg2 结算个数
    // st -> arg3 结算类型 2战斗结束结算；9战斗失败结算
    // star -> arg4 获得星数
    public const UInt32 pve_eventdungeon_settlement_req = server_start + 0x60a;
    public const UInt32 pve_eventdungeon_settlement_ret = 0x60a;

    // ----------------------------------------------------------------------------
    // 扫荡活动副本 9739
    // playerId
    // account
    // mapId -> arg1 地图ID
    // dungeonId -> arg2 副本ID
    // times -> arg3 扫荡次数
    // hero -> arg4 出战英雄列表
    public const UInt32 pve_eventdungeon_flash_req = server_start + 0x60b;
    public const UInt32 pve_eventdungeon_flash_ret = 0x60b;

    // ----------------------------------------------------------------------------
    // 重置某精英副本 9740
    // playerId
    // account
    // mapId -> arg1 地图ID
    // dungeonId -> arg2 副本ID
    public const UInt32 pve_reset_elite_dungeon_req = server_start + pve_reset_elite_dungeon_ret;
    public const UInt32 pve_reset_elite_dungeon_ret = 0x60c;



    //-------------------------------------------------------------------------------
    //--服务器同步给客户端的引导数据
    //--"sd"= scriptId;
    //--"td" = typeId;
    //--"sp"= stepId;
    //--wd =widgetId
    //	引导相关协议
    public const UInt32 c_player_guide_info_ret = 0x612;
    public const UInt32 s_player_guide_info_req = server_start + c_player_guide_info_ret;
    //-----------------------------------------------------------------------------------
    public const UInt32 c_player_manipulate_specified_UI = 0x613;



    // ----------------------------------------------------------------------------
    // 功能开放通知 1552
    //功能开发id的list
    public const UInt32 guide_function_opening_notify = server_start + 0x610;

    //    -- moba实时PVP战斗申请 9760
    //-- playerId
    //-- account
    //-- types -> arg1 战斗类型 1 1v1，3 3v3，5 5v5，0 测试专用
    //-- 
    public const UInt32 pvp_application_fight_req = server_start + 0x620;
    public const UInt32 pvp_application_fight_ret = 0x620;    



    // ------------------------------------------------------------------------------
    //玩家fixed1024信息 
    //common服务ID 1030
    //playerId
    //arg1 区间最小值
    //arg2 区间最大值
    public const UInt32 c_plsyer_Fixed_info_req = server_start + 0x112;//请求 8466
    public const UInt32 c_plsyer_Fixed_info_ret = 0x112;//返回 274




    // ------------------------------------------------------------------------------
    //
    public const UInt32 guide_set_req = server_start + 0x611;//  设置引导状态
    public const UInt32 c_player_buffer_specified_part = 0x111;//  缓冲器数据更新
    public const UInt32 guide_ask_for_event_req = server_start + 0x614;//  请求引导触发事件




    // ------------------------------------------------------------------------------
    //	..

    
    // 获取指定英雄属性列表 9508
    // playerId
    // account
    // hero -> arg1 英雄列表 {"1":111,"2":222,"3":333,"4":444,"5":0,"6":0}
    public const UInt32 common_specified_hero_attrib_ret = 0x524; // 1316
    public const UInt32 common_specified_hero_attrib_req = server_start + common_specified_hero_attrib_ret;
    //-----------------------------------------------------------------------------------------
	// 消息处理错误： 4098
	public const UInt32 err_message_common_ret = 0x1002;

	//	服务端中继
	//服务器转发协议
	public const UInt32 player_relay_ret = 0x15;
	//客户端上传同步数据 
	public const UInt32 player_relay_req = server_start + player_relay_ret;
  

}