using UnityEngine;
using System.Collections;
using Tianyu;
using System;

public class UIGuidePanel : GUIBase
{
    public GameObject go;
    
    public GameObject EffectButton;
   
    private static UIGuidePanel single;

    public SpinWithMouse spinDrug;

    public int testTimes = 0;

    public static UIGuidePanel Single()
    {
       
        return single;
    }

    public UIGuidePanel()
    {
        single = this;
    }


    void Awake()
    {
        EffectButton.transform.parent = null;
        GuideManager.Single().ChangeObjectPosition(EffectButton);
        GuideManager.Single().ChangeObjectScale(EffectButton);
        //    testTimes++;
        //EffectButton = Instantiate(Resources.Load("Effect/Prefabs/UI/yd_guangq")) as GameObject;
        //    // Debug.Log(EffectButton.gameObject.ToString());
        //    if (EffectButton == null)
        //    {
        //        Debug.Log("!!!Warning:EffectButton.name:" + EffectButton.name);
        //        Debug.Log("!!!Warning:EffectButton.Times:" + testTimes.ToString());
        //    }
        //    else
        //    {
        //        Debug.Log("!!!TEST:EffectButton.name:" + EffectButton.name);
        //        Debug.Log("!!!TEST:EffectButton.transform:" + EffectButton.transform);
        //        Debug.Log("!!!TEST:InitGuide tag:" + EffectButton.tag);
        //        Debug.Log("!!!TEST:EffectButton.Times:" + testTimes.ToString());
        //    }
    }

