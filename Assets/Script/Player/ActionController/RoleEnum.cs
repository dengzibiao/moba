using System;
[System.Serializable]
public class RoleEnum
{
	public enum Elite_Level
	{
		NONE=0,
		ELITE=1,
		BOSS=2,
		ELITE_BOSS=3,
		TEAM_NONE=4,
		TEAM_ELITE=5,
		TEAM_ELITE_BOSS=6,
		COIN_BOSS=7,
		GUIDE=8,
		DEVIL=9,
		WBOSS=10,
		GONGHUIBOSS = 11,
	}

	public enum FireTarget
	{
		FIRE_ON_ONE_TEAM = 1,
		FIRE_ON_OTHER_TEAM = 2,
	}

	public enum Role_Face
	{
		FACE_LEFT,
		FACE_RIGHT,
	}

	public enum Control_Type
	{
		CONTROL_TYPE_FORWARDLY,
		CONTROL_TYPE_PASSIVE,
	}

	public enum Driver_Type
	{
		NONE,
		ENJOY_STICK,
		ENEMY_AI,
	}

	public enum Role_Initial_Type
	{
		INITIAL_TYPE_LOCAL,
		INITIAL_TYPE_CONFIG,
	}
//	public const short Role_Type = -1;
	public class Role_Type
	{
		public const short  NONE = -1;
		//PLAYERS
		public const short  ACTOR_TYPE_PLAYER_PROFESSION_SOLDIER = 0;
		public const short  ACTOR_TYPE_PLAYER_PROFESSION_MAGICIAN = 1;
		public const short  ACTOR_TYPE_PLAYER_PROFESSION_ARCHER = 2;
		public const short  ACTOR_TYPE_PLAYER_PROFESSION_PASTOR = 3;

