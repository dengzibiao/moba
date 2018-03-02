using UnityEngine;
using System.Collections;
using Tianyu;
public class UIExpBar : GUIBase {

    public GUISingleProgressBar expBar;
    public static UIExpBar instance;
    public static UIExpBar GetInstance()
    { return instance; }
    int Upgradelvl = 0;
    public override UIPanleID GetUIKey()
    {
        return UIPanleID.UIExpBar;
    }
    protected override void Init()
    {
        instance = this;
        expBar.state = ProgressState.STRING;
        if (FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.level))
        {
            expBar.InValue(int.Parse(playerData.GetInstance().selfData.exprience.ToString()), FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.level].exp);
        }
        
        expBar.onChange = OnExpChange;
    }
    public int RefreshExpBar(int addExp)
    {
        Upgradelvl = 0;
        playerData.GetInstance().selfData.exprience += addExp;

        PlayerUpgrade();

        expBar.InValue(int.Parse(playerData.GetInstance().selfData.exprience.ToString()), int.Parse(playerData.GetInstance().selfData.maxExprience.ToString()));
       // expBar.onChange = OnExpChange;

        if (Upgradelvl > 0)
        {
            playerData.GetInstance().ChangeActionPointCeilingHandler();
        }

        return Upgradelvl;
    }

    void PlayerUpgrade()
    {
        if (playerData.GetInstance().selfData.exprience >= playerData.GetInstance().selfData.maxExprience)
        {
            playerData.GetInstance().selfData.exprience -= playerData.GetInstance().selfData.maxExprience;
            playerData.GetInstance().selfData.level++;
            Upgradelvl++;
            playerData.GetInstance().selfData.maxExprience = FSDataNodeTable<PlayerLevelUpNode>.GetSingleton().FindDataByType(playerData.GetInstance().selfData.level).exp;
            if (playerData.GetInstance().selfData.exprience < playerData.GetInstance().selfData.maxExprience)
                return;
        }
        if (playerData.GetInstance().selfData.exprience < playerData.GetInstance().selfData.maxExprience)
            return;
        PlayerUpgrade();
    }
    private void OnExpChange(float percent)
    {

    }

}
