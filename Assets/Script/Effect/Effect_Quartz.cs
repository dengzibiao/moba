using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 施法动作
/// </summary>
public class Effect_Quartz : MonoBehaviour
{
    private GameObject sf;
    public Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();

    public bool Quartz(string url, string id)
    {
        try
        {
            if(!dic.ContainsKey(id))
            {
                GameObject go = Resources.Load(url) as GameObject;
                sf = Instantiate(go, transform.parent.position, Quaternion.identity) as GameObject;
                sf.transform.parent = this.transform.parent;
                sf.transform.localRotation =Quaternion.Euler(0, 0, 0);
                dic.Add(id, sf);
            }

            dic.TryGetValue(id, out sf);
            sf.SetActive(false);
            sf.SetActive(true);

            return true;
        }
        catch(System.Exception)
        {
            return false;
        }
    }
}