		//ENEMYS
		public const short  ACTOR_ENEMY_XIAOMOGU = 18 ;
		public const short  ACTOR_ENEMY_HUANGMOGU = 57  ;
		public const short  ACTOR_ENEMY_LVSHUILING = 20  ;
		public const short  ACTOR_ENEMY_SHIRENHUA = 16  ;
		public const short  ACTOR_ENEMY_GANGBEIMOGU = 60 ;
		public const short  ACTOR_ENEMY_MOGUQISHI = 59 ;
		public const short   ACTOR_ENEMY_SHUREN = 14  ;
		public const short   ACTOR_ENEMY_FENGKUANGLULU = 23 ;
		public const short   ACTOR_ENEMY_DAWANGSHIRENHUA = 61  ;
		public const short   ACTOR_ENEMY_GUIMIANDAOCAOREN = 21  ;
		public const short   ACTOR_ENEMY_XUANFENGDAOCAOREN = 22  ;
		public const short   ACTOR_ENEMY_MONSTER_SPAWNER = 19  ;
		public const short   ACTOR_ENEMY_LANMOGU = 58 ;
		public const short   ACTOR_ENEMY_LANSHUILING = 56  ;
		public const short   ACTOR_ENEMY_SHAMIANJUREN = 34  ;
		public const short  ACTOR_ENEMY_SHADIAOGUI = 11 ;
		public const short  ACTOR_ENEMY_SANYANZHANGYU = 62 ;
		public const short  ACTOR_ENEMY_RENYUFASHI = 24  ;
		public const short  ACTOR_ENEMY_YURENZHANSHI = 63 ;
		public const short  ACTOR_ENEMY_TENGSHUREN = 64  ;
		public const short  ACTOR_ENEMY_GEBULINWUSHI = 26  ;
		public const short  ACTOR_ENEMY_SENLINSHIJUREN = 65  ;
		public const short  ACTOR_ENEMY_SENLINEMO = 66  ;
		public const short  ACTOR_ENEMY_NVWU = 25  ;
		public const short    ACTOR_ENEMY_BULUOLULU = 67  ;
		public const short 	  ACTOR_ENEMY_BULUOGUXIE = 27 ;
		public const short  	ACTOR_ENEMY_YERENCHANGMAOSHOU = 17 ;
		public const short  	ACTOR_ENEMY_YERENMUSHI = 28  ;
		public const short    	ACTOR_ENEMY_BULUOYUANREN = 13  ;
		public const short   	ACTOR_ENEMY_SANWEIBULUOGUXIE = 68 ;
		public const short  	ACTOR_ENEMY_SHIJUREN = 29 ;
		public const short   ACTOR_ENEMY_BITIGUAI = 70  ;
		public const short   ACTOR_ENEMY_WENYINIGUAI = 69  ;
		public const short    ACTOR_ENEMY_WENYIEMO = 30  ;
		public const short   ACTOR_ENEMY_MUNAIYISHUREN = 71  ;
		public const short   ACTOR_ENEMY_MUNAIYIMOGU = 72  ;
		public const short   ACTOR_ENEMY_MUNAIYI = 31  ;
		public const short   ACTOR_ENEMY_KULOUZHANSHI = 32  ;
		public const short   ACTOR_ENEMY_MUMIANJUREN = 73  ;
		public const short   ACTOR_ENEMY_GUMIANJUREN = 33  ;
		public const short   ACTOR_ENEMY_KUANDONGNIGUAI = 77  ;
		public const short   ACTOR_ENEMY_KUANGSHANJIANGSHI = 35  ;
		public const short   ACTOR_ENEMY_KUANGSHANKULOUZHANSHI = 74  ;
		public const short   ACTOR_ENEMY_KUANGDONGJUXIE = 76  ;
		public const short   ACTOR_ENEMY_KUANGSHANSHIJUREN = 75  ;
		public const short ACTOR_ENEMY_BINGSHUANGMUNAIYIMOGU = 81;
		public const short ACTOR_ENEMY_BINGSHUANGMUNAIYI = 83  ;
		public const short ACTOR_ENEMY_BINGSHUANGGEBULINWUSHI = 79  ;
		public const short ACTOR_ENEMY_BINGMIANZHANSHI = 36  ;
		public const short ACTOR_ENEMY_XUESHANGONGJIANSHOU = 37  ;
		public const short ACTOR_ENEMY_XUESHANMUSHI = 80  ;
		public const short ACTOR_ENEMY_BINGSHUANGYUANREN = 78  ;
		public const short ACTOR_ENEMY_BINGXUEEMO = 82  ;
		public const short ACTOR_ENEMY_HANBINGYILONG = 88  ;
		public const short  ACTOR_ENEMY_HUOYANMUNAIYIMOGU = 85  ;
		public const short  ACTOR_ENEMY_HUOYANLULU = 86  ;
		public const short  ACTOR_ENEMY_HUOYANMUNAIYI = 39  ;
		public const short  ACTOR_ENEMY_RONGYANGUAI = 84  ;
		public const short  ACTOR_ENEMY_HUOYUANSU = 38  ;
		public const short  ACTOR_ENEMY_HONGYILONG = 40  ;
		public const short  ACTOR_ENEMY_CHENGBAOQISHI = 15  ;
		public const short  ACTOR_ENEMY_KUIJIALULU = 42  ;
		public const short  ACTOR_ENEMY_QISHIDIAOXIANG = 44  ;
		public const short  ACTOR_ENEMY_YAZUIYILONG = 87  ;
		public const short  ACTOR_ENEMY_JINJIAQISHI = 41  ;
		public const short  ACTOR_ENEMY_GANGTIEYILONG = 43  ;
		public const short  ACTOR_ENEMY_FENGYINDELALA = 91  ;
		public const short  ACTOR_ENEMY_HAIGANGXIAOYAO = 92;
		public const short  ACTOR_ENEMY_SANYANLANZHANGYU = 93;
		public const short  ACTOR_ENEMY_YURENQIANGDAO = 94;
		public const short  ACTOR_ENEMY_HUOYANGEBULINWUSHI = 97;
		public const short  ACTOR_ENEMY_SHUIYUANSU = 98;
		public const short  ACTOR_ENEMY_DICI = 2000;
		public const short  ACTOR_ENEMY_GUIDE_SHIZIWANG = 2001;
		public const short  ACTOR_ENEMY_GUIDE_MOGUWANG = 2002;
		public const short  ACTOR_ENEMY_GUIDE_COMMON_RANDOM_MONSTER = 2003;
        public const short ACTOR_ENEMY_GUIDE_FLY_MONSTER = 2008;
		// BOSS
		public const short   ACTOR_BOSS_MOGUWANG = 45  ;
		public const short   ACTOR_BOSS_GUIMIANXUANFENGDAOCAOREN = 47  ;
		public const short   ACTOR_BOSS_BEIKEJUREN = 46  ;
		public const short   ACTOR_BOSS_SHAYINGXIAOXI = 52  ;
		public const short   ACTOR_BOSS_SHIJIEZHISHU = 89  ;
		public const short   ACTOR_BOSS_BULUOQIUCHANG = 48  ;
		public const short   ACTOR_BOSS_WENYISHITU = 49  ;
		public const short   ACTOR_BOSS_YIJISHIZHONG = 50  ;
		public const short   ACTOR_BOSS_ZHAZHA = 54  ;
		public const short   ACTOR_BOSS_SHIZIWANG = 12  ;
		public const short   ACTOR_BOSS_XINASI = 55  ;
		public const short   ACTOR_BOSS_HUOYANLONG = 53  ;
		public const short   ACTOR_BOSS_BIANFUMO = 51  ;
		public const short   ACTOR_BOSS_QIUZHANG = 95;
		public const short   ACTOR_BOSS_ANHEILONGWANG = 90;


