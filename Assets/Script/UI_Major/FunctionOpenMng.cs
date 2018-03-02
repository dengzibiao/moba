using UnityEngine;
using System.Collections;
using Tianyu;
//系统开放 1024存储位是41，42
public class FunctionOpenMng{

    static FunctionOpenMng mSingleton;
    public static FunctionOpenMng  GetInstance()
    {
        if (mSingleton == null)
            mSingleton =  new FunctionOpenMng();
        return mSingleton;
    }
    public bool GetValu(int id)
    {
        int index = 841;
        if (id > 32)
        {
            index = 842;
            id -= 32;
        }
        return (((uint)(1 << (id - 1)) & playerData.GetInstance().selfData.infodata[index]) > 0);
        
    }
    public void SetValue(int id)
    {
        int index = 841;
        if (id > 32)
        {
            index = 842;
            id -= 32;
        }
        uint valu = ((uint)(1 << (id - 1)) | playerData.GetInstance().selfData.infodata[index]);
        playerData.GetInstance().selfData.infodata[index] = valu;
    }
    public bool GetIndexbypos(int index,int pos)
    {
       
        return (((uint)(1 << (pos)) & playerData.GetInstance().selfData.infodata[index]) > 0);

    }
  public bool GetFunctionOpen(int functionid)
    {
        return DataDefine.isSkipFunction || FSDataNodeTable<UnLockFunctionNode>.GetSingleton().DataNodeList[functionid].unlock_system_type == 0 || GetValu(functionid);
    }
}
