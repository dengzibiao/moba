using UnityEngine;
using System.Collections;

public class EquipCompoundPanel : GUIBase
{
    public static EquipCompoundPanel instance;
   public ItemNodeState selectMaterial;//所选合成材料
    public   UISprite equipFram;
    public UISprite equipIcon;
    public GUISingleLabel materialCount;
    public EquipMaterialItem[] MaterialArr;//
    public GameObject[] MaterialFram;
    public UILabel useMomey;//
    public GUISingleButton compoundButton;
    Transform materialEff;//材料合成特效

    public EquipCompoundPanel()
    {
        instance = this;
    }
    public void RefreshItemData()
    {
        InitData(selectMaterial);
    }
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    // Use this for initialization

    protected override void Init()
    {
    }
    protected override void ShowHandler()
    {
        materialEff = transform.Find("UI_HCCG_01");
        materialEff.gameObject.SetActive(false);
    }
    public void InitData(ItemNodeState item)
    {
        selectMaterial = item;
        materialEff = transform.Find("UI_HCCG_01");
        //materialEff.gameObject.SetActive(false);
        if (selectMaterial != null)
        {
            equipIcon.spriteName = selectMaterial.icon_name;
            equipFram.spriteName = UISign_in.GetspriteName(selectMaterial.grade);
            materialCount.text = GoodsDataOperation.GetInstance().GetItemCountById(selectMaterial.props_id).ToString();
            int count = selectMaterial.syn_condition.Length / 2;
            if(count>0)//有合成材料
            {
                for(int i = 0;i<4;i++)
                {
                    if(i == count-1)
                    {
                        MaterialFram[i].SetActive(true);
                    }
                    else
                    {
                        MaterialFram[i].SetActive(false);
                    }
                   
                }
                ItemNodeState ins;
                
                for (int m = 0;m<count;m++)
                {
                    ins = GameLibrary.Instance().ItemStateList[selectMaterial.syn_condition[m,0]];
                    MaterialArr[GetIndex(m, count)].InitData(ins,0);
                    
                }
            
            }
            useMomey.text = selectMaterial.syn_cost.ToString();
            compoundButton.onClick = OncompoundButton;
            if(!isCompoundcondition(1))
            {
                compoundButton.SetState(GUISingleButton.State.Disabled);
            }
            else
            {
                compoundButton.SetState(GUISingleButton.State.Normal);
            }

        }
        //获取索引值
    }
    bool isCompoundcondition(byte types)
    {
        if (types == 0)//判断金钱是否足够
        {
            if (selectMaterial.syn_cost > playerData.GetInstance().baginfo.gold)
            {
                return false;
            }
        }
        else if(types == 1)//判断合成材料是否足够
        {
            for (int i = 0; i < selectMaterial.syn_condition.Length / 2; i++)
            {
                if (GoodsDataOperation.GetInstance().GetItemCountById(selectMaterial.syn_condition[i, 0]) < selectMaterial.syn_condition[i, 1])
                {
                    return false;
                }
            }
        }
        return true;
    }
    //合成
    public void OncompoundButton()
    {
        if (selectMaterial.syn_condition.Length > 0)
        {
            if(isCompoundcondition(1))
            {
                ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroECom(selectMaterial.props_id, 1);
            }
            else
            {
                Control.ShowGUI(UIPanleID.UITooltips, EnumOpenUIType.DefaultUIOrSecond, false, "材料不足");
            }

            if (isCompoundcondition(0))
            {
                ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroECom(selectMaterial.props_id, 1);

            }
            else
            {
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            }
        }
        else
        {
            if (isCompoundcondition(0))
            {
                ClientSendDataMgr.GetSingle().GetHeroSend().SendHeroECom(selectMaterial.props_id, 1);

            }
            else
            {
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
            }
          
        }
    }
    void CheckMaterial0()
    {
        ItemNodeState checkeitem =GameLibrary.Instance().ItemStateList[ selectMaterial.syn_condition[0, 0]];
        if(checkeitem.syn_condition.Length>0)
        {
            EquipDevelop.GetSingolton().equipIntensifyDlg.SetIntersifyData(EquipDevelop.GetSingolton().index + 1, checkeitem.props_id);
        }
        else
        {
            //如果没有合成材料显示出处面板
            //UIGoodsGetWayPanel.Instance.SetID(selectMaterial.syn_condition[0, 0]);
            //if (UILevel.instance.GetLevelData())
            //{
            //    Control.ShowGUI(GameLibrary.UIGoodsGetWayPanel);
            //}
            Control.ShowGUI(UIPanleID.UIGoodsGetWayPanel, EnumOpenUIType.DefaultUIOrSecond, false, selectMaterial.syn_condition[0, 0]);
        }
    }
  

    int GetIndex(int pos, int count)
    {
        if (count == 1)
            return 0;
        return (count - 1) * 2 + pos - 1;
    }
    // Update is called once per frame
    void Update () {
	
	}
    public void OpenMaterialCompountEff()
    {
        materialEff.gameObject.SetActive(true);
        StartCoroutine(HideMaterialCompountEff());
    }
    IEnumerator HideMaterialCompountEff()
    {
        yield return new WaitForSeconds(1f);
        materialEff.gameObject.SetActive(false);
    }
}
