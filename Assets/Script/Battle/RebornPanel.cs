using UnityEngine;

public class RebornPanel : MonoBehaviour
{
    public UILabel laRebornCD;
    public UISprite spRebornCD;

    CDTimer.CD cd;

    public void ShowRebornCD(int rebornCD)
    {
        gameObject.SetActive(true);
        UITweener[] tweeners = spRebornCD.GetComponents<UITweener>();
        for (int i = 0; i < tweeners.Length; i++)
        {
            tweeners[i].ResetToBeginning();
            tweeners[i].enabled = true;
        }
        if (Camera.main.GetComponent<UIDraggableCamera>() != null)
        {
            UIDraggableCamera dc = GetComponentInChildren<UIDragCamera>().draggableCamera = Camera.main.GetComponent<UIDraggableCamera>();
            dc.scale = 0.01f * Vector2.one;
            if (SceneBaseManager.instance.sceneType == SceneType.MB1 || SceneBaseManager.instance.sceneType == SceneType.Dungeons_MB1)
            {
                dc.SetDragBounds(20, -20, -6, -10, 0);
            }
            else if (SceneBaseManager.instance.sceneType == SceneType.MB3)
            {
                dc.SetDragBounds(-22, -58, -2, -15, 5);
            }
        }
        cd = CDTimer.GetInstance().AddCD(1, (int count, long id) =>
        {
            //laRebornCD.text = string.Format(Localization.Get("RebornCD"), count);
            laRebornCD.text = count.ToString();
        }, rebornCD + 1, true);
        cd.OnRemove += (int count, long id) =>
        {
            spRebornCD.alpha = 0f;
            spRebornCD.transform.localScale = new Vector3(0.1f, 1f, 1f);
            spRebornCD.transform.localPosition = new Vector3(0f, -350f, 0f);
            gameObject.SetActive(false);
        };
    }

    public void HideRevornCD()
    {
        if (null != cd)
            CDTimer.GetInstance().RemoveCD(cd);
    }

}