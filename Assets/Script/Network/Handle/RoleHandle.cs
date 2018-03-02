using System;
using UnityEngine;
using System.Collections.Generic;
using Tianyu;
public class CRoleHandle : CHandleBase
{
    public CRoleHandle(CHandleMgr mgr)
        : base(mgr)
    {
    }

    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.login_change_name_ret, RoleInfoHandle);
        RegistHandle(MessageID.common_player_levelup_notify, RoleUpdateHandle);

        RegistHandle(MessageID.login_change_photo_frame_ret, RoleIconHandle);

    }
    public bool KeepAliveHandle(CReadPacket packet)
    {

        return true;
    }
    /// <summary>
    ///修改名字
    /// </summary>
    public bool RoleInfoHandle(CReadPacket packet)
    {

        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
           // Control.ShowGUI(UIPanleID.UIRoleInfo, EnumOpenUIType.DefaultUIOrSecond);
            int ret = int.Parse(data["ret"].ToString().ToString());         //昵称
            if (data["diamond"] != null)
            { 
            playerData.GetInstance().RoleMoneyHadler(MoneyType.Diamond, UInt32.Parse(data["diamond"].ToString()));
            }
           // playerData.GetInstance().selfData.playeName = ChangeName._instance.nicknameInput.value;
           // UIRole.Instance.nameTxt.text = ChangeName._instance.nicknameInput.value;
           // UIRoleInfo.Instance.nameLab.text = ChangeName._instance.nicknameInput.value;
          //  if (null != CharacterManager.playerCS.pet)//改名成功后同步宠物的名字
          //  {
          //      CharacterManager.playerCS.pet.GetComponent<Pet_AI>().ChangePetName();
          //  }
           // playerData.GetInstance().selfData.changeCount = 1;
            return true;
        }
        else
        {
            if(data.ContainsKey("desc")){
                //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            }
            Control.HideGUI(UIPanleID.UIRoleInfo);
            UIRole.Instance.OpenChangeNamePanel(true);
           // Control.ShowGUI(UIPanleID.ChangeName,EnumOpenUIType.DefaultUIOrSecond);
            return false;
        }
        
    }



    /// <summary>
    /// 玩家升级
    /// </summary>
    public bool RoleUpdateHandle(CReadPacket packet)
    {
        //Control.ShowGUI(GameLibrary.Upgrade);///升级面板
        Dictionary<string, object> data = packet.data;
        Globe.isUpgrade = true;
        playerData.GetInstance().beforePlayerLevel = playerData.GetInstance().selfData.level;//记录一下之前的等级
        playerData.GetInstance().beforeStrength = playerData.GetInstance().baginfo.strength;//记录一下之前的体力
        playerData.GetInstance().selfData.level = int.Parse(data["values"].ToString());
        playerData.GetInstance().baginfo.strength = int.Parse(data["thew"].ToString());
        if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 && !Globe.isSaoDang)
        {
            //如果是在主城 经验来源是日常 和 主线 这两个都要弹出任务奖励，关闭任务奖励 在弹出升级面板
            Globe.isMainCityUpgrade = true;
        }
        else if (Singleton<SceneManage>.Instance.Current == EnumSceneID.Dungeons)
        {
            //如果是在副本 结算完之后再弹出升级面板
            //Control.ShowGUI(GameLibrary.Upgrade);
            //playerData.GetInstance().beforePlayerLevel = playerData.GetInstance().selfData.level;//记录一下之前的等级
            //playerData.GetInstance().selfData.level = int.Parse(data["values"].ToString());
            Globe.isDungeonsUpgrade = true;
        }
        else if(Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 && Globe.isSaoDang)
        {
            //如果是在主城 经济来源是扫荡 就直接弹出升级面板
            //playerData.GetInstance().beforePlayerLevel = playerData.GetInstance().selfData.level;//记录一下之前的等级
            //playerData.GetInstance().selfData.level = int.Parse(data["values"].ToString());
            Control.ShowGUI(UIPanleID.Upgrade, EnumOpenUIType.DefaultUIOrSecond);
            Globe.isSaoDang = false;
        }
        return true;

    }
    /// <summary>
    /// 修改头像
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool RoleIconHandle(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;

        int resolt = int.Parse(data["ret"].ToString());
        if (resolt == 0)
        {
            {
                int ret = int.Parse(data["ret"].ToString().ToString());         //头像 
            }
        }
        else
        {
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());//失败时返回描述
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
            UIRole.Instance.OpenChangeNamePanel(false);

        }
        return true;
    }
}
