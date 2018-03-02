using UnityEngine;

public class SceneButton : MonoBehaviour
{
    public UISprite BgIcon;
    public UISprite Icon;
    public UILabel LaSceneName;
    public UISprite[] Stars;
    public SceneNode SceneData;

    public void RefreshUI(SceneNode scene, int star = -99)
    {
        if (null != scene)
        {
            SceneData = scene;
            if (null != scene.icon_name && scene.icon_name != "0" && null != Icon)
            {
                BgIcon.spriteName = "BOSS";
                Icon.enabled = true;
                if (scene.icon_name.Contains("yx_"))
                    Icon.atlas = ResourceManager.Instance().GetUIAtlas("UIHeroHead");
                else
                    Icon.atlas = ResourceManager.Instance().GetUIAtlas("BossHead");
                Icon.spriteName = scene.icon_name;
                LaSceneName.transform.parent.localPosition = new Vector3(0, 85, 0);
                Stars[0].transform.parent.localPosition = new Vector3(0, -22, 0);
            }
            else
            {
                LaSceneName.transform.parent.localPosition = new Vector3(0, 69, 0);
                Stars[0].transform.parent.localPosition = Vector3.zero;
                BgIcon.spriteName = UIMap.SceneIcon(scene.game_type);
            }
            BgIcon.MakePixelPerfect();
            LaSceneName.text = scene.SceneName;
        }

        if (star >= 0)
        {
            BgIcon.color = new Color(1, 1, 1);
            GetComponent<BoxCollider>().enabled = true;
            for (int i = 0; i < 3; i++)
            {
                Stars[i].enabled = true;
                Stars[i].spriteName = i < star ? "Star1" : "Star2";
            }
        }
        else
        {
            BgIcon.color = new Color(0, 0, 0);
            GetComponent<BoxCollider>().enabled = false;
            for (int i = 0; i < 3; i++)
            {
                Stars[i].enabled = false;
            }
        }

        if (Icon.enabled)
            Icon.color = Stars[0].enabled ? new Color(1, 1, 1) : new Color(0, 0, 0);
    }


}