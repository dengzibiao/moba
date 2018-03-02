using UnityEngine;
//using MojingSample.CrossPlatformInput;
public class EasyTouch : MonoBehaviour
{
    public static EasyTouch instance;

    GUIBase[] uiarray = new GUIBase[3];

    Transform up;
    Vector3 initPos;
    Vector3 clamp = Vector3.zero;
    float range = 80;
    float ratio;
    UIRoot root;

    void Awake()
    {
        instance = this;
        up = transform.Find("up");
        initPos = transform.position;
        isUIPress = false;
    }

    void Start()
    {
        uiarray[0] = UI_Setting.GetInstance();
        TweenAlpha.Begin(gameObject, 0.2f, 0.5f);

        root = NGUITools.GetRoot(gameObject).GetComponent<UIRoot>();
        ratio = NGUITools.screenSize.y / root.manualHeight;
        range = ratio * range;
    }

    void OnPress(bool isPress)
    {
        isUIPress = isPress;
        TweenAlpha.Begin(gameObject, 0.2f, isPress ? 1f : 0.5f);

        if(!isPress)
        {
            transform.position = initPos;
            up.localPosition = Vector3.zero;
            upPosition = Vector3.zero;
        }
        else
        {
            startPos = UICamera.currentTouch.pos;
            startPos.x = Mathf.Clamp(startPos.x, ratio * 120, ratio * 280);
            startPos.y = Mathf.Clamp(startPos.y, ratio * 120, ratio * 280);
            transform.position = UICamera.currentCamera.ScreenToWorldPoint(startPos);
        }
    }

    Vector2 startPos;
    void FixedUpdate ()
    {
        if(isUIPress)
        {
            if(UICamera.lastEventPosition.x < 0.32f * NGUITools.screenSize.x && UICamera.lastEventPosition.y < 0.56f * NGUITools.screenSize.y)
            {
                Vector2 rangePos = UICamera.lastEventPosition;
                if((startPos - UICamera.lastEventPosition ).magnitude >= range)
                    rangePos = startPos - (startPos - UICamera.lastEventPosition).normalized * range;
                up.position = UICamera.currentCamera.ScreenToWorldPoint(rangePos);
                clamp.x = Mathf.Clamp(up.localPosition.x, -range, range);
                clamp.y = Mathf.Clamp(up.localPosition.y, -range, range);
                upPosition = clamp / range;
            }
        }
    }

