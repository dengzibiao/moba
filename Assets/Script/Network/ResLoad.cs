using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System;

public class ResLoad : MonoBehaviour
{
    public static string LoadJsonRes1(string name)
    {
        TextAsset ob = (TextAsset)Resources.Load("jsondata/"+name);    
        return ob.ToString();       
    }
}
