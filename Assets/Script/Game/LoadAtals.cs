/*
文件名（File Name）:   LoadAtals.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class LoadAtals : MonoBehaviour
{
    public UIAtlas[] ats;
    void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < ats.Length; i++)
        {
            ResourceManager.Instance().AddAtlas(ats[i].name, ats[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
