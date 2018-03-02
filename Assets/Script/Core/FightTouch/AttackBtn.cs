using UnityEngine;

public class AttackBtn : MonoBehaviour 
{
    public GameObject Effect;
    public delegate void LongPressEvent(bool b);
    public LongPressEvent OnLongPress;
    public bool isPressed;

    void OnPress(bool b )
    {
        isPressed = b;
        if (!b)
        {
            if(OnLongPress != null)
                OnLongPress(false);
        }
    }

    void Update()
    {
        if (OnLongPress != null && isPressed)
        {
            OnLongPress(true);
        }
    }

    public void ClearBtn()
    {
        OnPress(false);
    }

}