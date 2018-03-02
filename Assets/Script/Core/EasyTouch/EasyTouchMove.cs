using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tianyu;
using System;

public class EasyTouchMove : MonoBehaviour
{
    private float last_xoff = 0;
    private float last_zoff = 0;
    private float xoff = 0;
    private float zoff = 0;
    private Vector3 v = Vector3.zero;
    private EasyTouch up;
    System.Random rd = new System.Random();
    private TouchHandler mTouchHandler;
    public delegate void OnMoveEvent(float timeLast);
    public OnMoveEvent OnMove;
    MapInfoNode[] mCurMapInfo;

    void Awake()
    {
        up = transform.GetComponentInChildren<EasyTouch>();
        mTouchHandler = TouchHandler.GetInstance();
        Dictionary<long, MapInfoNode> tempMapInfo = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList;
        List<MapInfoNode> mTempMapInfo = new List<MapInfoNode>(tempMapInfo.Values);
        mCurMapInfo = new MapInfoNode[mTempMapInfo.Count];
        mTempMapInfo.CopyTo(mCurMapInfo);
    }

    float tempTime;

    void FixedUpdate()
    {
        if(CharacterManager.player == null)
            return;
#if UNITY_STANDALONE
        xoff = Input.GetAxis("Horizontal");
        zoff = Input.GetAxis("Vertical");
#elif UNITY_EDITOR
        xoff = Input.GetAxis( "Horizontal" ) != 0 ? Input.GetAxis("Horizontal") : up.upPosition.x;
        zoff = Input.GetAxis( "Vertical" ) != 0 ? Input.GetAxis("Vertical") : up.upPosition.y;
#elif UNITY_ANDROID
        xoff = up.upPosition.x;
        zoff = up.upPosition.y;
#elif UNITY_IPHONE
        xoff = up.upPosition.x;
        zoff = up.upPosition.y;
#endif

        CheckMapInfo();
        v = new Vector3(xoff, 0f, zoff);
        if(Camera.main != null)
        {
            Quaternion q = Camera.main.transform.rotation;
            v = Quaternion.Euler( 0f, q.eulerAngles.y, 0f)* v;
        }
        //if(GameLibrary.isMoba)
        //    v = Quaternion.Euler(0f, 45f, 0f) * v;

        if (mTouchHandler != null)
        {
            if (Mathf.Abs(xoff) > 0.1f || Mathf.Abs(zoff) > 0.1f)
            {
                mTouchHandler.Touch(TOUCH_KEY.Run);
            }
            else
            {
                mTouchHandler.Release(TOUCH_KEY.Run);
            }
            mTouchHandler.mOffset = v;
        }

        if (xoff == 0 && zoff == 0)
        {
            if(last_xoff!=0 || last_zoff!=0 )
            {
             //   ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos( CharacterManager.player.transform.position );
             //   ClientSendDataMgr.GetSingle().GetWalkSend().SendOrientation( CharacterManager.player.transform.rotation.eulerAngles );

                if ( FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey( playerData.GetInstance().selfData.mapID ) )
                {
                    MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList [ playerData.GetInstance().selfData.mapID ];
                    if ( tempMN != null )
                    {
                        if (StartLandingShuJu.GetInstance().currentScene == tempMN.MapName )
                        {
                            Vector3 pos = CharacterManager.player.transform.position;
                          //  WalkSendMgr.GetSingle().GetWalkSend().SendSelfPos( playerData.GetInstance().selfData.accountId , new Vector3( pos.x , pos.y , pos.z ) );
                        }
                    }
                }
            }

            if ( CharacterManager.player != null && !CharacterManager.instance.shouldMove && CheckLastMove())
            {
                CharacterManager.instance.PlayerStop();
            }
            tempTime = 0;
        }
        else if(Mathf.Abs(xoff) > 0.1f || Mathf.Abs(zoff) > 0.1f)
        {
            if (FightTouch._instance != null)
            {
                if(CharacterManager.playerCS.pm.isAutoMode)
                {
                    FightTouch._instance.OnAutoClick();
                    CharacterManager.instance.PlayerStop();
                }
                FightTouch._instance.CancelTp();
            }
            if(UIRole.instance != null)
                UIRole.instance.CancelRide();
            if (CharacterManager.instance.shouldMove)
            {
                if (!(mTouchHandler.IsTouched(TOUCH_KEY.Attack) || mTouchHandler.IsTouched(TOUCH_KEY.Skill1) || mTouchHandler.IsTouched(TOUCH_KEY.Skill2) ||
                 mTouchHandler.IsTouched(TOUCH_KEY.Skill3) || mTouchHandler.IsTouched(TOUCH_KEY.Skill4)))
                {
                    CharacterManager.instance.shouldMove = false;
                }
            }
            else
            {
                RotatePlayer();
                CharacterManager.instance.PlayerMove(v.normalized * Time.deltaTime);
            }
            //if ( Time.frameCount % 30 == 0 )//每隔30帧处理一次
            tempTime += Time.deltaTime;
            if(OnMove != null)
                OnMove(tempTime);
            if ( tempTime > 0.3f )
            {
                tempTime = 0f;
              
                if (FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList.ContainsKey(playerData.GetInstance().selfData.mapID))
                {
                    MapInfoNode tempMN = FSDataNodeTable<MapInfoNode>.GetSingleton().DataNodeList[playerData.GetInstance().selfData.mapID];
                    if (tempMN != null)
                    {
                        if (StartLandingShuJu.GetInstance().currentScene == tempMN.MapName)
                        {
                            Vector3 pos = CharacterManager.player.transform.position;
                            // WalkSendMgr.GetSingle().GetWalkSend().SendSelfPos(playerData.GetInstance().selfData.accountId, new Vector3(pos.x, pos.y, pos.z));
                        }
                    }
                }
                //ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos(CharacterManager.player.transform.localPosition);

            }
        }
        else
        {
            if (CharacterManager.player != null && !CharacterManager.instance.shouldMove && CheckLastMove())
            {
                //玩家停下来要同步给服务器位置
                //ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos(CharacterManager.player.transform.localPosition);
                CharacterManager.instance.PlayerStop();
            }
            tempTime = 0;
        }
        last_xoff = xoff; last_zoff = zoff;
    }

