using UnityEngine;
using System.Collections;

public class CreateRoleSkillItem : GUISingleItemList
{

    public UISprite icon;
    private SkillData skilldata;
    /// <summary>
    /// item初始化函数
    /// </summary>
    protected override void InitItem()
    {
        //初始化
        icon = transform.Find("Icon").GetComponent<UISprite>();
        UIEventListener.Get(icon.gameObject).onPress = OnIconPress;
    }

    public override void Info(object obj)
    {
        if (obj != null)
        {
            skilldata = (SkillData)obj;
            icon.spriteName = skilldata.icon;
        }

    }
    private void OnIconPress(GameObject go, bool state)
    {
        UICreateRole.instance.skillPanel.gameObject.SetActive(state);
        UICreateRole.instance.skillPanel.GetComponent<SkillDesPanel>().SetData(skilldata.name,0,skilldata.des);
    }
}
