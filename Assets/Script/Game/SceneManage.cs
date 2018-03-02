/*
文件名（File Name）:   SceneManager.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2016-10-26 19:16:49
*/
using UnityEngine;
using System.Collections;

public class SceneManage {
    private EnumSceneID _current = EnumSceneID.Null;
	public  byte mobaltype = 1;
    public EnumSceneID Current
    {
        get { return _current; }
        set {if(_current!=value)Regist(value); }
    }

    public void Regist(EnumSceneID current)
    {
        this._current = current;
        Control.ClearGUI();
        if(this._current!= EnumSceneID.UI_MajorCity01)
        {
            Singleton<GUIManager>.Instance.dic.Clear();

        }
        else
        {
            //if (Control.GetFullScreenUIList().Count > 0)
            //{
            //    Control.ShowGUI(Control.GetFullScreenUIList()[Control.GetFullScreenUIList().Count - 1],
            //        EnumOpenUIType.OpenNewCloseOld);
            //}
        }
    }
}
