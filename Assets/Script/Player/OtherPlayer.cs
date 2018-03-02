using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;

public enum RoleType
{
    otherPlayer = 1,
    monster,
}

public class OtherPlayer : MonoBehaviour
{
    PlayerMotion playerMotion;
    public UnityEngine.AI.NavMeshAgent navMA;
    //Vector3 cameraAngle;
    Vector3 oldPos;
    public GameObject OtherName;
    public GameObject objTitleName;
    public GameObject objSocietyName;
    // List<Vector3> posL = new List<Vector3>();
    Vector3 sererpos = new Vector3();
    public Vector3 rot;
    public bool isLookAt;
    public RoleType rType;
    bool isRotate;
    string otherPlayerTitleName;
    string otherPlayerSocietyName;
    void Awake()
    {
        //Destroy( GetComponent<CharacterController>() );
        //Destroy( GetComponent<CharacterState>() );
        //if ( GetComponent<Monster_AI>() != null)
        //{
        //    GetComponent<Monster_AI>().enabled = false;
        //}

        //cameraAngle = Camera.main.transform.rotation.eulerAngles;
        playerMotion = GetComponent<PlayerMotion>();
        if (GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
        {
            gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
        }
        navMA = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMA.enabled = false;
        rType = RoleType.monster;
        PlayPrepare();
    }

    public void SetStartPosAndRot(Vector3 pos, Vector3 lookat, string name, int title, string societyName)
    {
        if (rType == RoleType.otherPlayer)
        {
            OtherName = GameObject.Instantiate(Resources.Load("Prefab/OtherName")) as GameObject;
            if (OtherName != null)
            {
                OtherName.GetComponent<UIWidget>().depth = -2;
                OtherName.GetComponent<UILabel>().text = name;
                OtherName.name = name;
                OtherName.transform.parent = CharacterManager.instance.UIControl;
                OtherName.transform.localScale = Vector3.one;
            }


            //CreateTilteName(title);
            CreateSocietyName(societyName);

        }
        navMA.enabled = false;
        transform.position = pos;
        transform.LookAt(lookat);
        navMA.enabled = true;
    }

    public void SetTargetPos(Vector3 pos)
    {
        // Debug.LogError("SetTargetPos " + pos);
        if (navMA != null)
        {
            float dis = Mathf.Abs(Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(transform.localPosition.x, transform.localPosition.z)));
            if (Mathf.Abs(Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(transform.localPosition.x, transform.localPosition.z))) <= 0.2f)
            {
                if (navMA.enabled)
                {
                    navMA.Stop();
                }
                PlayPrepare();
                // Debug.LogError("dis is " + dis);
                return;
            }
            //if(posL.Count != 0&&Mathf.Abs( Vector3.Distance(pos,posL[0]))<0.2f)
            //{
            //    navMA.Stop();
            //    return;
            //}
            //posL.Clear();
            //posL.Add( pos );

            //if (!navMA.gameObject.activeSelf)
            //{
            //    navMA.gameObject.SetActive(true);
            //}
            sererpos.x = pos.x; sererpos.y = pos.y; sererpos.z = pos.z;
            if (!navMA.enabled)
                navMA.enabled = true;
            //if (!navMA.gameObject.activeSelf)
            //{
            //    navMA.gameObject.SetActive(true);
            //}
            //navMA.enabled = true;

           

                if (navMA != null && navMA.enabled)
            {
                navMA.Stop();
                navMA.ResetPath();
                navMA.Resume();
                navMA.stoppingDistance = 0.1f;
                navMA.SetDestination(sererpos);
                // Debug.LogError("sererpos is " + sererpos);
            }

        }
    }

    public void SetRot(Vector3 targetQuaternion)
    {
        rot = targetQuaternion;
        isLookAt = true;
    }

    void LateUpdate()
    {
        SetNamePos();
        //SetTitleNamePos();
        SetSocietyNamePos();

        if (navMA != null)
        {
            if (navMA.enabled)
            {
                if (playerMotion != null && playerMotion.cs != null && !playerMotion.cs.isDie)
                {
                    if (!navMA.pathPending)
                    {
                        //增加已经寻路到目的地的判断，然后结束这一次寻路  
                        //if (!navMA.pathPending && navMA.remainingDistance != Mathf.Infinity && navMA.pathStatus == NavMeshPathStatus.PathComplete && navMA.remainingDistance <= navMA.stoppingDistance)
                        //{
                        //    navMA.ResetPath();
                        //}
                        if(Mathf.Abs(navMA.remainingDistance) < 0.2f)
                            // Debug.Log( transform.name);

                        //    float targetDis = Vector2.Distance(new Vector2(navMA.nextPosition.x, navMA.nextPosition.z), new Vector2(navMA.steeringTarget.x, navMA.steeringTarget.z));
                        //Debug.LogError("targetDis is " + targetDis);
                        //if (targetDis < 0.1f)
                        {

                            //if (playerMotion != null)
                            //    playerMotion.Stop();
                            PlayPrepare();
                            if (isLookAt)
                            {
                                isRotate = true;
                            }
                            navMA.enabled = false;
                            //Debug.LogError("navMA disabled");
                        }
                        else
                        {

                            isRotate = false;
                            
                            transform.LookAt(new Vector3(navMA.steeringTarget.x, transform.position.y, navMA.steeringTarget.z));
                            playerMotion.Run();
                        }
                    }
                }
                else
                {
                    //Debug.LogError("navMA disabled 2");
                    navMA.Stop();
                    navMA.enabled = false;
                }
            }

        }

        if (isRotate)
        {
            transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles,rot,Time.time*0.005f);//Quaternion.Slerp(transform.rotation, rot, Time.time * 0.005f);
            if (transform.localEulerAngles == rot)
            {
                isRotate = false;
                isLookAt = false;
            }
        }

        oldPos = transform.position;
    }

    public void PlayPrepare()
    {
        //if ( rType == RoleType.otherPlayer )
        //{
        //    playerMotion.Stop();
        //}
        //else
        //{

        //}
        playerMotion.Stop();
    }

    void SetNamePos()
    {
        //if ( UICamera.mainCamera != null && rType == RoleType.otherPlayer )
        //{
        //    Vector3 tempPos = Camera.main.WorldToScreenPoint( transform.Find( "Headbuff" ).transform.position );
        //    tempPos.z = 0;
        //    OtherName.transform.position = UICamera.mainCamera.ScreenToWorldPoint( tempPos ); 
        //}
        if (rType == RoleType.otherPlayer)
        {
            OtherName.transform.position = BattleUtil.WorldToScreenPoint(transform.Find("Headbuff").position);
        }
    }
    public void SetHeadBuffPos(bool isRide)
    {
        if (isRide)
        {
            transform.Find("Headbuff").localPosition = new Vector3(0f, 0.5f, 0f);
        }
        else
        {
            transform.Find("Headbuff").localPosition = new Vector3(0f, 0.8f, 0f);
        }
    }
    void CreateSocietyName(string societyName)
    {
        if (objSocietyName == null && societyName != "" && societyName != null)
        {
            objSocietyName = GameObject.Instantiate(Resources.Load("Prefab/PlayerSocietyName")) as GameObject;
            objSocietyName.GetComponent<UIWidget>().depth = -2;
            objSocietyName.transform.parent = CharacterManager.instance.UIControl;
            objSocietyName.transform.localScale = Vector3.one;
            if (societyName != "")
            {
                otherPlayerSocietyName = societyName;
                objSocietyName.name = otherPlayerSocietyName;
                objSocietyName.GetComponent<UILabel>().text = "[5eaeff]" + otherPlayerSocietyName + "[-]";
            }
        }
    }
    void SetSocietyNamePos()
    {
        if (UICamera.mainCamera != null && objSocietyName != null)
        {
            Vector3 tempPos = Camera.main.WorldToScreenPoint(transform.Find("Headbuff").transform.position);
            tempPos.z = 0;
            tempPos.y += 28;//设置在玩家名字的上方
            Vector3 screenPos = UICamera.mainCamera.ScreenToWorldPoint(tempPos);
            objSocietyName.transform.position = screenPos;
            objSocietyName.GetComponent<UILabel>().text = "[5eaeff]" + otherPlayerSocietyName + "[-]";
        }
    }
    void CreateTilteName(int title)
    {
        if (objTitleName == null)
        {
            objTitleName = GameObject.Instantiate(Resources.Load("Prefab/PlayerTitleName")) as GameObject;
            objTitleName.GetComponent<UIWidget>().depth = -2;
            objTitleName.transform.parent = CharacterManager.instance.UIControl;
            objTitleName.transform.localScale = Vector3.one;


            if (FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList.ContainsKey(title))
            {
                otherPlayerTitleName = FSDataNodeTable<TitleNode>.GetSingleton().DataNodeList[title].titlename;
                if (Singleton<SceneManage>.Instance.Current == EnumSceneID.UI_MajorCity01)
                {
                    objTitleName.name = otherPlayerTitleName;
                    objTitleName.GetComponent<UILabel>().text = "[2dd740]" + otherPlayerTitleName + "[-]";
                }
            }

        }
    }
    void SetTitleNamePos()
    {
        if (UICamera.mainCamera != null && rType == RoleType.otherPlayer)
        {
            Vector3 tempPos = Camera.main.WorldToScreenPoint(transform.Find("Headbuff").transform.position);
            tempPos.z = 0;
            tempPos.y += 35;//设置在公会名字的上方
            Vector3 screenPos = UICamera.mainCamera.ScreenToWorldPoint(tempPos);
            objTitleName.transform.position = screenPos;
            objTitleName.GetComponent<UILabel>().text = "[2dd740]" + otherPlayerTitleName + "[-]";
        }
    }
    public void RefreshTitleName(string titleName)
    {

        //如果没有携带称号 并且有称号对象 删除称号对象
        if (string.IsNullOrEmpty(titleName))
        {
            if (objTitleName != null)
            {
                Destroy(objTitleName);
            }
        }
        else
        {
            otherPlayerTitleName = titleName;
            //携带称号 但是没称号对象 需要创建一个
            if (objTitleName == null)
            {
                if (objTitleName == null)
                {
                    objTitleName = GameObject.Instantiate(Resources.Load("Prefab/PlayerTitleName")) as GameObject;
                    objTitleName.GetComponent<UIWidget>().depth = -2;
                    objTitleName.name = otherPlayerTitleName;
                    objTitleName.transform.parent = CharacterManager.instance.UIControl;
                    objTitleName.transform.localScale = Vector3.one;
                    objTitleName.GetComponent<UILabel>().text = "[2dd740]" + otherPlayerTitleName + "[-]";
                }
            }
            else
            {
                objTitleName.name = otherPlayerTitleName;
                objTitleName.GetComponent<UILabel>().text = "[2dd740]" + otherPlayerTitleName + "[-]";
            }

        }
    }
    public void RefreshSocietyName(string socityName)
    {
        //如果没有公会 并且有公会对象 删除公会对象
        if (string.IsNullOrEmpty(socityName))
        {
            if (objSocietyName != null)
            {
                Destroy(objSocietyName);
            }
        }
        else
        {
            otherPlayerSocietyName = socityName;
            //有公会 但是没个公会对象 需要创建一个
            if (objSocietyName == null)
            {
                objSocietyName = GameObject.Instantiate(Resources.Load("Prefab/PlayerSocietyName")) as GameObject;
                objSocietyName.GetComponent<UIWidget>().depth = -2;
                objSocietyName.transform.parent = CharacterManager.instance.UIControl;
                objSocietyName.transform.localScale = Vector3.one;
                objSocietyName.name = otherPlayerSocietyName;
                objSocietyName.GetComponent<UILabel>().text = "[5eaeff]" + otherPlayerSocietyName + "[-]";
            }
            else
            {
                objSocietyName.name = otherPlayerSocietyName;
                objSocietyName.GetComponent<UILabel>().text = "[5eaeff]" + otherPlayerSocietyName + "[-]";
            }

        }
    }

    //private void RotatePlayer ()
    //{
    //    Vector3 q = Quaternion.Euler(0, cameraAngle.y, 0) * new Vector3( targetPosition.x,0, targetPosition.y );
    //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(q), Time.deltaTime * 10);
    //}

    void OnDestroy()
    {
        if (OtherName != null)
        {
            Destroy(OtherName);
        }
        if (objTitleName != null)
        {
            Destroy(objTitleName);
        }
        if (objSocietyName != null)
        {
            Destroy(objSocietyName);
        }
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

}