        //guild boss
		public const short   ACTOR_BOSS_GONGHUI_MOGUWANG = 601;
		public const short   ACTOR_BOSS_GONGHUI_GUIMIANXUANFENGDAOCAOREN = 603;
		public const short   ACTOR_BOSS_GONGHUI_BEIKEJUREN = 602;
		public const short   ACTOR_BOSS_GONGHUI_WENYISHITU = 604;
		public const short   ACTOR_BOSS_GONGHUI_BIANFUMO = 605;
		//PET
		public const short   ACTOR_PET_HMG = 101;
		public const short   ACTOR_PET_XMG = 102;
		public const short   ACTOR_PET_BSYR = 103;
		public const short   ACTOR_PET_RYFS = 110;
		public const short	 ACTOR_PET_SLLS = 111;
		public const short   ACTOR_PET_JJQS = 116;
		public const short   ACTOR_PET_MNYMG = 131;
		public const short   ACTOR_PET_MGW = 132;
		public const short   ACTOR_PET_BFM = 133;
		public const short   ACTOR_PET_BSMNYMG = 134;
		public const short   ACTOR_PET_SYXX = 135;
		public const short   ACTOR_PET_WYST = 136;
		public const short   ACTOR_PET_BLQZ = 139;
		public const short   ACTOR_PET_HYL = 140;
		public const short   ACTOR_PET_SZW = 142;
		public const short   ACTOR_PET_XNS = 143;

		// NPC
		public const short  TOWN_NPC_SMITH = 301;
		public const short  TOWN_NPC_PET = 302;
		public const short  TOWN_NPC_VILLAGE = 303;
		public const short  TOWN_NPC_GUILD_MAN = 304;

		// NPC for guild
		public const short  GUILD_NPC_SKILL_MASTER = 350;
		public const short  GUILD_NPC_UPGRADE_MASTER = 351;
		public const short  GUILD_NPC_PET_MASTER = 352;
		public const short  GUILD_NPC_SWING_MASTER = 353;
		public const short  GUILD_NPC_GUILD_DEFENCE = 354;
		public const short  GUILD_NPC_WISHING_WELL = 355;
        public const short  GUILD_NPC_PVE = 356;
        public const short  GUILD_NPC_PVP = 357;

		// Transformer
		public const short   ACTOR_TRANSLATER = 1000;

		//ORGANS
		public const short    ORGANS_GUNMU = 3001;// 滚木  //
		public const short    ORGANS_BOMB = 3000;//炸弹 //
		public const short    ORGANS_COIN = 3020;//金币小人  //
		public const short    ORGANS_BUFF_SPRITE_RANDOM = 4002;//buff 精灵  //
		public const short    ORGANS_BUFF_SPRITE_GONGJI = 3032;//buff 精灵  //
		public const short    ORGANS_BUFF_SPRITE_FANGYU = 3033;//buff 精灵  //


