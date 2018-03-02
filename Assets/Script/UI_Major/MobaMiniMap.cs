using UnityEngine;
using System.Collections.Generic;


public class MobaMiniMap : MonoBehaviour
{
    public static List<CharacterState> mapElements = new List<CharacterState>();
    public static MobaMiniMap instance;

    public UISprite heroHead;
    public UISprite map1v1;
    public UISprite map3v3;
    public UISprite mapDragBox;

    Transform mapTrans;
    float mapSize;
    float mapScale = 380;
    float mapRotZ = 5;
    float mapPosOffsetZ = 1.2f;
    Vector2 mapDragConstrain;
    Vector2 mapDragCenter;
    int baseDepth;

    void Awake()
    {
        instance = this;
        mapElements.Clear();

        GameObject mapGo = GameObject.Find("Map");
        if(mapGo != null)
        {
            mapTrans = mapGo.transform;
            SphereCollider mapCol = mapGo.GetComponent<SphereCollider>();
            if(mapCol != null)
                mapSize = mapCol.radius;
        }

        mapDragBox.gameObject.SetActive(false);
        baseDepth = map1v1.depth + 1;
    }

    void RefreshIcon(CharacterState element)
    {
        if (element != null && element.MapIcon != null)
        {
            element.MapIcon.transform.localPosition = GetMapPos(element.transform.position);
        }
    }

    Vector3 GetMapPos (Vector3 worldPos)
    {
        Vector3 ret = Vector3.zero;
        float xOff = worldPos.x - mapTrans.position.x;
        float zOff = worldPos.z - mapTrans.position.z;
        float xRatio = xOff / mapSize;
        float zRatio = zOff / mapSize;
        ret.x = 0.5f * xRatio * mapScale;
        ret.y = 0.5f * zRatio * mapScale;
        ret = Quaternion.Euler(0f, 0f, mapRotZ) * ret;
        return ret;
    }

    Vector3 GetWorldPos ( Vector2 mapPos )
    {
        Vector3 ret = Vector3.zero;
        mapPos = Quaternion.Euler(0f, 0f, -mapRotZ) * mapPos;
        float xRatio = mapPos.x / (0.5f * mapScale );
        float yRatio = mapPos.y / (0.5f * mapScale );
        float xPos = xRatio * mapSize + mapTrans.position.x;
        float zPos = yRatio * mapSize + mapTrans.position.z;
        ret = new Vector3(xPos, 0f, zPos);
        return ret;
    }

    void OnPress ( bool b )
    {
        mapDragBox.gameObject.SetActive(b);
        if(b)
        {
            ThirdCamera.instance._MainPlayer = null;
            Vector2 touchPos = NGUIMath.ScreenToPixels(UICamera.currentTouch.pos, transform);
            Vector3 camTrans = GetWorldPos(touchPos);
            mapDragBox.transform.localPosition = touchPos;
            Camera.main.transform.position = new Vector3(camTrans.x, Camera.main.transform.position.y, camTrans.z - mapPosOffsetZ);
        }
        else
        {
            ThirdCamera.instance._MainPlayer = CharacterManager.player.transform;
        }
    }

    void OnDrag (Vector2 delta)
    {
        Vector2 touchPos = NGUIMath.ScreenToPixels(UICamera.currentTouch.pos, transform);
        touchPos.x = Mathf.Clamp(touchPos.x, -0.5f * ( mapDragConstrain.x - mapDragBox.width) + mapDragCenter.x, 0.5f * ( mapDragConstrain.x -mapDragBox.width ) + mapDragCenter.x);
        touchPos.y = Mathf.Clamp(touchPos.y, -0.5f * ( mapDragConstrain.y - mapDragBox.height ) + mapDragCenter.y, 0.5f * ( mapDragConstrain.y - mapDragBox.height ) + mapDragCenter.y);
        Vector3 camTrans = GetWorldPos(touchPos);
        mapDragBox.transform.localPosition = touchPos;
        Camera.main.transform.position = new Vector3(camTrans.x, Camera.main.transform.position.y, camTrans.z - mapPosOffsetZ);
    }

    void Update()
    {
        if(Time.frameCount % GameLibrary.mTowerDelay != 0) return;
        for (int i = 0; i < mapElements.Count; i++)
        {
            RefreshIcon(mapElements[i]);
        }
    }

    string GetMapIconNameByState(CharacterState cs)
    {
        switch (cs.state)
        {
            default:
            case Modestatus.Player:
            case Modestatus.NpcPlayer:
                return cs.CharData.attrNode.icon_name + "_head";
            case Modestatus.Monster:
                return cs.groupIndex == 1 ? "posMonsterBlue" : "posMonsterRed";
            case Modestatus.Tower:
                return cs.groupIndex == 1 ? "posTowerBlue" : "posTowerRed";
        }
    }

    public UISprite AddMapIconByType(CharacterState cs)
    {
        if (GameLibrary.playerstate == 1)
            return null;
        if(SceneBaseManager.instance != null)
        {
            switch(SceneBaseManager.instance.sceneType)
            {
                case SceneType.Dungeons_MB1:
                case SceneType.MB1:
                case SceneType.TP:
                    map1v1.gameObject.SetActive(true);
                    map3v3.gameObject.SetActive(false);
                    mapScale = 300;
                    mapRotZ = 0;
                    mapDragConstrain = new Vector2(360, 80);
                    mapDragCenter = new Vector2(0, 0);
                    break;
                case SceneType.MB3:
                    map1v1.gameObject.SetActive(false);
                    map3v3.gameObject.SetActive(true);
                    mapScale = 430;
                    mapRotZ = 5;
                    mapDragConstrain = new Vector2(400, 140);
                    mapDragCenter = new Vector2(0, -10);
                    break;
                default:
                    break;
            }
        }

        UISprite icon = NGUITools.AddSprite(gameObject, BattleUtil.IsHeroTarget(cs) ? heroHead.atlas : map1v1.atlas, GetMapIconNameByState(cs));
        icon.depth = map1v1.depth + 1;
        if(BattleUtil.IsHeroTarget(cs))
        {
            icon.depth = baseDepth + 1;
            string borderName = "";
            if(cs == CharacterManager.playerCS)
                borderName = "lvdiankuang";
            else
                borderName = cs.groupIndex == 0 ? "hongdiankuang" : "landiankuang";
            UISprite iconGroupBorder = NGUITools.AddSprite(icon.gameObject, heroHead.atlas, borderName);
            iconGroupBorder.width = iconGroupBorder.height = 32;
            iconGroupBorder.depth = icon.depth + 1;
            baseDepth += 2;
            if(cs == CharacterManager.playerCS)
            {
                icon.depth = 199;
                iconGroupBorder.depth = icon.depth + 1;
            }
        }

        if(cs.state == Modestatus.Monster)
            icon.width = icon.height = 10;
        if(cs.state == Modestatus.Tower)
        {
            icon.width = 10;
            icon.height = 16;
        }
        if(BattleUtil.IsHeroTarget(cs))
            icon.width = icon.height = 28;
        if(cs.groupIndex > 1)
            ChangeColorGray.Instance.ChangeSpriteColor(icon, false);
        cs.MapIcon = icon;
        if (!mapElements.Contains(cs)) mapElements.Add(cs);
        cs.OnDead += ( CharacterState mCs ) => RemoveMapIcon(mCs);
        return icon;
    }

    void RemoveMapIcon(CharacterState cs)
    {
        if (mapElements.Contains(cs)) mapElements.Remove(cs);
        if (cs.MapIcon != null)  NGUITools.Destroy(cs.MapIcon.gameObject);
    }
}