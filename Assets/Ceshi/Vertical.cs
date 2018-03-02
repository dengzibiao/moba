/*
文件名（File Name）:   Vertical1.cs

作者（Author）:    #高#

创建时间（CreateTime）:  #CreateTime#
*/
using UnityEngine;
using System.Collections;

public class Vertical : MonoBehaviour
{
    object []obj=new object[15] { 1, 2, 3, 4, 5, 6, 7, 8,9,10,11,12,13,14,15};
    public GUIDynamicMultList uIWrapContent;

    void Start()
    {
        uIWrapContent = transform.Find("Scroll View/UIWrap Content").GetComponent<GUIDynamicMultList>();
        uIWrapContent.InSize(10,1);
        uIWrapContent.Info(obj);
    }
}