		public const short  ORGANS_BUFF_LIFE = 3030;//生命buff //
		public const short  ORGANS_CHEST_RANDOM = 4001;// 宝箱 //
		public const short  ORGANS_CHEST_JIANGLI = 3011;
		public const short  ORGANS_CHEST_LIANJI = 3012;
		public const short  ORGANS_CHEST_XUANYUN = 3013;
		public const short  ORGANS_BATTERY = 3006;//炮台 //
		//enemy dis plugin
		public const short  ACTOR_YUANCHENG_YERENCHANGMAOSHOU = 401;
		public const short  ACTOR_YUANCHENG_NVWU = 402;
		public const short  ACTOR_YUANCHENG_GEBULINWUSHI = 403;
		public const short  ACTOR_FLY_SHADIAOGUI = 451;
		public const short  ACTOR_FLY_HAIGANGXIAOYAO = 452;
		public const short  ACTOR_FLY_SANYANZHANGYU = 453;
		public const short  ACTOR_FLY_SANYANLANZHANGYU = 454;
		public const short  ACTOR_FLY_HUOYUANSU = 455;
		public const short  ACTOR_PAOWU_DAWANGSHIRENHUA = 501;
		public const short  ACTOR_PAOWU_SHIRENHUA = 502;
		public const short  ACTOR_HUIFU_LVSHUILING = 551;
		public const short  ACTOR_HUIFU_LANSHUILING = 552;

		//SPECIAL ACTOR
		public const short  ACTOR_COIN_CHEST = 5001;
		public const short  ACTOR_CASH_KUANGGONG = 5002;
		public const short  ACTOR_PET_RANDOM_PET = 5003;

	}

	public static bool IsRemoteMonster (short type)
	{
		if (type == Role_Type.ACTOR_ENEMY_SHIRENHUA 
		    || type == Role_Type.ACTOR_ENEMY_YERENCHANGMAOSHOU
		    || type == Role_Type.ACTOR_ENEMY_GEBULINWUSHI
		    || type == Role_Type.ACTOR_ENEMY_BULUOGUXIE
		    || type == Role_Type.ACTOR_ENEMY_XUESHANGONGJIANSHOU
		    || type == Role_Type.ACTOR_ENEMY_DAWANGSHIRENHUA
		    || type == Role_Type.ACTOR_ENEMY_SANWEIBULUOGUXIE
		    || type == Role_Type.ACTOR_ENEMY_KUANGDONGJUXIE
		    || type == Role_Type.ACTOR_ENEMY_BINGSHUANGGEBULINWUSHI
		    || type == Role_Type.ACTOR_ENEMY_HUOYANGEBULINWUSHI
		    || type == Role_Type.ACTOR_ENEMY_SHUIYUANSU)
			return true;
		return false;
	}

	public static bool IsFlyMonster (short type)
	{
		if (type == Role_Type.ACTOR_ENEMY_SHADIAOGUI 
		    || type == Role_Type.ACTOR_ENEMY_NVWU
		    || type == Role_Type.ACTOR_ENEMY_GANGTIEYILONG
		    || type == Role_Type.ACTOR_ENEMY_YAZUIYILONG
		    || type == Role_Type.ACTOR_ENEMY_HANBINGYILONG
		    || type == Role_Type.ACTOR_ENEMY_HAIGANGXIAOYAO
            || type == Role_Type.ACTOR_ENEMY_GUIDE_FLY_MONSTER)
			return true;
		return false;
	}

	public static bool IsCureMonster (short type)
	{
		if (type == Role_Type.ACTOR_ENEMY_RENYUFASHI 
		    || type == Role_Type.ACTOR_ENEMY_YERENMUSHI
		    || type == Role_Type.ACTOR_ENEMY_XUESHANMUSHI)
			return true;
		return false;
	}

	public static bool IsPet (short type)
	{
		int value = (int)type;
		if (value >= 101 && value <= 266)
			return true;
		if (value >= 6001 && value < 6500)
			return true;
		return false;
	}

	public static bool IsBoss (short type)
	{
		int value = (int)type;
		if (value >= 45 && value <= 55)
			return true;
		if (value == 95 || value == 12
			|| value == 89 || value == 90)
			return true;
		if (value >= 601 && value <=605)
			return true;
        if (value == 2001)
            return true;
		return false;
	}

	public static bool IsPlayer (short type)
	{
        return type > 100 && type < 200;
	}

