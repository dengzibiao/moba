using UnityEngine;
using System.Collections;
using System;

public class EquipIntensifyPanel : GUIBase {

    public UISprite select;//选中框
    public UISprite[] equipstepfram;//装备品质框
   // public UISprite[] equipstepicon;//装备图标
    public GUISingleButton[] equipstepbutton;//装备按钮
  //  public UILabel[] itemlvarr;
    public UILabel[] equipLv;//装备等级
    EquipData ed;
    public int index;
    ItemNodeState[] itemarr = new ItemNodeState[4];
    public  EquipEvolvePanel evolveDlg;//进化面板
    public EquipCompoundPanel equipCompoundPanel;//材料合成面板
    public static EquipIntensifyPanel instance;
    private Transform equipEff;//进化特效
    public EquipIntensifyPanel()
    {
        instance = this;
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.EquipIntensifyPanel;
    }

    protected override void OnLoadData()
    {
        base.OnLoadData();
        Singleton<Notification>.Instance.RegistMessageID(MessageID.common_upgrade_hero_equipment_ret, UIPanleID.EquipIntensifyPanel);
    }
    public override void ReceiveData(UInt32 messageID)
    {
        base.ReceiveData(messageID);
        switch (messageID)
        {
            case MessageID.common_upgrade_hero_equipment_ret://装备进化
                OpenEquipIntensEff();//播放装备进化特效
                Debug.LogError("播放强化特效");
                break;
        }
    }
    protected override void Init()
    {
        instance = this;
        equipEff = transform.Find("EquipEvolvePanel/EquipFrontFrame/UI_JingHua_01");
        equipstepbutton[0].onClick = checkbutton0;
        equipstepbutton[1].onClick = checkbutton1;
        equipstepbutton[2].onClick = checkbutton2;
        equipstepbutton[3].onClick = checkbutton3;
    }
    protected override void ShowHandler()
    {
        equipEff.gameObject.SetActive(false);
        if (EquipDevelop.GetSingolton().index + 1 < 6)
        {
            InitData(playerData.GetInstance().selectHeroDetail.equipSite[EquipDevelop.GetSingolton().index + 1]);
        }
        else
        {
            InitData(playerData.GetInstance().selectHeroDetail.equipSite[6]);
        }

    }
    void checkbutton0()
    {
        changeIndex(0);
    }
    void checkbutton1()
    {
        changeIndex(1);
    }
    void checkbutton2()
    {
        changeIndex(2);
    }
    void checkbutton3()
    {
        changeIndex(3);
    }
    public void InitData(EquipData equipData)
    {
        ed = equipData;
        itemarr[0] = GameLibrary.Instance().ItemStateList[ed.id];

        equipstepfram[0].spriteName = UISign_in.GetspriteName(ed.grade);
        // equipstepicon[0].spriteName = itemarr[0].icon_name;
        equipstepbutton[0].spriteName = itemarr[0].icon_name;
        select.transform.parent = equipstepfram[0].transform;
        select.transform.localPosition = Vector3.zero;

        equipLv[0].text = ed.level + "级";

        for (int i = 1;i<4;i++)
        equipstepfram[i].gameObject.SetActive(false);

        if (!evolveDlg.gameObject.activeSelf)
        {
            evolveDlg.gameObject.SetActive(true);
            equipCompoundPanel.gameObject.SetActive(false);
        }
        evolveDlg.InitData(ed);

    }
        // Update is called once per frame
        void Update () {
	
	}
    public void changeIndex(int pos)
    {
        index = pos;
        for (int i = index + 1; i < 4; i++)
            equipstepfram[i].gameObject.SetActive(false);
        if (index == 0)
        {
            evolveDlg.gameObject.SetActive(true);
            equipCompoundPanel.gameObject.SetActive(false);
        }
        else
        {
            equipCompoundPanel.InitData(itemarr[index]);
        }
    }
    public void SetIntersifyData(int index,long itemid)
    {
        if (index > 3)
            return;
      
        itemarr[index] = GameLibrary.Instance().ItemStateList[itemid];

        equipstepfram[index].spriteName = UISign_in.GetspriteName(ed.grade);
        if (itemarr[index].types == 1)
        {
            equipstepbutton[index].AtlasName = ResourceManager.Instance().GetUIAtlas("UIEquip");

        }
        else
        {
            equipstepbutton[index].AtlasName = ResourceManager.Instance().GetUIAtlas("Prop");
        }
        equipstepbutton[index].spriteName = itemarr[index].icon_name;
        equipstepfram[index].gameObject.SetActive(true);
        select.transform.parent = equipstepfram[index].transform;
        select.transform.localPosition = Vector3.zero;
        equipLv[index].text = "";
        this.index = index;
        for (int i = index+1; i < 4; i++)
            equipstepfram[i].gameObject.SetActive(false);
        if(index ==0)
        {
            evolveDlg.gameObject.SetActive(true);
            equipCompoundPanel.gameObject.SetActive(false);
        }
        else
        {
            equipCompoundPanel.InitData(itemarr[index]);
        }
    }
    /// <summary>
    /// 装备进化特效
    /// </summary>
    public void OpenEquipIntensEff()
    {
        equipEff.gameObject.SetActive(true);
        StartCoroutine(HideEquipStrengEff());
    }
    IEnumerator HideEquipStrengEff()
    {
        yield return new WaitForSeconds(0.7f);
        equipEff.gameObject.SetActive(false);
    }
}
