using UnityEngine;
using System.Collections;
using Tianyu;
public class ServerItemList : GUISingleItemList
{

    public GUISingleLabel label;

    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
       // label = this.GetComponentInChildren<GUISingleLabel>();
    }

    public override void Info(object obj)
    {
        if (obj==null)
        {
            
        }
        else
        {
            ServeData svdata = (ServeData)obj;
            if (svdata.state == 1)
                label.text = svdata.name;// (index + 1) + " 区";
            else
            {
                label.text = svdata.name + svdata.Desc;
            }
        }
      
    }

    private void OnBtnClick()
    {
        print("我是钉子" + index);
    }
}
