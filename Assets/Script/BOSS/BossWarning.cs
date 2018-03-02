using UnityEngine;

public class BossWarning : MonoBehaviour
{
    public void ShowWarning(float time)
    {
        GetComponent<TweenAlpha>().PlayForward();
        Destroy(this.gameObject, time);
    }
}