    public void InitGuide()
    {
        //Debug.Log("<color=#10DF11>InitGuide true</color>" + playerData.GetInstance().guideData.uId);
        if (playerData.GetInstance().guideData.uId != 0 && GameLibrary.UI_Major == Application.loadedLevelName)
        {
            switch (playerData.GetInstance().guideData.uId)
            {
                
                case 906:
                    go = UITaskTracker.instance.GuideBut;
                    GuideTarget();
                    //EffectButton.transform.localPosition = new Vector3(78, -24, 0);
                    break;
                case 919:
                    go = Control.GetComponent(919);
                    GuideTarget();
                    break;
                case 1219:
                    go = Control.GetComponent(1219);
                    GuideTarget();
                    break;
                case 2319:
                    go = Control.GetComponent(2319);
                    GuideTarget();
                    break;

                case 2719:
                    go = Control.GetComponent(2319);
                    GuideTarget();
                    break;

                case 2919:
                    go = Control.GetComponent(2919);
                    GuideTarget();
                    break;
                case 3019:
                    go = Control.GetComponent(3019);
                    GuideTarget();
                    break;

                case 1092:
                    go = Control.GetComponent(1092);
                    GuideTarget();
                    GetPanel();
                    break;

                case 1204:
                    go = Control.GetComponent(1204);
                    GuideTarget();
                    GetPanel();
                    break;

                case 1331:
                    go = Control.GetComponent(1331);
                    GuideTarget();
                    GetPanel();
                    break;
                    
                case 2471:
                    go = Control.GetComponent(2471);
                    GuideTarget();
                    GetPanel();
                    break;

                case 2871:
                    go = Control.GetComponent(2871);
                    GuideTarget();
                    GetPanel();
                    break;

                case 25110:
                    go = Control.GetComponent(25110);
                    GuideTarget();
                    GetPanel();
                    break;

                case 2697:

                    //EffectButton.transform.parent = UIEmbattle.instance.HeroButton1.transform;
                    //GuideManager.Single().ChangeObjectPosition(EffectButton);
                    //GuideManager.Single().ChangeObjectScale(EffectButton);
                    //EffectButton.GetComponent<RenderQueueModifier>().m_target = UIEmbattle.instance.HeroButton1.GetComponent<UIWidget>();
                    ////GetPanel();
                    //NextGuidePanel.Single().GuideDialogWin.GetComponent<RenderQueueModifier>().m_target = UIEmbattle.instance.HeroButton1.GetComponent<UIWidget>();
                    go = UIEmbattle.instance.HeroButton1;
                    GuideTarget();
                    GetPanel();
                    break;
               
                case 3194:
                    go = Control.GetComponent(3194);
                    GuideTarget();
                    GetPanel();
                    break;
                    
                case 3219:
                    go = Control.GetComponent(3219);
                    GuideTarget();
                    GetPanel();
                    break;

                case 33120:

                    //EffectButton.transform.parent = EquipDevelop.GetSingolton().selectFram.gameObject.transform;
                    //GuideManager.Single().ChangeObjectPosition(EffectButton);
                    //GuideManager.Single().ChangeObjectScale(EffectButton);
                    //EffectButton.GetComponent<RenderQueueModifier>().m_target = EquipDevelop.GetSingolton().selectFram.gameObject.GetComponent<UIWidget>();
                    go = EquipDevelop.GetSingolton().selectFram.gameObject;
                    GuideTarget();
                    GetPanel();

                    break;

                case 35120:

                    //EffectButton.transform.parent = EquipDevelop.GetSingolton().EvolveBtn.transform;
                    //GuideManager.Single().ChangeObjectPosition(EffectButton);
                    //GuideManager.Single().ChangeObjectScale(EffectButton);
                    //EffectButton.GetComponent<RenderQueueModifier>().m_target = EquipDevelop.GetSingolton().EvolveBtn.GetComponent<UIWidget>();
                    //////GetPanel();
                    //NextGuidePanel.Single().GuideDialogWin.GetComponent<RenderQueueModifier>().m_target = EquipDevelop.GetSingolton().EvolveBtn.GetComponent<UIWidget>();
                    go = EquipDevelop.GetSingolton().EvolveBtnLabel;
                    GuideTarget();
                    GetPanel();
                    break;

                case 36120:
                    //EffectButton.transform.parent = EquipDevelop.GetSingolton().selectFram.gameObject.transform;
                    //GuideManager.Single().ChangeObjectPosition(EffectButton);
                    //GuideManager.Single().ChangeObjectScale(EffectButton);
                    //EffectButton.GetComponent<RenderQueueModifier>().m_target = EquipDevelop.GetSingolton().selectFram.gameObject.GetComponent<UIWidget>();
                    //////GetPanel();
                    go = EquipDevelop.GetSingolton().selectFram.gameObject;
                    GuideTarget();
                    GetPanel();
                    break;

                case 34120:
                    go = Control.GetComponent(34120);
                    GuideTarget();
                    GetPanel();
                    break;

                case 37120:
                    go = Control.GetComponent(37120);
                    GuideTarget();
                    GetPanel();
                    break;

                case 38120:
                    go = Control.GetComponent(38120);
                    GuideTarget();
                    GetPanel();
                    break;

                case 3938:
                    go = Control.GetComponent(3938);
                    GuideTarget();
                    GetPanel();
                    break;

                case 4031:
                    go = Control.GetComponent(4031);
                    GuideTarget();
                    GetPanel();
                    break;

                case 4119:
                    go = Control.GetComponent(4119);
                    GuideTarget();
                    GetPanel();
                    break;

                case 4397:
                    go = Control.GetComponent(4397);
                    GuideTarget();
                    GetPanel();
                    break;

                case 4297:

                    //EffectButton.transform.parent = UIEmbattle.instance.HeroButton2.transform;
                    //GuideManager.Single().ChangeObjectPosition(EffectButton);
                    //GuideManager.Single().ChangeObjectScale(EffectButton);
                    //EffectButton.GetComponent<RenderQueueModifier>().m_target = UIEmbattle.instance.HeroButton2.GetComponent<UIWidget>();
                    //NextGuidePanel.Single().GuideDialogWin.GetComponent<RenderQueueModifier>().m_target = UIEmbattle.instance.HeroButton2.GetComponent<UIWidget>();
                    //////GetPanel();
                    go = UIEmbattle.instance.HeroButton2;
                    GuideTarget();
                    GetPanel();
                    break;

                case 4419:

                    go = Control.GetComponent(1219);
                    GuideTarget();
                    break;

                case 4531:
                    go = Control.GetComponent(4531);
                    GuideTarget();
                    GetPanel();
                    break;

                case 4638:
                    go = Control.GetComponent(3938);
                    GuideTarget();
                    GetPanel();
                    break;

                case 4731:
                    go = Control.GetComponent(4031);
                    GuideTarget();
                    GetPanel();
                    break;

                case 4897:
                    if (UIEmbattle.instance!=null)
                    go = UIEmbattle.instance.HeroButton2;
                    GuideTarget();
                    GetPanel();
                    break;

                case 4997:
                    go = Control.GetComponent(4397);
                    GuideTarget();
                    GetPanel();
                    break;

                default:
                    break;
            }
        }
       
    }

