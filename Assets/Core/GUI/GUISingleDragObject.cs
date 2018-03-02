using UnityEngine;
using System.Collections;
using System;

public class GUISingleDragObject: MonoBehaviour
{

    #region Init

    public delegate void OnDragObject(GameObject go);
    public OnDragObject onDrag;

    private UISprite _sprite;
    private UISprite _spriteClone;
    #endregion

    void Awake()
    {
        _sprite = this.GetComponent<UISprite>();
        isDrag = true;
        isReserve = false;
    }

    void OnDragStart()
    {
        _spriteClone.atlas = _sprite.atlas;
        _spriteClone.spriteName = _sprite.spriteName;
        _spriteClone.gameObject.SetActive(true);
        _spriteClone.depth = 100;
    }
    void OnDrag(Vector2 delta)
    {
        Vector3 dv = delta;
        _spriteClone.transform.localPosition += dv;
    }

    void OnDragEnd()
    {
        _spriteClone.gameObject.SetActive(false);
        _spriteClone.transform.localPosition = Vector2.zero;
    }
    void OnDrop(GameObject go)
    {
        GameObject target = UICamera.lastHit.collider.gameObject;
        target.GetComponent<UISprite>().atlas = go.GetComponent<UISprite>().atlas;
        target.GetComponent<UISprite>().spriteName = go.GetComponent<UISprite>().spriteName;
        if(!isReserve) go.GetComponent<UISprite>().atlas = null;
    }

    bool isColliderEnabled
    {
        get
        {
            Collider c = GetComponent<Collider>();
            if(c != null) return c.enabled;
            Collider2D b = GetComponent<Collider2D>();
            return (b != null && b.enabled);
        }
    }

    public bool isDrag
    {
        set
        {
            if(value)
            {
                _spriteClone = NGUITools.AddChild<UISprite>(this.gameObject);
                _spriteClone.name = "_Clone";
                _spriteClone.width = _sprite.width;
                _spriteClone.height = _sprite.height;
                _spriteClone.atlas = _sprite.atlas;
                _spriteClone.spriteName = _sprite.spriteName;
                _spriteClone.gameObject.SetActive(false);

                if(!isColliderEnabled)
                {
                    NGUITools.AddWidgetCollider(this.gameObject, true);
                }
            }
            else
            {
                if(this.GetComponent<BoxCollider>() != null) this.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    public bool isReserve { set; get; }

}