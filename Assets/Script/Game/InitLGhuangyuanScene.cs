using UnityEngine;
using System.Collections;

public class InitLGhuangyuanScene : BaseScene
{

    protected override void InitState()
    {
        this.SceneID = EnumSceneID.LGhuangyuan;
        this.Name = "LGhuangyuan";
    }
    protected override void Init()
    {
        // CreatPrefab("SceneCtrl_LG", 0, 0, 0, null, "Prefab/SceneCtrls/");
        //CreatPrefab("UI Root",0,100,0,null, "Prefab/");
        //RegistUI("UITaskUseItemPanel", UIPanleID.UITaskUseItemPanel);
        //RegistUI(GameLibrary.UITaskCollectPanel, UIPanleID.UITaskCollectPanel);
        //RegistUI("UITaskTracker", UIPanleID.UITaskTracker);
        //RegistUI("UIDeadToReborn", UIPanleID.UIDeadToReborn);
        //RegistUI("UITooltips", UIPanleID.UITooltips);
        //RegistUI("UITaskInfoPanel", UIPanleID.UITaskInfoPanel);
        //RegistUI("UITaskList", UIPanleID.UITaskList);
        //RegistUI("UIDialogue", UIPanleID.UIDialogue);
        //RegistUI("UITaskRewardPanel", UIPanleID.UITaskRewardPanel);
        //RegistUI("UIMarquee", UIPanleID.UIMarquee);//跑马灯
        //RegistUI("UIPromptBox", UIPanleID.UIPromptBox);
        CreatPrefab("HeroPosEmbattle", 10, 100, 0);
    }
    
}
