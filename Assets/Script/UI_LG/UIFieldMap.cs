using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIFieldMap : GUIBase
{

    public GUISingleButton backBtn;

    public bool isMap = false;

    private GameObject itsKGFMapSystem;


    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }

    protected override void Init()
    {

        backBtn = transform.FindComponent<GUISingleButton>("backBtn");
        if(GameObject.Find("KGFMapSystem")!=null)
        itsKGFMapSystem = GameObject.Find("KGFMapSystem").gameObject;

        backBtn.onClick = OnBackClick;
        
    }

    void OnBackClick()
    {
        Init();
        itsKGFMapSystem.SetActive(isMap);
        //UI_Loading.LoadScene(GameLibrary.UI_Major, 2);
        GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
        StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.UI_Major, 2);
        SceneManager.LoadScene("Loding");
    }
}
