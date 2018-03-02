using UnityEngine;
using System.Collections.Generic;
using Tianyu;

public class GuideManager  {

    public Vector3 DialogWinPosition;

    public Vector3 GuideModelPosition;

    public static bool isGuide = true;//是否完成所有的新手引导 

    public GameObject GuideModel;

    public UISprite DialogWin;

    public int GuideId;

    public int NevtGuideId;
    
    private static GuideManager single;

    public static GuideManager Single()
    {
        if (single == null)
        {
            single = new GuideManager();
        }
        return single;
    }
    static uint[] FixedSize1 = new uint[]
    {
            0x0,
            0x01,           0x03,           0x07,           0x0F,
            0x1F,           0x3F,           0x7F,           0xFF,
            0x01FF,         0x03FF,         0x07FF,         0x0FFF,
            0x1FFF,         0x3FFF,         0x7FFF,         0xFFFF,
            0x01FFFF,       0x03FFFF,       0x07FFFF,       0x0FFFFF,
            0x1FFFFF,       0x3FFFFF,       0x7FFFFF,       0xFFFFFF,
            0x01FFFFFF,     0x03FFFFFF,     0x07FFFFFF,     0x0FFFFFFF,
            0x1FFFFFFF,     0x3FFFFFFF,     0x7FFFFFFF,     0xFFFFFFFF,
    };

    public int GetBitValue(uint value, short start, short size)
    {
        uint _mask = FixedSize1[size];
        long _data = (value >> start) & _mask;
        return (int)_data;
    }
     public void InitData()
    {
        playerData.GetInstance().guideData.guideId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[831], 17, 16);

