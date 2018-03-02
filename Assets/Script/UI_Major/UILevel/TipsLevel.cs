using UnityEngine;
using System.Collections;

public class TipsLevel : MonoBehaviour
{

    public UITexture tipsBG;
    public UITexture obtainBG;
    public UILabel starL;
    public UISprite iconS;
    public UISprite borderS;
    public UILabel countL;
    public UILabel nameL;
    public UIButton determineBtn;
    public UISprite mask;
    public UISprite subscript;

    void Start()
    {
        EventDelegate.Set(determineBtn.onClick, OnDetermineBtnClick);
        UIEventListener.Get(mask.gameObject).onClick = OnMaskClick;
    }

    public void RefreshUI(int[] info, bool isTips = true)
    {
        gameObject.SetActive(true);

        if (null == info) return;

        if (isTips)
        {
            tipsBG.gameObject.SetActive(true);
            obtainBG.gameObject.SetActive(false);
            iconS.transform.localPosition = new Vector3(0, 40, 0);
            borderS.transform.localPosition = new Vector3(0, 40, 0);
            countL.transform.localPosition = new Vector3(0, 14, 0);
            nameL.transform.localPosition = new Vector3(0, -27, 0);
            starL.text = info[0].ToString();
        }
        else
        {
            tipsBG.gameObject.SetActive(false);
            obtainBG.gameObject.SetActive(true);
            iconS.transform.localPosition = new Vector3(0, 61, 0);
            borderS.transform.localPosition = new Vector3(0, 61, 0);
            countL.transform.localPosition = new Vector3(0, 35, 0);
            nameL.transform.localPosition = new Vector3(0, -5, 0);
        }

        iconS.atlas = ResourceManager.Instance().GetUIAtlas("Prop");
        subscript.enabled = false;

        if (info[1] != 0) 
        {
            
            iconS.spriteName = "jinbi";
            countL.text = info[1].ToString();
            nameL.text = "金币";
            borderS.spriteName = "bai";
        }
        else if (info[2] != 0)
        {
            iconS.spriteName = "zuanshi";
            countL.text = info[2].ToString();
            nameL.text = "钻石";
            borderS.spriteName = "bai";
        }
        else if (info[3] != 0)
        {
            ItemNodeState item = GameLibrary.Instance().ItemStateList[info[3]];
            //if (item.icon_name.Contains("yx_"))
            //    iconS.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
            //else
            //    iconS.atlas = ResourceManager.Instance().GetUIAtlas("Prop");

            if (item.types == 6 || item.types == 7)
                iconS.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
            else
                iconS.atlas = ResourceManager.Instance().GetUIAtlas("Prop");

            iconS.spriteName = item.icon_name;
            countL.text = info[4].ToString();
            nameL.text = item.name;
            borderS.spriteName = ItemData.GetFrameByGradeType((GradeType)item.grade);
            ItemData.SetAngleMarking(subscript, item.types);
        }

    }

    void OnDetermineBtnClick()
    {
        CloseUI();
    }

    void OnMaskClick(GameObject go)
    {
        CloseUI();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
