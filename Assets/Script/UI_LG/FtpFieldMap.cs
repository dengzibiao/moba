using UnityEngine;
using System.Collections;

public class FtpFieldMap : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag =="Player")
        {
            //UIFieldMap_Enter.instance.OpenWin();
            collider.gameObject.SetActive(false);
            if (playerData.GetInstance().selfData.mapID == 20000)
            {
                ClientSendDataMgr.GetSingle().GetBattleSend().SendGetHerosBattleAttr(Globe.fightHero);
                GameLibrary.isSkipingScene = true;
                ClientSendDataMgr.GetSingle().GetLoginSend().SendChengeScene(20000, 20100);
       
            }
            else
            {
                GameLibrary.isSkipingScene = true;
                ClientSendDataMgr.GetSingle().GetLoginSend().SendChengeScene(20100, 20000);
            }
           // UI_Loading.LoadScene("LGhuangyuan", 2);
        }
    }
    //void OnCollisionExit(Collider collider)
    //{
    //    if (collider.tag == "Player")
    //    {
    //        UIFieldMap_Enter.instance.OnCancelClick();
    //    }
    //}
}
