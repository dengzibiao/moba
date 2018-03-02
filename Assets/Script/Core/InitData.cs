using UnityEngine;
using System.Collections;

/// <summary>
/// 初始化数据
/// </summary>

public class InitData : MonoBehaviour
{
    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);

        if (GameLibrary.isInitData)
        {
            VOManager.Instance().Init();
            VOManager.Instance().LoadTxt();
            GameLibrary.isInitData = false;
        }
        
    }
}