    public void GuideTarget()
    {
        if (go != null&& EffectButton!=null)
        {
            //Debug.Log("<color=#10DF11>GuideTarget 生成指引特效</color>" + playerData.GetInstance().guideData.uId);
            //Debug.Log("go.name:" + go.name);

            EffectButton.transform.parent = go.transform;
            //Debug.Log("EffectButton.transform.parent:" + EffectButton.transform.parent.name);
            //Debug.Log("EffectButtonPosition:" + EffectButton.transform.localPosition);
            //Debug.Log("EffectButtonPosition:" + EffectButton.transform.position);
            //Debug.Log("goPosition:" + go.transform.position);
            //Debug.Log("EffectButtonScale:" + EffectButton.transform.localScale);
            GuideManager.Single().ChangeObjectPosition(EffectButton);
            GuideManager.Single().ChangeObjectScale(EffectButton);
            //Debug.Log("EffectButtonPosition:" + EffectButton.transform.localPosition);
            //Debug.Log("EffectButtonPosition:" + EffectButton.transform.position);
            //Debug.Log("goPosition:" + go.transform.position);
            //Debug.Log("EffectButtonScale:" + EffectButton.transform.localScale);
            EffectButton.GetComponent<RenderQueueModifier>().m_target = go.GetComponent<UIWidget>();
            // Debug.Log("UIWidget:" + go.GetComponent<UIWidget>());
            // Debug.Log("UIWidget:" + EffectButton.GetComponent<RenderQueueModifier>().m_target);
            //  NextGuidePanel.Single().GuideDialogWin.GetComponent<RenderQueueModifier>().m_target = go.GetComponent<UIWidget>();
            PlayGuideMusic();
        }
        else
        {
            int panleID = 0;
            switch (playerData.GetInstance().guideData.uId)
            {
                case 919:
                    panleID = 19;
                    break;

                case 906:
                    panleID = 106;
                    break;

                case 1092:
                    panleID = 92;
                    break;

                case 1204:
                    panleID = 104;
                    break;

                case 1219:
                    panleID = 19;
                    break;

                case 3194:
                    panleID = 94;
                    break;

                case 2319:
                    panleID = 19;
                    break;

                case 2719:
                    panleID = 19;
                    break;

                case 2919:
                    panleID = 19;
                    break;

                case 3019:
                    panleID = 19;
                    break;

                case 2269:
                    panleID = 69;
                    break;

                case 2471:
                    panleID = 71;
                    break;

                case 2871:
                    panleID = 71;
                    break;

                case 25110:
                    panleID = 110;
                    break;

                case 2697:
                    panleID = 97;
                    break;

                //case 1419:
                //    guideId = 14;
                //    break;

                case 1331:
                    panleID = 31;
                    break;

                case 3219:
                    panleID = 19;
                    break;

                case 33120:
                    panleID = 120;
                    break;

                case 34120:
                    panleID = 120;
                    break;

                case 35120:
                    panleID = 120;
                    break;

                case 36120:
                    panleID = 120;
                    break;

                case 37120:
                    panleID = 120;
                    break;

                case 38120:
                    panleID = 120;
                    break;

                case 3938:
                    panleID = 37;
                    break;

                case 4031:
                    panleID = 31;
                    break;

                case 4119:
                    panleID = 19;
                    break;

                case 4297:
                    panleID = 97;
                    break;

                case 4397:
                    panleID = 97;
                    break;

                case 4419:
                    panleID = 19;
                    break;

                case 4531:
                    panleID = 31;
                    break;

                case 4638:
                    panleID = 37;
                    break;

                case 4731:
                    panleID = 31;
                    break;

                case 4897:
                    panleID = 97;
                    break;

                case 4997:
                    panleID = 97;
                    break;
                default:
                    break;
            }
            if (Control.GetUI<GUIBase>((UIPanleID) panleID) != null)
                Control.GetUI<GUIBase>((UIPanleID) panleID).UIID = 1;
            //Debug.Log("!!!gjhjh!!! " + name+"====>Control.GetGUI(name).UIID"+ Control.GetGUI(name).UIID);

        }
        //Debug.Log("!!TRUE OK!! ");
    }
    public void uicallback()
    {
        InitGuide();
    }