    public bool isUIPress { set; get; }
    public Vector3 upPosition { set; get; }
    public byte mVerticalIndex = 0;//纵列索引
    public byte mVIndex = 0;//横列索引
    public byte UIIndex = 0;
    bool bVertical;//是否是纵列
    public bool bUIControl = false;
    public UISprite currentbutton;
    //void Update()
    //{
    //    if (CrossPlatformInputManager.GetButtonDown("OK"))
    //    {
    //        str = "OK-----GetButtonDown";
    //        if(!bUIControl)
    //        {
    //            bUIControl = true;
    //            mVerticalIndex = 0;
    //            mVIndex = 0;
    //        }
           
            
    //    }
    //    if (CrossPlatformInputManager.GetButton("OK"))
    //    {
    //        str = "OK-----GetButton";
    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("OK") || CrossPlatformInputManager.GetButtonUp("Submit"))
    //    {
    //        if(bUIControl)
    //        {
    //            OnOK();
    //        }
    //        str ="OK-----GetButtonUp";
    //        //if (menu_controller != null && glass_controller != null)
    //        //{
    //        //    if (UIListController.show_flag)//镜片选择的二级菜单
    //        //        glass_controller.PressCurrent();
    //        //    else//场景选择的一级菜单
    //        //        menu_controller.PressCurrent();
    //        //}
    //    }

    //    if (CrossPlatformInputManager.GetButtonDown("C"))
    //    {
    //        str ="C-----GetButtonDown";
    //    }
    //    if (CrossPlatformInputManager.GetButton("C"))
    //    {
    //        str = "C-----GetButton";
    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("C"))
    //    {
    //        //#if UNITY_IOS
    //        //	MojingSDK.Unity_StopTracker();
    //        //#endif
    //        //  Application.Quit();
    //        if (bUIControl)
    //        {
    //            bUIControl = false;
    //            mVerticalIndex = 0;
    //            mVIndex = 0;
    //        }
    //    }

    //    if (CrossPlatformInputManager.GetButtonDown("MENU"))
    //    {
    //        str = "MENU-----GetButtonDown";
    //    }
    //    if (CrossPlatformInputManager.GetButton("MENU"))
    //    {
    //        str = "MENU-----GetButton";
    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("MENU"))
    //    {
    //       str ="MENU-----GetButtonUp";
    //    }

    //    if (CrossPlatformInputManager.GetButton("UP"))
    //    {
           
    //        if(!bUIControl)
    //        {
    //            if (iscenter)
    //                iscenter = false;
    //           str ="UP-----GetButton";
    //            up.localPosition += new Vector3(0, range, 0);
    //            clamposition();
    //        }

    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("UP"))
    //    {
    //        if (bUIControl)
    //        {
    //            mVerticalIndex += 1;
    //            if (mVerticalIndex > 4)
    //            {
    //                mVerticalIndex = 0;
    //            }

    //              ChangeHSelect();
    //          //  UI_Setting.GetInstance().nextUp();
    //           // uiarray[uiindex].nextUp();
    //             bVertical = true;
    //        }
    //        str = "Up ----GetButtonUp";
    //        //    if (menu_controller != null && glass_controller != null)
    //        //    {
    //        //        if (UIListController.show_flag)
    //        //            glass_controller.HoverPrev();
    //        //        else
    //        //            menu_controller.HoverPrev();
    //        //    }
    //    }

    //    if (CrossPlatformInputManager.GetButton("DOWN"))
    //    {
    //        if (!bUIControl)
    //        {
    //            if (iscenter)
    //                iscenter = false;
    //            up.localPosition += new Vector3(0, -range, 0);
    //            clamposition();
    //        }
    //        str = "DOWN-----GetButton";
    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("DOWN"))
    //    {
    //        if (bUIControl)
    //        {
    //            mVerticalIndex -= 1;
    //            if (mVerticalIndex < 0)
    //            {
    //                mVerticalIndex = 4;
    //            }
    //            ChangeHSelect();
    //            bVertical = true;
    //        }
    //        str = "DOWN-----GetButtonUp";
    //        //if (menu_controller != null && glass_controller != null)
    //        //{
    //        //    if (UIListController.show_flag)
    //        //        glass_controller.HoverNext();
    //        //    else
    //        //        menu_controller.HoverNext();
    //        //}
    //    }

    //    if (CrossPlatformInputManager.GetButton("LEFT"))
    //    {
    //        if (!bUIControl)
    //        {
    //            if (iscenter)
    //                iscenter = false;
    //            str = "LEFT-----GetButton";
    //            up.localPosition += new Vector3(-range, 0, 0);
    //            clamposition();
    //        }
    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("LEFT"))
    //    {
    //        if (bUIControl)
    //        {
    //            mVIndex -= 1;
    //            if (mVIndex < 0)
    //            {
    //                mVIndex = 4;
    //            }
    //            ChangeHSelect();
    //            bVertical = false;
    //        }
    //        str = "LEFT-----GetButtonUp";
    //        //if (menu_controller != null && glass_controller != null)
    //        //{
    //        //    if (!UIListController.show_flag)
    //        //        menu_controller.HoverLeft();
    //        //}
    //    }

    //    if (CrossPlatformInputManager.GetButton("RIGHT"))
    //    {
    //        if (!bUIControl)
    //        {
    //            if (iscenter)
    //                iscenter = false;
    //           str = "RIGHT-----GetButton";
    //            up.localPosition += new Vector3(range, 0, 0);
    //            clamposition();
               
    //        }
    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("RIGHT"))
    //    {
    //        if (bUIControl)
    //        {
    //            mVIndex += 1;
    //            if (mVIndex >4)
    //            {
    //                mVIndex = 0;
    //            }
    //            ChangeHSelect();
    //            bVertical = false;
    //        }
    //        str = "RIGHT-------GetButtonUp";
    //        //if (menu_controller != null && glass_controller != null)
    //        //{
    //        //    if (!UIListController.show_flag)
    //        //        menu_controller.HoverRight();
    //        //}
    //    }

    //    if (CrossPlatformInputManager.GetButton("CENTER"))
    //    {
    //        if (!iscenter)
    //        {
    //            transform.position = local;
    //            up.localPosition = Vector3.zero;
    //            upPosition = Vector3.zero;
    //            str = "CENTER-----GetButton";
    //            iscenter = true;
    //        }
           
    //    }
    //    else if (CrossPlatformInputManager.GetButtonUp("CENTER"))
    //    {
    //        str ="CENTER-----GetButtonUp";
    //    }
    //}
    string str = "";

    void OnGUI()
    {
        GUI.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.fontSize = 100;
        style.normal.textColor = Color.red;
        if (str != "")
        {
            GUI.Label(new Rect(10, 150, 400, 400), "key:" + str, style);
        }
    }

    void ChangeHSelect()
    {
        currentbutton.color = new Color(1, 1, 1);
        switch (mVerticalIndex)
        {
            
            case 0:                
                currentbutton = UI_Setting.GetInstance().shrinkBtn.GetComponent<UISprite>();//功能
                break;
            case 1:
                currentbutton = UI_Setting.GetInstance().shopBtn.GetComponent<UISprite>();//商店
                break;
            case 2:
                currentbutton = UI_Setting.GetInstance().ectypeBtn.GetComponent<UISprite>();//副本
                break;
            case 3:
                currentbutton = UI_Setting.GetInstance().taskBtn.GetComponent<UISprite>();//任务
                break;
            case 4:
                currentbutton = UI_Setting.GetInstance().enchantBtn.GetComponent<UISprite>();//附魔
                break;
        }
        str = "mVertical = " + mVerticalIndex;
        currentbutton.color = new Color(1, 0, 0);
    }

    void OnOK()
    {
       // GUISingleButton singlebutton = currentbutton.GetComponent<GUISingleButton>();
        //if(singlebutton !=null)
       // singlebutton.onClick();
        str = "onOk = " ;
        if(!bVertical)
        {
            switch (mVerticalIndex)
            {
                case 0:
                    UI_Setting.GetInstance().OnShrinkClick();//功能
                    break;
                case 1:
                     UI_Setting.GetInstance().OnBagClick();//物品，背包
                    break;
                case 2:
                     UI_Setting.GetInstance().OnEventDungBtnClick();//布阵
                    break;
                case 3:
                     UI_Setting.GetInstance().OnHeroBtnClick();//英雄
                    break;
                case 4:
                     UI_Setting.GetInstance().OnAltarClick();//祭坛
                    break;
            }
        }
        else
        {
            switch (mVerticalIndex)
            {

                case 0:
                     UI_Setting.GetInstance().OnShrinkClick();//功能
                    break;
                case 1:
                     UI_Setting.GetInstance().OnShopBtnClick();//商店
                    break;
                case 2:
                     UI_Setting.GetInstance().ectypeBtn.GetComponent<UISprite>();//副本
                    break;
                case 3:
                     UI_Setting.GetInstance().taskBtn.GetComponent<UISprite>();//任务
                    break;
                case 4:
                     UI_Setting.GetInstance().enchantBtn.GetComponent<UISprite>();//附魔
                    break;
            }
        }


    }
    void ChangeVSelect()
    {
        currentbutton.color = new Color(1, 1, 1);
        switch (mVIndex)
        {
            case 0:
                currentbutton = UI_Setting.GetInstance().shrinkBtn.GetComponent<UISprite>();//功能
                break;
            case 1:
                currentbutton = UI_Setting.GetInstance().bagBtn.GetComponent<UISprite>();//物品，背包
                break;
            case 2:
                currentbutton = UI_Setting.GetInstance().eventDungBtn.GetComponent<UISprite>();//布阵
                break;
            case 3:
                currentbutton = UI_Setting.GetInstance().heroBtn.GetComponent<UISprite>();//英雄
                break;
            case 4:
                currentbutton = UI_Setting.GetInstance().altarBtn.GetComponent<UISprite>();//祭坛
                break;
        }
        str = "mVertical = " + mVerticalIndex;
        currentbutton.color = new Color(1, 0, 0);
    }
}
