using UnityEngine;
using System.Collections;

public class PointSelf : MonoBehaviour
{
    public int index;
    private UISprite sp;

    void Awake()
    {
        sp = this.GetComponent<UISprite>();
        index = int.Parse(transform.name.Substring(5));
    }

    public void ChangeColor(int count)
    {
        //Debug.Log(count.ToString() + "xxxx");
        if(count >10 && (count-10)>=index)
        {
            sp.spriteName = "nuqidian-huang";
        }
        else if (count >= index)

       // else if (count <= 10 && count >= index)
        {
            sp.spriteName = "nuqidian-lan";
        }
        else
        {
            sp.spriteName = "nuqidian-hui";
        }

    }
}
