using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Control
{
    /// <summary>
    /// ui，isRepel = false排斥其他界面，args被关闭的界面
    /// </summary>
    public static GUIBase Show(UIPanleID uikey)
    {
        return Singleton<GUIManager>.Instance.Show(uikey);
    }
    /// <summary>
    /// 注册组件
    /// </summary>
    /// <param name="panleID"></param>
    /// <param name="id"></param>
    /// <param name="go"></param>
    public static void RegisterComponentID(int panleID, int id, GameObject go)
    {
        int b = int.Parse(panleID.ToString() + id.ToString());
        Singleton<GUIManager>.Instance.RegisterComponent(b, go);
    }
   /// <summary>
   /// 打开界面
   /// </summary>
   /// <param name="uikey"></param>
   /// <param name="openType">打开界面的类型</param>
   /// <param name="isCloseAllUI">是否关闭所有UI</param>
   /// <param name="uiObjParams">可传参数</param>
    public static void ShowGUI(UIPanleID uikey, EnumOpenUIType openType, bool isCloseAllUI=false, params object[] uiObjParams)
    {
        if(openType== EnumOpenUIType.DefaultUIOrSecond)
        Singleton<GUIManager>.Instance.OpenUI(uikey, isCloseAllUI, uiObjParams);
        else if(openType== EnumOpenUIType.OpenNewCloseOld) Singleton<GUIManager>.Instance.OpenMutexUI(isCloseAllUI, uikey,uiObjParams);
    }
    /// <summary>
    /// 打开一组界面
    /// </summary>
    /// <param name="uikeys"></param>
    /// <param name="isCloseAllUI"></param>
    public static void ShowGUI(UIPanleID[] uikeys, EnumOpenUIType openType, bool isCloseAllUI = false)
    {
        if (openType == EnumOpenUIType.DefaultUIOrSecond)
            Singleton<GUIManager>.Instance.OpenUI(uikeys, isCloseAllUI);
        else if (openType == EnumOpenUIType.OpenNewCloseOld) Singleton<GUIManager>.Instance.OpenMutexUI(isCloseAllUI, uikeys);
      
    }
    /// <summary>
    ///  关闭当前，打开上一个界面
    /// </summary>
    /// <param name="isCloseAllUI">默认为不关闭全部UI，true则关闭所有UI，包括跳转场景后要打开的缓存界面</param>
    public static void HideGUI(bool isCloseAllUI = false)
    {
        Singleton<GUIManager>.Instance.CloseMutexUI(isCloseAllUI);
    }
    /// <summary>
    /// 关闭默认界面或者二级界面时使用
    /// </summary>
    /// <param name="uiKey"></param>
    public static void HideGUI(UIPanleID uiKey)
    {
        Singleton<GUIManager>.Instance.CloseUI(uiKey);
    }
    /// <summary>
    /// 返回全屏UI界面列表
    /// </summary>
    /// <returns></returns>
    public static List<UIPanleID> GetFullScreenUIList()
    {
        return Singleton<GUIManager>.Instance.GetFullScreenUI();
    }
    /// <summary>
    /// 通过界面ID获取界面上的脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uiKey"></param>
    /// <returns></returns>
    public static GUIBase GetUI<T>(UIPanleID uiKey)
    {
        return Singleton<GUIManager>.Instance.GetUI<GUIBase>(uiKey);
    }
    /// <summary>
    /// 通过界面ID获取界面GameObject
    /// </summary>
    /// <param name="uiKey"></param>
    /// <returns></returns>
    public static GameObject GetUIObject(UIPanleID uiKey)
    {
        return Singleton<GUIManager>.Instance.GetUIObject(uiKey);
    }
    public static void AddOrDeletFullScreenUI(UIPanleID uiKey,bool isAdd=true)
    {
        Singleton<GUIManager>.Instance.AddOrDeletFullScreenUI(uiKey, isAdd);
    }
    //获取指定界面的控件
    public static GameObject GetComponent(int id)
    {
        return Singleton<GUIManager>.Instance.GetComponent(id);
    }

    public static void Hide(UIPanleID uikey)
    {
        Singleton<GUIManager>.Instance. Hide(uikey);
    }
    /// <summary>
    /// parent可以为null,也可以自定义,float为vector3.posation的参数,path预制体路径(默认是加载UI路径)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="parent"></param>
    public void CreatPrefab(string name, float a, float b, float c, GameObject parent = null, string path = null)
    {
        Vector3 aa = new Vector3(a, b, c);
        if (aa != default(Vector3))
        {
            ScenesManage.Instance.CreatPrefab(name, parent, aa, path);
        }
        else
        {
            aa = default(Vector3);
            ScenesManage.Instance.CreatPrefab(name, parent, aa, path);
        }

    }
    /// <summary>
    /// 转换场景时需要调用清空引用
    /// </summary>
    public static void ClearGUI()
    {
        Singleton<GUIManager>.Instance.Clear();
    }

    //打开窗口播放某个背景音乐
    public static void PlayBgmWithUI(UIPanleID panelId)
    {
        string bgm = GetBgmByPanelId(panelId);
        if (!AudioController.Instance.bgmPlaying.Equals(bgm))
        {
            AudioController.Instance.PlayBackgroundMusic(bgm, true);
        }
    }

    public static void PlayBGmByClose(UIPanleID panelId)
    {
        string bgm = GetBgmByPanelId(panelId);
        if (AudioController.Instance.bgmPlaying.Equals(bgm))
        {
            StartLandingShuJu.GetInstance().PlayBgMusic();
        }
    }

    static string GetBgmByPanelId(UIPanleID panelId)
    {
        string result = string.Empty;
        switch (panelId)
        {
            case UIPanleID.UIMountAndPet:
                result = "mount";
                break;
            case UIPanleID.UIHeroList:
            case UIPanleID.EquipDevelop:
                result = "hero";
                break;
            case UIPanleID.UIActivityPanel:
                result = "shilian";
                break;
            case UIPanleID.UIShopPanel:
                result = "shop";
                break;
            case UIPanleID.UIPvP:
                result = "duizhan";
                break;
            case UIPanleID.UILevel:
                result = "copyUI";
                break;
            case UIPanleID.UILottery:
                result = "lucky";
                break;
            default:
                break;
        }
        return result;
    }
}
