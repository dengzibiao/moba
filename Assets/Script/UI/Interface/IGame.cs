/*
文件名（File Name）:   IGame.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2017-1-3 15:47:25
*/
using UnityEngine;
using System.Collections;

public interface IGame  {

    //游戏当前状态
    EnumObjectState State { get; }

    //开始游戏
    void Run();
    //退出游戏
    void Exit();
}
