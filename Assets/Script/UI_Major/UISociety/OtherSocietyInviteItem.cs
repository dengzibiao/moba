using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 未加入公会 其他公会要求自己入会item
/// </summary>
public class OtherSocietyInviteItem : GUISingleItemList
{
    public GUISingleLabel societyID;//公会ID
    public GUISingleLabel societyName;//公会名称
    public GUISingleLabel societyLevel;//公会等级
    public GUISingleLabel presidentName;//公会会长
    public GUISingleLabel societyPlayerCount;//公会人数
    public GUISingleLabel intheapplication;//申请中
    public GUISingleButton sureBtn;//同意入会
    public GUISingleButton refuseBtn;//拒绝入会


    protected override void InitItem()
    {
        sureBtn.onClick = OnSureClick;
        refuseBtn.onClick = OnRefuseClick;
    }

    private void OnRefuseClick()
    {
        Debug.LogError("发送拒绝入会协议");
    }

    private void OnSureClick()
    {
        Debug.LogError("发送同意入会协议");
    }
}
