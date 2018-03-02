using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class UIBatttleWin: GUIBase
{
    public GUISingleButton closeBtn;


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }


    protected override void Init()
    {
        closeBtn.onClick = OnCloseBtnClick;
    }
    private void OnCloseBtnClick()
    {
        Hide();
        Globe.isUpdate = false;
        Globe.isRefresh = false;
        Globe.isC = false;
        Globe.isFB = false;
        Singleton<SceneManage>.Instance.Current = EnumSceneID.UI_MajorCity01;
        //UI_Loading.LoadScene(GameLibrary.UI_Major, 3);
        GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
        StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.UI_Major, 3);
        SceneManager.LoadScene("Loding");
    }
}
