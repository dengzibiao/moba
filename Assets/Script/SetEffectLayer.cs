using UnityEngine;
using System.Collections;

public class SetEffectLayer : MonoBehaviour {

    public int layer = 3050;
    int OldLayer;
    // Use this for initialization
    void Awake () {
        OldLayer = layer;
        SetLayer(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if ( OldLayer != layer )
        {
            OldLayer = layer;
            SetLayer( gameObject );
        }
	}

    void SetLayer ( GameObject obj )
    {
        if (obj.GetComponent<Renderer>()!=null)
        {
            Material[] tempM = obj.GetComponent<Renderer>().materials;
            for (int i = 0; i < tempM.Length; i++)
            {
                tempM[i].renderQueue = layer;
            }

        }

        for ( int i = 0 ; i < obj.transform.childCount ; i++ )
        {
            SetLayer( obj.transform.GetChild( i ).gameObject );
        }
    }
}
