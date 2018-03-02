using UnityEngine;
using System.Collections;
using Tianyu;
using System;

/// <summary>
/// 技能信息
/// </summary>

public class ItemSkillList : MonoBehaviour
{

    public GUISingleButton upgradeBtn;  //升级按钮
    public UISprite icon;               //技能图标
    public UILabel skillName;           //技能名字
    public UILabel level;               //技能等级
    public UILabel gold;                //升级金币
    public UISprite golds;                //升级金币
    public UISprite mlock;                //升级金币

    int lvLevel = 1;
    public SkillData data;

    /// <summary>
    /// item初始化函数
    /// </summary>
    void Awake()
    {
        //初始化
        upgradeBtn = transform.Find("UpgradeBtn").GetComponent<GUISingleButton>();
        upgradeBtn.onClick = OnUpgradeBtnBtnClick;

        level.text = lvLevel.ToString();
        gold.text= lvLevel * 100 + "";

        UIEventListener.Get(icon.gameObject).onPress = OnIconPress;

        GetComponent<BoxCollider>().enabled = false;

    }

    private void OnIconPress(GameObject go, bool state)
    {
        UI_HeroDetail.instance.skillDes.SetActive(state);
        UI_HeroDetail.instance.skillDes.transform.localPosition = new Vector3( 25 ,transform.localPosition.y -100,-600 );
        UI_HeroDetail.instance.skillDes.GetComponent<SkillDesPanel>().SetData( skillName.text , data.level , data.des );
    }

    public void RefreshItem(SkillData skill)
    {
        data = skill;
        if (skill == null)
        {
            icon.spriteName = "";
            skillName.text = "";
            level.text = "Lv.";
            gold.text = "";
        }
        else
        {
            skillName.text = data.name;
            icon.spriteName = data.icon;

            if (data.isLock)
            {
                //icon.color = Color.gray;
                upgradeBtn.gameObject.SetActive(false);
                gold.gameObject.SetActive(false);
                //mlock.gameObject.SetActive(true);

                level.text = data.needLevel;
                golds.gameObject.SetActive(false);
            }
            else
            {
                //icon.color = new Color(1, 1, 1, 1);
                level.text = "Lv." + data.level;
                gold.text = data.goldCount + "";
                upgradeBtn.gameObject.SetActive(false);//这里暂时修改成false
                gold.gameObject.SetActive(false);//这里暂时修改成false
                //mlock.gameObject.SetActive(false);
                golds.gameObject.SetActive(false);//这里暂时修改成false

            }
        }
    }

    /// <summary>
    /// 升级按钮
    /// </summary>
    private void OnUpgradeBtnBtnClick()
    {
        //升级条件
        //1.技能点足够
        //2.技能等级不大于英雄的等级
        //3.金币足够

        HeroData hd = playerData.GetInstance().GetHeroDataByID(Globe.selectHero.hero_id);

        if (playerData.GetInstance().skillPoints > 0)  
        {

            if (data.level >= hd.lvl)
            {
                //Control.ShowGUI(GameLibrary.UI_Mask);
                //Control.ShowGUI(GameLibrary.UISkillAndGoldHintPanel);
                //Control.ShowGUI(GameLibrary.UISkillAndGoldHintPanel).GetComponent<SkillAndGoldHintPanel>().SetData(3);
                PromptPanel.instance.ShowPrompt("请提升英雄等级");
                return;
            }

            if (playerData.GetInstance().baginfo.gold >= data.goldCount)
            {
                hd.skill[data.skillId]++;
                playerData.GetInstance().MoneyHadler(MoneyType.Gold, -data.goldCount);
                data.clickCount++;
                object[] obj = new object[] { data.skillId, data.clickCount };

                if (GameLibrary.skillLevelcount.ContainsKey(data.skillId))
                    GameLibrary.skillLevelcount[data.skillId]++;
                else
                    GameLibrary.skillLevelcount.Add(data.skillId, 1);


                UI_HeroDetail.instance.HeroSkill.RefreshSkill(hd);
                //UI_HeroDetail.instance.InitSkillData(Globe.selectHero.hero_id);

            }
            else
            {
                //print("金币不足");
                //Control.ShowGUI(GameLibrary.UI_Mask);
                //Control.ShowGUI(GameLibrary.UISkillAndGoldHintPanel);
                //Control.ShowGUI(GameLibrary.UISkillAndGoldHintPanel).GetComponent<SkillAndGoldHintPanel>().SetData(2);
                Control.ShowGUI(UIPanleID.UIGoldHand, EnumOpenUIType.DefaultUIOrSecond);
                return;
            }

            //UI_HeroDetail.skillCount -= 1;

            playerData.GetInstance().skillPoints -= 1;

            


        }
        else
        {
            Control.Show(UIPanleID.UIMask);
            Control.Show(UIPanleID.UISkillAndGoldHintPanel);
            Control.Show(UIPanleID.UISkillAndGoldHintPanel).GetComponent<UISkillAndGoldHintPanel>().SetData(1);
        }
        
        
    }


   
}