    private bool CheckLastMove()
    {
        return last_xoff != 0 || last_zoff != 0;
    }

    void CheckMapInfo ()
    {
        if ( playerData.GetInstance().selfData.oldMapID != playerData.GetInstance().selfData.mapID )
        {
            if (mCurMapInfo != null)
            {
                for (int i = 0; i < mCurMapInfo.Length; i++)
                {
                    MapInfoNode min = mCurMapInfo[i];
                    playerData.GetInstance().selfData.oldMapID =(int) min.key;
                  //  playerData.GetInstance().selfData.mapID = min.key;

                    if ( min.MapName == StartLandingShuJu.GetInstance().currentScene)
                    {
                        playerData.GetInstance().selfData.mapName = StartLandingShuJu.GetInstance().currentScene;
                        RoleInfo tempRI = new RoleInfo();
                        Vector3 pos = CharacterManager.player.transform.position;
                        tempRI.mapID = min.key;
                        tempRI.keyID = playerData.GetInstance().selfData.accountId;
                        tempRI.accID = playerData.GetInstance().selfData.accountId;
                        tempRI.playID = playerData.GetInstance().selfData.playerId;
                        tempRI.roleID = playerData.GetInstance().selfData.heroId;
                        tempRI.posX = pos.x;
                        tempRI.posY = pos.y;
                        tempRI.posZ = pos.z;
                        tempRI.name = playerData.GetInstance().selfData.playeName;
                        if (CharacterManager.player.transform.GetComponent<SetMainHeroName>() == null)
                            CharacterManager.player.transform.gameObject.AddComponent<SetMainHeroName>();
                        ClientSendDataMgr.GetSingle().GetWalkSend().SendInitializePosInfo(tempRI);
                        return;

                    }
                }
            }
        }
       
    }

    private void RotatePlayer()
    {
        if (CharacterManager.playerCS == null || CharacterManager.playerCS.isDie) return;
        PlayerMotion pm = CharacterManager.playerCS.pm;
        if(!pm.CanMoveState())
            return;

        //向量v围绕y轴旋转cameraAngle.y度
        Vector3 q = Quaternion.Euler(0, 0, 0) * v;
        Quaternion qq = Quaternion.LookRotation(q);
        Transform playerTrans = CharacterManager.player.transform;
        playerTrans.rotation = Quaternion.Lerp(playerTrans.rotation, qq, Time.deltaTime * 100);
        ClientSendDataMgr.GetSingle().GetWalkSend().SendSelfPos();
    }
}
