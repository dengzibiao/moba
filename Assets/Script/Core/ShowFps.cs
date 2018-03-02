using UnityEngine;
using System.Collections;

/// <summary>
/// 显示FPS
/// </summary>

public class ShowFps : MonoBehaviour
{

    public float f_UpdateInterval = 0.5F;

    private float f_LastInterval;

    private int i_Frames = 0;

    private float f_Fps;
    public static string str = "";
	static ShowFps m_singleton;
	public static ShowFps GetSingolton()
	{
		return m_singleton;
	}

    void Awake()
    {
        //过场景不销毁，显示FPS
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
		m_singleton = this;
        //Application.targetFrameRate = 60;
        f_LastInterval = Time.realtimeSinceStartup;

        i_Frames = 0;
    }

    void OnGUI()
    {
        if (true)
        {
            GUI.color = Color.red;
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.red;
            if (str != "")
            {
                GUI.Label(new Rect(10, 10, 400, 400), "FPS:" + str, style);
            }
            else
            {
                GUI.Label(new Rect(10, 10, 400, 400), "FPS:" + f_Fps.ToString("f2"), style);
            }

            if (CharacterManager.player)
            {
                GUI.Label(new Rect(150, 10, 400, 400), "Version: "+DataDefine.version , style);
                //GUI.Label(new Rect(150, 10, 400, 400), "POS:" + CharacterManager.player.transform.localPosition, style);
                //GUI.Label(new Rect(350, 10, 400, 400), "ROA:" + CharacterManager.player.transform.localRotation.eulerAngles, style);
            }
        }

    }

    void Update()
    {
        if (true)
        {
            ++i_Frames;
            if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
            {
                f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);

                i_Frames = 0;

                f_LastInterval = Time.realtimeSinceStartup;
            }
        }
    }
}
