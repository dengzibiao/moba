using System.Collections.Generic;

public class CTaskSend : CSendBase
{
    public CTaskSend(ClientSendDataMgr mgr)
        : base(mgr)
    {

    }

    public void RequestDailyList () {
        NormalSend(MessageID.common_ask_daily_mission_req, C2SMessageType.ActiveWait);
    }

    public void RequestRewardList()
    {

    }

    public void ClickNpc(long npcId, int paramFirst, long paramSecond,int type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("npcId", npcId);
        newpacket.Add("paramFirst", paramFirst);
        newpacket.Add("paramSecond", paramSecond);
        newpacket.Add("arg1", type);
        PackNormalKvpAndSend(MessageID.common_click_npc_req, newpacket);
    }

    /// <summary>
    /// 打开对话�?
    /// </summary>
    /// <param name="scriptId">任务脚本id</param>
    /// <param name="PlayerSelect">选择的第几条任务（选任务的时候是任务索引 对话的时候是0�?/param>
    /// <param name="paramFirst"></param>
    /// <param name="paramSecond">1是跳过剧�?其他的为接任�?交任�?/param>
    /// <param name="thrdPram">副本id</param>
    /// <param name="fourParm">副本挑战结果</param>
    public void OpenDialogUI(int scriptId, int PlayerSelect, int paramFirst, int paramSecond, int thrdPram, int fourParm)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("scriptId", scriptId);
        newpacket.Add("PlayerSelect", PlayerSelect);
        newpacket.Add("paramFirst", paramFirst);
        newpacket.Add("paramSecond", paramSecond);
        newpacket.Add("thrdPram", thrdPram);
        newpacket.Add("fourParm", fourParm);
        PackNormalKvpAndSend(MessageID.common_open_mission_dialog_req, newpacket);
    }

    public void GetTaskList(C2SMessageType c2sType)
    {
        NormalSend(MessageID.common_mission_list_req, c2sType);
    }

    public void GetTaskListComplete(C2SMessageType c2sType)
    {
        NormalSend(MessageID.common_mission_complete_list_req, c2sType);
    }

    public void SendTaskSkillMonster(C2SMessageType c2sType, int taskId, long monsterId, long allCount)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("missionId", taskId);
        newpacket.Add("monsterId", monsterId);
        newpacket.Add("allCount", allCount);
        PackNormalKvpAndSend(MessageID.common_mission_killmonster_complete_req, newpacket, c2sType);
    }

    public void SendCompleteCopyTask(C2SMessageType c2sType, int taskId, long dungeonId, int result)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("missionId", taskId);
        newpacket.Add("dungeonId", dungeonId);
        newpacket.Add("result", result);
        PackNormalKvpAndSend(MessageID.common_mission_dungeon_complete_req, newpacket, c2sType);
    }

    public void SendCompleteGatherTask(C2SMessageType c2sType, int taskId, long itemId, long allCount,int taskType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("missionId", taskId);
        newpacket.Add("itemId", itemId);
        newpacket.Add("allCount", allCount);
        newpacket.Add("arg1", taskType);
        PackNormalKvpAndSend(MessageID.common_mission_collect_complete_req, newpacket, c2sType);
    }

    public void SendReqTaskInfo(C2SMessageType c2sType,int taskId,int scid)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", taskId);
        newpacket.Add("arg2", scid);
        PackNormalKvpAndSend(MessageID.c_Req_Taskinfo, newpacket, c2sType);
    }

    public void SendIsShowItem(C2SMessageType c2sType, int taskId, float posX, float posZ,int taskType)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("missionId", taskId);
        newpacket.Add("posX", posX);
        newpacket.Add("posZ", posZ);
        newpacket.Add("arg1", taskType);
        PackNormalKvpAndSend(MessageID.common_mission_move_complete_req, newpacket, c2sType);
    }

    public void common_mission_box_info_req()
    {
        NormalSend(MessageID.common_mission_box_info_req, C2SMessageType.ActiveWait);
    }

    public void common_offer_reward_mission_operation_req(int taskId, int type)
    {
        Dictionary<string, object> newpacket = new Dictionary<string, object>();
        newpacket.Add("arg1", taskId);
        newpacket.Add("arg2", type);//[1接受任务�?放弃 �?立即完成]
        PackNormalKvpAndSend(MessageID.common_offer_reward_mission_operation_req, newpacket);
    }

    public void common_offer_reward_mission_new_list_req()
    {
        NormalSend(MessageID.common_offer_reward_mission_new_list_req);
    }
}

