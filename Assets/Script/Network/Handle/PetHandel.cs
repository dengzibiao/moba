using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using UnityEngine.SceneManagement;

public class CPetHandel : CHandleBase
{
    public CPetHandel(CHandleMgr mgr) : base(mgr)
    {
    }
    public override void RegistAllHandle()
    {
        RegistHandle(MessageID.pet_query_list_ret, HaveMountOrPetResult);
        RegistHandle(MessageID.pet_change_status_ret, ChangeMountOrPetStateResult);
        RegistHandle(MessageID.pet_update_pet_list_ret, UpdatePetInfoResult);
        RegistHandle(MessageID.pet_update_mounts_list_ret, UpdateMountInfoResult);
        RegistHandle(MessageID.pet_set_defend_status_ret, UseMountOrPetResult);
        
    }
    public bool HaveMountOrPetResult(CReadPacket packet)
    {
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        object[] itemList = data["item"] as object[];
        int types = int.Parse(data["types"].ToString());
        if (resolt == 0)
        {
            if (itemList!=null)
            {
                if (types == 1)
                {
                    MountAndPetNodeData.Instance().havePetList.Clear();
                    
                }
                else if (types == 2)
                {
                    MountAndPetNodeData.Instance().haveMountlist.Clear();
                }
                if (itemList!=null)
                {
                    for (int i = 0; i < itemList.Length; i++)
                    {

                        Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                        ShoucangData shoucangData = new ShoucangData();
                        if(itemDataDic.ContainsKey("id"))
                        shoucangData.id = long.Parse(itemDataDic["id"].ToString());
                        //shoucangData.uuid = itemDataDic["uid"].ToString();
                        //shoucangData.level = int.Parse(itemDataDic["lv"].ToString());
                        //shoucangData.state = int.Parse(itemDataDic["st"].ToString());
                        if (types == 1)//宠物
                        {
                            if (MountAndPetNodeData.Instance().havePetList.ContainsKey(shoucangData.id))
                            {
                                MountAndPetNodeData.Instance().havePetList[shoucangData.id] = shoucangData;
                            }
                            else
                            {
                                MountAndPetNodeData.Instance().havePetList.Add(shoucangData.id, shoucangData);
                            }
                            
                        }
                        else if (types == 2)//坐骑
                        {
                            if (MountAndPetNodeData.Instance().haveMountlist.ContainsKey(shoucangData.id))
                            {
                                MountAndPetNodeData.Instance().haveMountlist[shoucangData.id] = shoucangData;
                            }
                            else
                            {

                                MountAndPetNodeData.Instance().haveMountlist.Add(shoucangData.id, shoucangData);
                            }
                            
                        }
                    }
                }
                
            }
            if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
            {
                //if (Control.GetGUI(GameLibrary.UIMountAndPet).gameObject.activeSelf && types == 1)
                //{
                //    UIMountAndPet.Instance.ShowPet();//刷新宠物
                //}
                //else if (Control.GetGUI(GameLibrary.UIMountAndPet).gameObject.activeSelf && types == 2)
                //{
                //    UIMountAndPet.Instance.ShowMount();//刷新坐骑
                //}
            }
        }
        else
        {
            Debug.Log(string.Format("获取已创建公会列表失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool ChangeMountOrPetStateResult(CReadPacket packet)
    {
        Debug.Log("改变宠物或坐骑的状态结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        int types = int.Parse(data["types"].ToString());
        
        //int state = int.Parse();
        //object[] itemList = data["item"] as object[];
        if (resolt == 0)
        {
            int state = int.Parse(data["status"].ToString());
            if (types == 1)
            {
                if (state == 1)
                {
                    MountAndPetNodeData.Instance().godefPetID = long.Parse(data["petId"].ToString());
                    //显示宠物
                    CharacterManager.playerCS.CreatePet(MountAndPetNodeData.Instance().godefPetID);
                    Debug.Log("显示宠物");
                }
                else if(state == 0)
                {
                    MountAndPetNodeData.Instance().godefPetID = 0;
                    //隐藏宠物
                    CharacterManager.playerCS.HidePet();
                    Debug.Log("隐藏宠物");
                }
            }
            else if (types == 2)
            {
                if (state == 1)
                {
                    MountAndPetNodeData.Instance().goMountID = long.Parse(data["petId"].ToString());
                    CharacterManager.playerCS.pm.Ride(true, MountAndPetNodeData.Instance().goMountID);
                    Debug.Log("上马");
                    //上马
                }
                else if(state == 0)
                {
                    MountAndPetNodeData.Instance().goMountID = 0;
                    CharacterManager.playerCS.pm.Ride(false);
                    Debug.Log("下马");
                    //下马
                }
            }
        }
        else
        {
            Debug.Log(string.Format("改变宠物或坐骑的状态失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool UpdatePetInfoResult(CReadPacket packet)
    {
        Debug.Log("更新宠物信息结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        object[] itemList = data["item"] as object[];
        if (resolt == 0)
        {

            if (itemList != null)
            {
                for (int i = 0; i < itemList.Length; i++)
                {

                    Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                    ShoucangData shoucangData = new ShoucangData();
                    shoucangData.id = long.Parse(itemDataDic["id"].ToString());
                    //shoucangData.uuid = itemDataDic["uid"].ToString();
                    //shoucangData.level = int.Parse(itemDataDic["lv"].ToString());
                    //shoucangData.state = int.Parse(itemDataDic["st"].ToString());
                    if (MountAndPetNodeData.Instance().havePetList.ContainsKey(shoucangData.id))
                    {
                        MountAndPetNodeData.Instance().havePetList[shoucangData.id] = shoucangData;
                    }
                    else
                    {
                        MountAndPetNodeData.Instance().havePetList.Add(shoucangData.id, shoucangData);
                    }
                }
            }

            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01 && Control.GetGUI(GameLibrary.UIMountAndPet).gameObject.activeSelf)
            //{
            //    UIMountAndPet.Instance.ShowPet();//刷新宠物
            //}
        }
        else
        {
            Debug.Log(string.Format("更新宠物信息失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
    public bool UpdateMountInfoResult(CReadPacket packet)
    {
        Debug.Log("更新坐骑信息结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        object[] itemList = data["item"] as object[];
        if (resolt == 0)
        {
            if (itemList != null)
            {
                for (int i = 0; i < itemList.Length; i++)
                {

                    Dictionary<string, object> itemDataDic = itemList[i] as Dictionary<string, object>;
                    ShoucangData shoucangData = new ShoucangData();
                    shoucangData.id = long.Parse(itemDataDic["id"].ToString());
                    //shoucangData.uuid = itemDataDic["uid"].ToString();
                    //shoucangData.level = int.Parse(itemDataDic["lv"].ToString());
                    //shoucangData.state = int.Parse(itemDataDic["st"].ToString());
                    if (MountAndPetNodeData.Instance().haveMountlist.ContainsKey(shoucangData.id))
                    {
                        MountAndPetNodeData.Instance().haveMountlist[shoucangData.id] = shoucangData;
                    }
                    else
                    {
                        MountAndPetNodeData.Instance().haveMountlist.Add(shoucangData.id,shoucangData);
                    }
                }
            }
            //if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01&&Control.GetGUI(GameLibrary.UIMountAndPet).gameObject.activeSelf)
            //{
            //    UIMountAndPet.Instance.ShowMount();//刷新坐骑
            //}
        }
        else
        {
            Debug.Log(string.Format("更新坐骑信息失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }


    public bool UseMountOrPetResult(CReadPacket packet)
    {
        Debug.Log("使用宠物或坐骑结果");
        Dictionary<string, object> data = packet.data;
        int resolt = int.Parse(data["ret"].ToString());
        int types = int.Parse(data["types"].ToString());
        if (resolt == 0)
        {
            if (types == 1)
            {
                MountAndPetNodeData.Instance().currentPetID = long.Parse(data["petId"].ToString());
                //UIMountAndPet.Instance.ShowPet();//刷新宠物
                CharacterManager.playerCS.HidePet();

            }
            else if (types == 2)
            {
                MountAndPetNodeData.Instance().currentMountID = long.Parse(data["petId"].ToString());
                //UIMountAndPet.Instance.ShowMount();//刷新坐骑
                if (CharacterManager.playerCS.pm.isRiding)//如果玩家正骑着坐骑，然后更换了坐骑，需要让玩家下坐骑（因为坐骑已经改变了）
                {
                    CharacterManager.playerCS.pm.Ride(false);
                }
            }
        }
        else
        {
            Debug.Log(string.Format("使用宠物或坐骑失败：{0}", data["desc"].ToString()));
            //UIPromptBox.Instance.ShowLabel(data["desc"].ToString());
            Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, data["desc"].ToString());
        }
        return true;
    }
}
