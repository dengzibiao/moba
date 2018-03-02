using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMoba : MonoBehaviour 
{
    public UIButton BattleBtn;
    public UIButton BackBtn;

    void Start () 
	{
        EventDelegate.Add(BackBtn.onClick, () =>
        {
            gameObject.SetActive(false);
            GameLibrary.player = GameLibrary.emeny;
        });
        EventDelegate.Add(BattleBtn.onClick, ()=>
        {
            GameLibrary.isPracticeMoba = true;
            List<long> randIds = BattleUtil.GetRandomTeam(3);
            Globe.mobaMyTeam = new HeroData[] { new HeroData(GameLibrary.player), new HeroData(randIds[0]), new HeroData(randIds[1]), new HeroData(randIds[2]) };
            gameObject.SetActive(false);
            Singleton<SceneManage>.Instance.Current = EnumSceneID.Dungeons;
            //UI_Loading.LoadScene(GameLibrary.PVP_Moba, 2f);
            GameLibrary.LastScene = SceneManager.GetActiveScene().name;//记录前一个场景名
            StartLandingShuJu.GetInstance().GetLoadingData(GameLibrary.PVP_Moba, 2f);
            SceneManager.LoadScene("Loding");
        });
    }
}