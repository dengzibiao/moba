using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class HandlerManager : Singleton<HandlerManager>
{
    private Dictionary<uint, CL_Base> handler = new Dictionary<uint, CL_Base>();

    public void SocketMessage()
    {
        //SocketPackage.Instance
    }

    public void ReceiveSocket(uint id, byte[] bytes)
    {
        CL_Base cl;
        bool isHas = handler.TryGetValue(id, out cl);
        if(!isHas)
        {
          //  Debug.Log("This message id is not exist");
        }
        else
        {
            ParseBytes(id, cl, bytes);
        }

    }

    private void ParseBytes(uint id, CL_Base cl, byte[] bytes)
    {
        switch(id)
        {
            case 1:
                (cl as CL_LoginAccount).rsp.unserial(new BinaryReader(new MemoryStream(bytes)));
                //if((cl as CL_LoginAccount).rsp.rc == 0) Control.GetGUI(GUISign.UINEW).SocketData(Singleton<CL_LoginAccount>.Instance.rsp);
                //else Debug.LogError((cl as CL_LoginAccount).rsp.rc);
                break;
        }
    }

    public void Dispense()
    {


    }

    public void InQueue(uint id, CL_Base cl)
    {
        handler.Add(id, cl);
    }
}
