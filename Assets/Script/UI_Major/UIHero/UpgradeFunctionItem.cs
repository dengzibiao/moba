using UnityEngine;
using System.Collections;

public class UpgradeFunctionItem : GUISingleItemList
{
    public GUISingleSprite icon;
    private string iconName;
    private UnLockFunctionNode functionData;
    protected override void InitItem()
    {  
    }
    public override void Info(object obj)
    {
        functionData = (UnLockFunctionNode)obj;
    }
    protected override void ShowHandler()
    {
        if (functionData != null)
        {
            icon.spriteName = functionData.icon_name;
        }
    }
}