        //playerData.GetInstance().guideData.scripId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[830], 0, 16);
        //playerData.GetInstance().guideData.typeId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[831], 0, 16);
        //playerData.GetInstance().guideData.stepId = GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[831], 17, 16);

      

    }

  

    /// <summary>
    /// 找到点击目标物体
    /// </summary>
    /// <param name="go"></param>
    public void SetObject(GameObject go)
    {
        if (UIGuidePanel.Single() == null || NextGuidePanel.Single() == null)
            return;
        //Debug.Log("<color=#10DF11>Guide 点击目标:::</color>" + go.name);
        if (go.name == "bc")
        {
            
            if (playerData.GetInstance().guideData.uId == 906)
            {
                NextGuide();
            }
           
        }
        else if (go.name == "EctypeBtn")

        {
            if (playerData.GetInstance().guideData.uId == 919)
            {
                NextGuide();
                
            }
            
        }
        else if (go.name == "LevelScene")
        {
            if (playerData.GetInstance().guideData.uId == 1092)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if(go.name == "BtnBattle")
        {
            if (playerData.GetInstance().guideData.uId == 1204)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }

        }
        else if (go.name == "HeroBtn")
        {
            if (playerData.GetInstance().guideData.uId == 1419)
            {
                NextGuide();
            }
            else if (playerData.GetInstance().guideData.uId == 906 || playerData.GetInstance().guideData.uId == 919 || playerData.GetInstance().guideData.uId == 1219 || playerData.GetInstance().guideData.uId == 4419
                  || playerData.GetInstance().guideData.uId == 2319 || playerData.GetInstance().guideData.uId == 2719 || playerData.GetInstance().guideData.uId == 2919 || playerData.GetInstance().guideData.uId == 3019
                   || playerData.GetInstance().guideData.uId == 3219)
            {
                NextGuidePanel.Single().content.SetActive(false);
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "AltarBtn")
        {
            if (playerData.GetInstance().guideData.uId == 1219 || playerData.GetInstance().guideData.uId == 4419)
            {
                //Debug.Log("<color=#10DF11>NextGuide:::</color>" + go.name);
                NextGuide();
            }
           
        }
        else if (go.name == "LeftBtn")
        {
            if (playerData.GetInstance().guideData.uId == 1331 || playerData.GetInstance().guideData.uId == 4531)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "GoldBtn")
        {
            if (playerData.GetInstance().guideData.uId == 2125)
            {
                NextGuide();
            }
            else if (playerData.GetInstance().guideData.uId == 906 || playerData.GetInstance().guideData.uId == 919 || playerData.GetInstance().guideData.uId == 1219 || playerData.GetInstance().guideData.uId == 4419
                  || playerData.GetInstance().guideData.uId == 2319 || playerData.GetInstance().guideData.uId == 2719 || playerData.GetInstance().guideData.uId == 2919 || playerData.GetInstance().guideData.uId == 3019
                   || playerData.GetInstance().guideData.uId == 3219)
            {
                NextGuidePanel.Single().content.SetActive(false);
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "1V1")
        {
            if (playerData.GetInstance().guideData.uId == 25110)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "ArenaABtn")
        {
            if (playerData.GetInstance().guideData.uId == 2719 || playerData.GetInstance().guideData.uId == 2319)
            {
                NextGuide();
            }
           
        }
        else if (go.name == "ArenaBtn")
        {
            if (playerData.GetInstance().guideData.uId == 2471)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "AbattoirBtn")
        {
            if (playerData.GetInstance().guideData.guideId == 2871)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "Icon")
        {
            if (playerData.GetInstance().guideData.uId == 2697 || playerData.GetInstance().guideData.uId == 4297 || playerData.GetInstance().guideData.uId == 4897)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
       
        else if (go.name == "ShopBtn")
        {
            if (playerData.GetInstance().guideData.uId == 3019)
            {
                NextGuide();
            }
           
        }
        else if (go.name == "EnchantBtn")
        {
            if (playerData.GetInstance().guideData.uId == 2919)
            {
                NextGuide();
            }
           
        }
        else if (go.name == "EquipBtn")
        {
            if (playerData.GetInstance().guideData.uId == 3219)
            {
                NextGuide();
            }
          
        }
        else if (go.name == "IconBtn")
        {
            if (playerData.GetInstance().guideData.uId == 33120 || playerData.GetInstance().guideData.uId == 36120)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }

        }
        else if (go.name == "StrengthenThriceBtn")
        {
            if (playerData.GetInstance().guideData.uId == 34120)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "EvolveBtn")
        {
            if (playerData.GetInstance().guideData.uId == 35120)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "OneEvolvesBtn")
        {
            if (playerData.GetInstance().guideData.uId == 37120)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "BackBtn")
        {
            if (playerData.GetInstance().guideData.uId == 38120 || playerData.GetInstance().guideData.uId == 4031 || playerData.GetInstance().guideData.uId == 4731)
            {
                NextGuide();
            }
            else if (playerData.GetInstance().guideData.uId == 906 || playerData.GetInstance().guideData.uId == 919 || playerData.GetInstance().guideData.uId == 1219 || playerData.GetInstance().guideData.uId == 4419
                  || playerData.GetInstance().guideData.uId == 2319 || playerData.GetInstance().guideData.uId == 2719 || playerData.GetInstance().guideData.uId == 2919 || playerData.GetInstance().guideData.uId == 3019
                   || playerData.GetInstance().guideData.uId == 3219)
            {
                NextGuidePanel.Single().content.SetActive(false);
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }

        }
        else if (go.name == "OKBtn")
        {
            if (playerData.GetInstance().guideData.uId == 3938 || playerData.GetInstance().guideData.uId == 4638)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "Embattle")
        {
            if (playerData.GetInstance().guideData.uId == 4119)
            {
                NextGuide();
            }
          
        }
        else if (go.name == "ConfirmBtn")
        {
            if (playerData.GetInstance().guideData.uId == 4397)
            {
                NextGuide();
            }
            else
            {
                NoNextGuide();
                NoNextGuideSelect();
            }
        }
        else if (go.name == "UITaskRewardPanel")//任务完成IU
        {
            return;
        }
        else if (go.name == "Upgrade")//升级提示IU
        {
            return;
        }
        else if (go.name == "Close")//
        {
            //Debug.Log("<color=#10DF11>Guide 点击目标:::</color>" + go.name);
            return;
        }
        else if (go.name == "SpriteBtn")//
        {
            //Debug.Log("<color=#10DF11>Guide 点击目标:::</color>" + go.name);
            return;
        }
        else
        {
            if (GameLibrary.UI_Major == Application.loadedLevelName)
            {
                if (playerData.GetInstance().guideData.uId == 906 || playerData.GetInstance().guideData.uId == 919 || playerData.GetInstance().guideData.uId == 1219 || playerData.GetInstance().guideData.uId == 4419
                    || playerData.GetInstance().guideData.uId == 2319 || playerData.GetInstance().guideData.uId == 2719 || playerData.GetInstance().guideData.uId == 2919 || playerData.GetInstance().guideData.uId == 3019
                     || playerData.GetInstance().guideData.uId == 3219)
                {
                    NextGuidePanel.Single().content.SetActive(false);
                }
                else
                {
                    NoNextGuide();
                    NoNextGuideSelect();
                }
              
            }

        }

    }
    /// <summary>
    /// 继续引导处理
    /// </summary>
    public void NextGuide()
    {
        //Debug.Log("<color=#10DF11>NextGuide uId:::</color>" + playerData.GetInstance().guideData.uId);
        if (null != UIGuidePanel.Single() && null != UIGuidePanel.Single().EffectButton)
        {
            UIGuidePanel.Single().EffectButton.transform.parent = null;
            ChangeObjectPosition(UIGuidePanel.Single().EffectButton);
            //Debug.Log("<color=#10DF11>NextGuide scripId:::</color>" + playerData.GetInstance().guideData.scripId);
            //if (playerData.GetInstance().guideData.scripId != 0)
            ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(0);
            NextGuidePanel.Single().Close();
        }
    }

    public void NoNextGuide()
    {
        if (UIGuidePanel.Single().EffectButton)
        {
            UIGuidePanel.Single().EffectButton.transform.parent = null;
            ChangeObjectPosition(UIGuidePanel.Single().EffectButton);
            NextGuidePanel.Single().Close();
        }
      
    }

    public void NoNextGuideSelect()
    {
        switch (playerData.GetInstance().guideData.uId)
        {
            case 1092:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 1, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            case 1204:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 1, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            case 1331:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 2, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            case 2269:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 3, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            case 2471:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 4, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            case 25110:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 4, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            case 2697:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 4, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            case 2871:
                if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 5, 1) != 1)
                {
                    ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

                }
                break;

            //case 4997:
            //    if (GuideManager.Single().GetBitValue(playerData.GetInstance().selfData.infodata[833], 5, 1) != 1)
            //    {
            //        ClientSendDataMgr.GetSingle().GetGuideSend().SendGuidStep(99);

            //    }
            //    break;
            default:
                break;
        }
    }

    /// <summary>
    /// 动态移动后位置重置
    /// </summary>
    /// <param name="go"></param>
    public void ChangeObjectPosition(GameObject go)
    {
       go.transform.localPosition = Vector3.zero;
    }
    /// <summary>
    /// 动态移动后旋转重置
    /// </summary>
    /// <param name="go"></param>
    public void ChangeObjectRotation(GameObject go)
    {
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    /// <summary>
    /// 动态移动后缩放重置
    /// </summary>
    /// <param name="go"></param>
    public void ChangeObjectScale(GameObject go)
    {
        go.transform.localScale = Vector3.one;
    }
}
