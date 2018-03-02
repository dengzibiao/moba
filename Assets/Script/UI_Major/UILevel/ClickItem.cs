using System.Collections.Generic;
using UnityEngine;

public class ClickItem : MonoBehaviour 
{
    public delegate void VoidDelegate ( GameObject go );
    public VoidDelegate ClickGo;

    public int Id { get; set; }
    public int MapId { get; set; }
    public delegate void IndexDelegate(int id, int MapID = 0);
    public IndexDelegate ClickWithId;

    [HideInInspector]
    public List<EventDelegate> ClickDele = new List<EventDelegate>();


    void OnClick ()
    {
        if(ClickGo != null)
            ClickGo(gameObject);
        if (ClickWithId != null)
            ClickWithId(Id, MapId);
        EventDelegate.Execute(ClickDele);
        GuideManager.Single().SetObject(this.gameObject);
    }
}