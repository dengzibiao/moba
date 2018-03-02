using Tianyu;
using System.Collections.Generic;
using System;

public class UnLockFunctionNode : FSDataNodeBase
{
    public short id;//系统ID 
    public int unlock_system_type;//解锁系统类型 0：默认解锁；1：战队等级；2：通关某个副本(副本id)；3：VIP等级；
    public string info;//系统开启信息
    //public string remark;//备注
    public string icon_name; //图标名称
    public int chapter_id;//章节ID
    public byte released;//当前版本是否开放 0 : No，1：Yes
    public int condition_parameter;//功能解锁参数
    public byte need_check;//需要提示 0：不需要；1：需要
    public byte need_guide;//需要引导 0：不需要；1：需要
    public int entrance_unlock;//图标或模型解锁类型 0：功能解锁才显示；1：显示但不可用（界面上锁），达到功能解锁条件才可用
    public string limit_tip;//限制提示 #：没有提示


    public override void parseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = short.Parse(item["id"].ToString());
        unlock_system_type = Convert.ToInt32(item["unlock_system_type"]);
        info = item["info"].ToString();
        // remark = item["remark"].ToString();
        icon_name = item["icon_name"].ToString();
        chapter_id = Convert.ToInt32(item["chapter_id"]);
        released = byte.Parse(item["released"].ToString());
        condition_parameter = Convert.ToInt32(item["condition_parameter"]);
        need_check = byte.Parse(item["need_check"].ToString());
        need_guide = byte.Parse(item["need_guide"].ToString());
        entrance_unlock = Convert.ToInt32(item["entrance_unlock"]);
        limit_tip = item["limit_tip"].ToString();
    }
}
