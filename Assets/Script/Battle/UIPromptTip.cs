using UnityEngine;
using System.Collections;

public class UIPromptTip : MonoBehaviour
{

    public UILabel[] labels;

    public void RefreshDesUI(SceneNode sceneNode)
    {
        if (null == sceneNode) return;

        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].text = sceneNode.star_describe[i];
        }
    }

}
