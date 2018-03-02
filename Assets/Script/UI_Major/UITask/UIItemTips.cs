using UnityEngine;
using System.Collections;

public class UIItemTips : MonoBehaviour
{
    public GUISingleSprite itemicon;
    public GUISingleLabel itemname;
    public GUISingleSprite qualityframe;
    public GUISingleLabel type;
    public GUISingleLabel shuoming;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItemInfo(ItemData itemdata)
    {
        ItemNodeState vo = GameLibrary.Instance().ItemStateList[itemdata.Id];
        itemdata.Describe = vo.describe;
        itemdata.IconName = vo.icon_name;
        switch (vo.grade)
        {
            case 1:
                itemdata.GradeTYPE = GradeType.Gray;
                break;
            case 2:
                itemdata.GradeTYPE = GradeType.Green;
                break;
            case 4:
                itemdata.GradeTYPE = GradeType.Blue;
                break;
            case 7:
                itemdata.GradeTYPE = GradeType.Purple;
                break;
            case 11:
                itemdata.GradeTYPE = GradeType.Orange;
                break;
            case 16:
                itemdata.GradeTYPE = GradeType.Red;
                break;
        }

        itemicon.spriteName = itemdata.IconName;
        itemname.text = itemdata.Name;
        type.text = itemdata.Types + "";
        shuoming.text = itemdata.Describe;
        string qualitycolor = "123";
        GameLibrary.QualityColor.TryGetValue(itemdata.GradeTYPE, out qualitycolor);
        if (!string.IsNullOrEmpty(qualitycolor))
        {
            qualityframe.spriteName = qualitycolor;
        }
        else
        {
            Debug.LogError("This Item Quality Is Err");
            qualityframe.spriteName = GameLibrary.QualityColor[GradeType.Gray];
        }
    }
}
