using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FlopPanel : MonoBehaviour 
{
    public GUISingleButton BtnFlopAll;
    public GUISingleButton BtnFightAgain;
    public GUISingleButton BtnOK;
    public UILabel AllPrice;
    public UILabel LaCoin;
    public UILabel LaMyDiamond;
    public UIGrid GdCards;
    public GameObject TitleEffect;

    FlopCardItem[] flopCardItems;
    List<FlopItem> flopItems = new List<FlopItem>();

    public const int MAX_FLOP = 5;
    bool flopAnimPlaying;
    bool flopNoShow;
    long playerDiamond;
    int currentFlopCount = 1;
    FlopCardItem cardToFlop;

    void Start ()
    {
        GameObject cardPrefab = Resources.Load<GameObject>(GameLibrary.PATH_UIPrefab + "FlopCardItem");
        for(int k = 0; k < MAX_FLOP; k++)
        {
            NGUITools.AddChild(GdCards.gameObject, cardPrefab);
        }
        GdCards.Reposition();

        flopCardItems = GetComponentsInChildren<FlopCardItem>();
        for(int i = 0; i < flopCardItems.Length; i++)
        {
            flopCardItems[i].OnFlop += Flop;
            flopCardItems[i].OnFlopped += ( f ) => flopAnimPlaying = false;
            flopCardItems[i].RefreshPrice(0);
        }
    }

    public void Show( Dictionary<string, object> dict, string coinNum)
    {
        foreach(string k in dict.Keys)
        {
            Dictionary<string, object> itemData = (Dictionary<string, object>)dict[k];
            FlopItem flopItem = new FlopItem();
            flopItem.index = int.Parse(k);
            flopItem.itemId = int.Parse(itemData["id"].ToString());
            flopItem.cost = int.Parse(itemData["cs"].ToString());
            flopItem.num = int.Parse(itemData["at"].ToString());
            flopItems.Add(flopItem);
        }
        flopItems.Sort((a,b)=> { return a.index - b.index; });

        gameObject.SetActive(true);
        LaCoin.text = coinNum;
        BtnFlopAll.onClick = FlopAll;
        BtnOK.onClick = backScene;
        BtnFightAgain.onClick = backScene;
        TitleEffect.gameObject.SetActive(true);

        AllPrice.text = "" + GetAllCost();
        playerDiamond = playerData.GetInstance().baginfo.diamond;
        LaMyDiamond.text = "" + playerDiamond;
    }

    void backScene()
    {
        if(currentFlopCount == 1)
        {
            flopNoShow = true;
            ClientSendDataMgr.GetSingle().GetMobaSend().SendFlopResult(new int[] { currentFlopCount });
        }
        if (GameLibrary.isMoba)
            GameLibrary.isMoba = false;
        gameObject.SetActive(false);
        Time.timeScale = 1;
        GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
        StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.UI_Major, 3);
        SceneManager.LoadScene("Loding");
    }

    public void Flopped ( int[] dn )
    {
        if(flopNoShow)
            return;
        if(dn.Length > 1 || currentFlopCount == MAX_FLOP)
        {
            for(int j = 0; j < flopCardItems.Length; j++)
            {
                if(!flopCardItems[j].flopped)
                {
                    DoFlop(flopCardItems[j]);
                }
            }
        }
        else
        {
            DoFlop(cardToFlop);
        }

        LaMyDiamond.text = "" + playerDiamond;
        if(currentFlopCount <= MAX_FLOP)
        {
            for(int i = 0; i < flopCardItems.Length; i++)
            {
                if(!flopCardItems[i].flopped)
                    flopCardItems[i].RefreshPrice(flopItems[currentFlopCount - 1].cost);
            }
            AllPrice.text = "" + GetAllCost();
        }
        else
        {
            BtnFlopAll.SetState(GUISingleButton.State.Disabled);
            AllPrice.text = "0";
        }
    }

    void DoFlop (FlopCardItem card)
    {
        card.DoFlop(flopItems[currentFlopCount - 1]);
        playerDiamond -= flopItems[currentFlopCount - 1].cost;
        currentFlopCount++;
    }

    int GetAllCost ()
    {
        int ret = 0;
        for(int i = currentFlopCount - 1; i<MAX_FLOP; i++)
        {
            ret += flopItems[i].cost;
        }
        return ret;
    }

    void Flop ( FlopCardItem flopCardItem )
    {
        if(currentFlopCount <= MAX_FLOP)
        {
            if(playerDiamond < flopItems[currentFlopCount - 1].cost)
            {
                //UIPromptBox.Instance.ShowLabel("您的钻石不足");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
            }
            else
            {
                if(!flopAnimPlaying)
                    flopAnimPlaying = true;
                else
                    return;
                cardToFlop = flopCardItem;
                ClientSendDataMgr.GetSingle().GetMobaSend().SendFlopResult(new int[] { currentFlopCount });
            }
        }
    }

    void FlopAll ()
    {
        if(currentFlopCount <= MAX_FLOP)
        {
            if(playerDiamond < GetAllCost())
            {
                //UIPromptBox.Instance.ShowLabel("您的钻石不足");
                Control.ShowGUI(UIPanleID.UIPromptBox, EnumOpenUIType.DefaultUIOrSecond, false, "您的钻石不足请充值");
            }
            else
            {
                if(!flopAnimPlaying)
                    flopAnimPlaying = true;
                else
                    return;
                int[] dn = new int[MAX_FLOP - currentFlopCount + 1];
                for(int i = currentFlopCount; i <= MAX_FLOP; i++)
                {
                    dn[i - currentFlopCount] = i;
                }
                ClientSendDataMgr.GetSingle().GetMobaSend().SendFlopResult(dn);
            }
        }
    }
}