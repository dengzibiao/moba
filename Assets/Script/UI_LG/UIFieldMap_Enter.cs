using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIFieldMap_Enter : GUIBase
{

    public static UIFieldMap_Enter instance;
    public GUISingleButton enterBtn;
    public GUISingleButton cancelBtn;
   
    private UILabel lable;

    public UIFieldMap_Enter()
    {
        instance = this;
    }


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {

        enterBtn.onClick = OnEnterClick;
        cancelBtn.onClick = OnCancelClick;
        
       
    }
    //private void OnRefreshClick()
    //{
    //    if (UIMoney.instance.jewelCount > refreshPrice)
    //    {
    //        ClientSendDataMgr.GetSingle().GetCShopSend().RefreshGoodsList(playerData.GetInstance().selfData.playerId, playerData.GetInstance().selfData.level, _index, count);
    //    }
    //}

   
  
    private void OnEnterClick()
    {
        this.gameObject.SetActive(false);
        Singleton<SceneManage>.Instance.Current = EnumSceneID.LGhuangyuan;
        //UI_Loading.LoadScene("LGhuangyuan", 3);
        GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
        StartLandingShuJu.GetInstance().GetLoadingData("LGhuangyuan", 3);
        SceneManager.LoadScene("Loding");
    }

    public void OpenWin()
    {
        Init();
        this.gameObject.SetActive(true);
    }

    public void OnCancelClick()
    {
        this.gameObject.SetActive(false);
    }
    //protected override void ShowHandler()
    //{
    //    count = 0;
    //    print(_index);
    //    print(VOManager.Instance().GetCSV<ShopCSV>("Shop").GetVO(_index).id);
    //    //for (int i = 1; i < VOManager.Instance().GetCSV<ShopCSV>("Shop").GetVoList() .Length ; i++)
    //    //{
    //    //    print(VOManager.Instance().GetCSV<ShopCSV>("Shop").GetVoList()[i]);
    //    //}
    //    // vo = VOManager.Instance().GetCSV<ShopCSV>("Shop").GetVO(_index);

    //    // refreshPrice = vo.refresh_cost[count];
    //}
    //public static int count;
    //private int refreshPrice;
    //private int _index;
    //private ShopVO vo;
}
