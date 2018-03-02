using UnityEngine;

public class MobaInitScene : BaseScene
{
    protected override void InitState ()
    {
        SceneID = EnumSceneID.Moba1v1;
        if(SceneBaseManager.instance!=null)
        SceneBaseManager.instance.sceneType = SceneType.MB1;
        Name = "PVP_xue";
    }
}