    IEnumerator setCameraGuideLayer(GameObject go)
    {
        yield return null;
      
    }

    public void GetPanel()
    {
        if (go != null && EffectButton != null)
        {
            go.transform.parent.GetComponent<UIPanel>();
            if (go.transform.parent.GetComponent<UIPanel>())
                NextGuidePanel.Single().content.GetComponent<UIPanel>().depth = go.transform.parent.GetComponent<UIPanel>().depth;
            else if (go.transform.parent.parent.GetComponent<UIPanel>())
                NextGuidePanel.Single().content.GetComponent<UIPanel>().depth = go.transform.parent.parent.GetComponent<UIPanel>().depth;
            else if (go.transform.parent.parent.parent.GetComponent<UIPanel>())
                NextGuidePanel.Single().content.GetComponent<UIPanel>().depth = go.transform.parent.parent.parent.GetComponent<UIPanel>().depth;
            else if (go.transform.parent.parent.parent.parent.GetComponent<UIPanel>())
                NextGuidePanel.Single().content.GetComponent<UIPanel>().depth = go.transform.parent.parent.parent.parent.GetComponent<UIPanel>().depth;
            else if (go.transform.parent.parent.parent.parent.parent.GetComponent<UIPanel>())
                NextGuidePanel.Single().content.GetComponent<UIPanel>().depth = go.transform.parent.parent.parent.parent.parent.GetComponent<UIPanel>().depth;
        }
        //NextGuidePanel.Single().content.GetComponent<UIPanel>().depth;
       
    }

    public void PlayGuideMusic()
    {
        int guideId = 0;
        switch (playerData.GetInstance().guideData.uId)
        {
            case 919:
                guideId = 9;
                break;

            case 906:
                guideId = 8;
                break;

            case 1092:
                guideId = 10;
                break;

            case 1204:
                guideId = 11;
                break;

            case 1219:
                guideId = 12;
                break;

            //case 2125:
            //    guideId = 21;
            //    break;

            case 2319:
                guideId = 23;
                break;

            case 2719:
                guideId = 27;
                break;

            case 2919:
                guideId = 29;
                break;

            case 3019:
                guideId = 30;
                break;

            case 2269:
                guideId = 22;
                break;

            case 2471:
                guideId = 24;
                break;

            case 2871:
                guideId = 28;
                break;

            case 25110:
                guideId = 25;
                break;

            case 2697:
                guideId = 26;
                break;

            //case 1419:
            //    guideId = 14;
            //    break;

            case 1331:
                guideId = 13;
                break;

            case 3219:
                guideId = 32;
                break;

            case 33120:
                guideId = 33;
                break;

            case 34120:
                guideId = 34;
                break;

            case 35120:
                guideId = 35;
                break;

            case 36120:
                guideId = 36;
                break;

            case 37120:
                guideId = 37;
                break;

            case 38120:
                guideId = 38;
                break;

            case 3938:
                guideId = 39;
                break;

            case 4031:
                guideId = 40;
                break;

            case 4119:
                guideId = 41;
                break;

            case 4297:
                guideId = 42;
                break;

            case 4397:
                guideId = 43;
                break;

            case 4419:
                guideId = 44;
                break;

            case 4531:
                guideId = 45;
                break;

            case 4638:
                guideId = 46;
                break;

            case 4731:
                guideId = 47;
                break;

            case 4897:
                guideId = 48;
                break;

            case 4997:
                guideId = 49;
                break;
            default:
                break;
        }
        if (FSDataNodeTable<GuideNode>.GetSingleton().DataNodeList.ContainsKey(guideId))
        {
            string musicName;
            musicName = FSDataNodeTable<GuideNode>.GetSingleton().DataNodeList[guideId].voice;
            //Debug.Log("<color=#10DF11>musicName:::</color>" + musicName);
            if (musicName != null && musicName != "0")
            {

                AudioController.Instance.PlayUISound(GameLibrary.Resource_GuideSound + musicName, true);
                //Debug.Log("<color=#10DF11>Guide music 播放指引音效</color>" + playerData.GetInstance().guideData.uId);
            }
        }
    }

    public override UIPanleID GetUIKey()
    {
        return UIPanleID.none;
    }
}
