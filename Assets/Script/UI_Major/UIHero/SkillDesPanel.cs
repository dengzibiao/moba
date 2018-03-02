using UnityEngine;
using System.Collections;

public class SkillDesPanel : MonoBehaviour
{

    private UILabel skillName;

    private UILabel skillLv;

    private UILabel skillDes;

    void Awake()
    {
        skillName = transform.Find("SkillName").GetComponent<UILabel>();
        skillLv = transform.Find("SkillLevel").GetComponent<UILabel>();
        skillDes = transform.Find("SkillDes").GetComponent<UILabel>();
    }

    public void SetData(string name,int lv,string des)
    {
        skillName.text = name;
        skillLv.text = "Lv."+lv;
        skillDes.text = des;
    }

}

