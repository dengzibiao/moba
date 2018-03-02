using UnityEngine;

/// <summary>
/// 提示面板
/// </summary>
public class PromptPanel : MonoBehaviour
{

    #region 字段

    public static PromptPanel instance;

    UILabel prompt;         //提示
    UISprite bgBtn;         //遮罩按钮

    float timer = 0;        //计时器
    bool isHide = false;    //隐藏开关

    #endregion
    
    #region 功能函数

    /// <summary>
    /// 显示提示面板
    /// </summary>
    public void ShowPrompt(string prompt)
    {
        bgBtn.alpha = 1;
        this.prompt.text = prompt;
        isHide = true;
    }

    #endregion

    #region 基础方法

    void Awake()
    {
        instance = this;
        bgBtn = transform.Find("BG").GetComponent<UISprite>();
        prompt = bgBtn.transform.Find("Prompt").GetComponent<UILabel>();
    }

    void Start()
    {
        UIEventListener.Get(bgBtn.gameObject).onClick = OnBGBtnClick;
    }

    void Update()
    {
        if (isHide)
        {

            timer += Time.deltaTime;

            if (timer >= 0.5f)
            {
                //GetComponent<UIPanel>().alpha -= Time.deltaTime;
                bgBtn.alpha -= Time.deltaTime;
                if (bgBtn.alpha <= 0)
                {
                    isHide = false;
                    Hide();
                    timer = 0;
                }
            }
        }
    }

    /// <summary>
    /// 遮罩显示
    /// </summary>
    /// <param name="go"></param>
    private void OnBGBtnClick(GameObject go)
    {
        Hide();
    }

    /// <summary>
    /// 隐藏方法
    /// </summary>
    void Hide()
    {
        bgBtn.alpha = 0;
    }

    #endregion
    
}