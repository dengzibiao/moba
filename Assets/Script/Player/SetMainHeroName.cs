using UnityEngine;
using System.Collections;

public class SetMainHeroName : MonoBehaviour {

    GameObject objName;
    GameObject objTitleName;
    GameObject objSocietyName;
    Transform mainTran;

    private static SetMainHeroName instance;
    public static SetMainHeroName Instance { get { return instance; } set { instance = value; } }

    void Awake ()
    {
        instance = this;
        mainTran = CharacterManager.player.transform.Find( "Headbuff" ).transform;
        CreateName();
        if (!string.IsNullOrEmpty(playerData.GetInstance().selfData.playerTitleName))
        {
            CreateTilteName();

        }
        if (!string.IsNullOrEmpty(SocietyManager.Single().societyName))
        {
            CreateSocietyName();
        }
        if (CharacterManager.playerCS.pm.isRiding)
        {
            SetHeadBuffPos(true);
        }
        else
        {
            SetHeadBuffPos(false);
        }
    }
    
    void LateUpdate () {
        SetNamePos();
        SetTitleNamePos();
        SetSocietyNamePos();
    }

    void CreateName ()
    {
        if ( objName == null )
        {
            objName = GameObject.Instantiate( Resources.Load( "Prefab/OtherName" ) ) as GameObject;
            objName.GetComponent<UIWidget>().depth = -2;
            objName.name = playerData.GetInstance().selfData.playeName;
            objName.transform.parent = CharacterManager.instance.UIControl;
            objName.transform.localScale = Vector3.one;
        }
    }
    public void SetHeadBuffPos(bool isRide)
    {
        if (isRide)
        {
            mainTran.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        }
        else
        {
            mainTran.transform.localPosition = new Vector3(0f,0.8f,0f);
        }
    }
    void CreateTilteName()
    {
        if (objTitleName == null)
        {
            objTitleName = GameObject.Instantiate(Resources.Load("Prefab/PlayerTitleName")) as GameObject;
            objTitleName.GetComponent<UIWidget>().depth = -2;
            objTitleName.name = playerData.GetInstance().selfData.playerTitleName;
            objTitleName.transform.parent = CharacterManager.instance.UIControl;
            objTitleName.transform.localScale = Vector3.one;
        }
    }
    void CreateSocietyName()
    {
        if (objSocietyName == null)
        {
            objSocietyName = GameObject.Instantiate(Resources.Load("Prefab/PlayerSocietyName")) as GameObject;
            objSocietyName.GetComponent<UIWidget>().depth = -2;
            objSocietyName.name = SocietyManager.Single().societyName;
            objSocietyName.transform.parent = CharacterManager.instance.UIControl;
            objSocietyName.transform.localScale = Vector3.one;
        }
    }
    void SetNamePos ()
    {
        if ( UICamera.mainCamera != null )
        {
            Vector3 tempPos = Camera.main.WorldToScreenPoint( mainTran.position );
            tempPos.z = 0;
            Vector3 screenPos = UICamera.mainCamera.ScreenToWorldPoint( tempPos );
            objName.transform.position = screenPos;
            objName.GetComponent<UILabel>().text = playerData.GetInstance().selfData.playeName;
        }
    }

    void SetTitleNamePos()
    {
        if (UICamera.mainCamera != null&& objTitleName !=null)
        {
            Vector3 tempPos = Camera.main.WorldToScreenPoint(mainTran.position);
            tempPos.z = 0;
            tempPos.y += 35;//设置在公会名字的上方
            Vector3 screenPos = UICamera.mainCamera.ScreenToWorldPoint(tempPos);
            objTitleName.transform.position = screenPos;
            objTitleName.GetComponent<UILabel>().text = "[2dd740]" +playerData.GetInstance().selfData.playerTitleName+"[-]";
        }
    }
    void SetSocietyNamePos()
    {
        if (UICamera.mainCamera != null && objSocietyName != null)
        {
            Vector3 tempPos = Camera.main.WorldToScreenPoint(mainTran.position);
            tempPos.z = 0;
            tempPos.y += 28;//设置在玩家名字的上方
            Vector3 screenPos = UICamera.mainCamera.ScreenToWorldPoint(tempPos);
            objSocietyName.transform.position = screenPos;
            objSocietyName.GetComponent<UILabel>().text = "[5eaeff]" + SocietyManager.Single().societyName + "[-]";
        }
    }
    public void RefreshSocietyName()
    {
        //如果没有公会 并且有公会对象 删除公会对象
        if (string.IsNullOrEmpty(SocietyManager.Single().societyName))
        {
            if (objSocietyName != null)
            {
                Destroy(objSocietyName);
            }
        }
        else
        {
            //有公会 但是没个公会对象 需要创建一个
            if (objSocietyName == null)
            {
                CreateSocietyName();
            }
            else
            {
                objSocietyName.name = SocietyManager.Single().societyName;
                objSocietyName.GetComponent<UILabel>().text = "[5eaeff]" + SocietyManager.Single().societyName + "[-]";
            }

        }
    }
    public void RefreshTitleName()
    {
        //如果没有携带称号 并且有称号对象 删除称号对象
        if (string.IsNullOrEmpty(playerData.GetInstance().selfData.playerTitleName))
        {
            if(objTitleName != null)
            {
                Destroy(objTitleName);
            }
        }
        else
        {
            //携带称号 但是没称号对象 需要创建一个
            if (objTitleName == null)
            {
                CreateTilteName();
            }
            else
            {
                objTitleName.name = playerData.GetInstance().selfData.playerTitleName;
                objTitleName.GetComponent<UILabel>().text = "[2dd740]" + playerData.GetInstance().selfData.playerTitleName + "[-]";
            }

        }
    }
    void OnDestroy ()
    {
        if ( objName != null )
        {
            Destroy( objName );
        }
        if (objTitleName !=null)
        {
            Destroy(objTitleName);
        }
        if (objSocietyName != null)
        {
            Destroy(objSocietyName);
        }
    }
}
