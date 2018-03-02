using UnityEngine;
using System.Collections;

public class TaskBubbleItem : MonoBehaviour
{

    public UILabel label;

    GameObject target = null;
    Camera cam = null;

    void FixedUpdate()
    {
        if (null != target)
        {
            transform.position = GetBubblePos(target);
        }
    }

    public void DialogBubble(GameObject target, string text, float time)
    {
        if (null == target) return;
        gameObject.SetActive(true);
        this.target = target;
        label.text = text;
        Invoke("CloseBubble", time);
    }

    void CloseBubble()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }

    Vector2 GetBubblePos(GameObject target)
    {
        Vector2 pos;

        if (null == cam)
            cam = Camera.main;

        pos = cam.WorldToScreenPoint(new Vector3(target.transform.position.x, target.transform.position.y + 1.2f, target.transform.position.z));

        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);

        return pos;
    }

}