	public static bool IsMonster (short type)
	{
		int value = (int)type;
		if (value >= 11 && value < 100)
			return true;
		if (value > 400 && value < 600)
			return true;
		if (value >= 601 && value <=605)
			return true;
		if (IsProduceDungoenMonster (type))
			return true;
		if (value == 2001 || value == 2002 || value == 2003||value==2008)//新手引导怪物 //
			return true;
		return false;
	}

	public static bool IsOrgans (short type)
	{
		int value = (int)type;
		if (value >= 3000 && value < 4500)
			return true;
		return false;
	}

	public static bool IsNpc (short type)
	{
		int value = (int)type;
		if (value >= 300 && value < 400)
			return true;
		return false;
	}

	public static bool IsTranslater (short type)
	{
		int value = (int)type;
		if (value >= 1000 && value < 2000)
			return true;
		return false;
	}

	public static bool IsPetShiziwang (short type)
	{
		if (type >=247 &&type <= 256)
			return true;
		return false;
	}

	public static bool IsPetHuoyanlong (short type)
	{
		if (type >=227 &&type <= 236)
			return true;
		return false;
	}

	public static bool IsPetBianfumo (short type)
	{
		if (type >=197 &&type <= 206)
			return true;
		return false;
	}

	public static bool IsPetMoguwang (short type)
	{
		if (type >=217 &&type <= 226)
			return true;
		return false;
	}

    public static bool IsGuildMonster (short type) {
        if (type > 600 && type < 608)
            return true;
        return false;
    }

	public static bool MonsterCanMove (short type)
	{
		return !(type == RoleEnum.Role_Type.ACTOR_ENEMY_SHIRENHUA ||
			type == RoleEnum.Role_Type.ACTOR_ENEMY_DAWANGSHIRENHUA ||
			type == RoleEnum.Role_Type.ACTOR_BOSS_SHIJIEZHISHU ||
			type == RoleEnum.Role_Type.ACTOR_BOSS_ANHEILONGWANG ||
			type == RoleEnum.Role_Type.ACTOR_BOSS_SHIZIWANG ||
			type == RoleEnum.Role_Type.ACTOR_BOSS_YIJISHIZHONG ||
			type == RoleEnum.Role_Type.ACTOR_ENEMY_GUIDE_SHIZIWANG);
	}

	public static bool MonsterCanRotate (short type)
	{
		return !(type == RoleEnum.Role_Type.ACTOR_BOSS_ZHAZHA ||
		         type == RoleEnum.Role_Type.ACTOR_BOSS_SHIZIWANG ||
		         type == RoleEnum.Role_Type.ACTOR_ENEMY_GUIDE_SHIZIWANG ||
		         type == RoleEnum.Role_Type.ACTOR_BOSS_ANHEILONGWANG);
	}

	public static bool UseGravity (short mRoleType)
	{
		return !(mRoleType == RoleEnum.Role_Type.ACTOR_ENEMY_SHADIAOGUI ||
			mRoleType == RoleEnum.Role_Type.ACTOR_BOSS_SHIZIWANG ||
			mRoleType == RoleEnum.Role_Type.ACTOR_BOSS_ZHAZHA ||
			mRoleType == RoleEnum.Role_Type.ACTOR_ENEMY_GUIDE_SHIZIWANG ||
			(IsFlyMonster (mRoleType)));
	}

	public static sbyte GetMonsterOccupation (short type)
	{
		switch (type) {
		default: // normal
			return (sbyte)1;
		case Role_Type.ACTOR_ENEMY_RENYUFASHI: // healer
			return (sbyte)2;
		case Role_Type.ACTOR_ENEMY_YERENCHANGMAOSHOU: // ranger
			return (sbyte)3;
		}
	}

	public static bool IsProduceDungoenMonster (short type)
	{
		return (type == RoleEnum.Role_Type.ACTOR_COIN_CHEST ||
			type == RoleEnum.Role_Type.ACTOR_CASH_KUANGGONG ||
			type == RoleEnum.Role_Type.ACTOR_PET_RANDOM_PET);
	}
}

public enum CloakType
{
	CLOAK_LANCHIBANG = 1,
	CLOAK_ZICHIBANG = 2,
}