using UnityEngine;
using System.Collections;

public class BossChuChangField : MonoBehaviour {

    int count = 3;

    //public GameObject KGF;

    float timeClose = 9.5f;

    public void ChangeCount()
    {
        if (count == 0) return;

        count -= 1;
        if (count == 0)
        {
           
            GameLibrary.isBossChuChang = true;
            CharacterManager.instance.PlayerStop();
           // KGF.SetActive(false);
            RenderSettings.fog = false;
            Instantiate(Resources.Load(GameLibrary.Effect_Boss + queue));
           // Invoke("KGFMapClose", timeClose);
            
            //this.gameObject.SetActive(false);
            //bossobj.SetActive(true);
        }
    }

    //void KGFMapClose()
    //{
    //    KGF.SetActive(true);
    //}

    public string queue;
    //public GameObject bossobj;
   
}
