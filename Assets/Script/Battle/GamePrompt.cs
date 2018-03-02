using UnityEngine;
using System.Collections;

public class GamePrompt : MonoBehaviour
{
    public GameObject bloodScreen;
    public UISprite sPrompt;
    public Transform tTween;
    public TweenAlpha mask;
    public UILabel delay;
    public TweenAlpha tweenAlpha;
    public GameObject esPrompt;

    public float promptTimer = 3f;

    float gameDuration = 0;
    float esTimer = 0;

    int index = 0;
    int delayIndex = 3;

    bool isDelay = false;
    bool isES = false;

    void Update()
    {
        if (isES && esTimer > 0)
        {
            if (Time.realtimeSinceStartup > esTimer + 1)
                esPrompt.GetComponent<UIPanel>().alpha -= 0.02f;
            if (Time.realtimeSinceStartup > esTimer + 2)
            {
                isES = false;
                StartPrompt();
            }
        }
        if (gameDuration > 0)
        {
            if (Time.realtimeSinceStartup > gameDuration + promptTimer)
            {
                PlayPrompt();
                Time.timeScale = 1;
                gameDuration = 0;
            }
            if (isDelay && Time.realtimeSinceStartup > gameDuration + index)
            {
                index++;
                delay.text = delayIndex + "";
                delayIndex--;
            }
        }
    }

    void PlayPrompt()
    {
        delay.enabled = false;
        SetUITween(true);
        mask.enabled = true;
        Invoke("ClosePrompt", 0f);
    }

    void ClosePrompt()
    {
        tTween.gameObject.SetActive(false);
        mask.gameObject.SetActive(false);
        tTween.transform.localPosition = new Vector3(0, 170, 0);
        SetUITween(false);
        tweenAlpha.from = 0.9f;
        tweenAlpha.to = 0.4f;
        tweenAlpha.style = UITweener.Style.PingPong;
        tweenAlpha.enabled = true;
        if (null != SceneUIManager.instance.bubble)
            SceneUIManager.instance.bubble.transform.parent.gameObject.SetActive(true);
    }

    void SetUITween(bool isOpen)
    {
        UITweener[] mTweens = tTween.GetComponents<UITweener>();
        for (int i = 0; i < mTweens.Length; i++)
        {
            mTweens[i].enabled = isOpen;
        }
    }

    public void StartGamePrompt(SceneType type)
    {
        isDelay = false;
        switch (type)
        {
            case SceneType.TP:
                sPrompt.spriteName = "zhanling";
                break;
            case SceneType.KV:
                sPrompt.spriteName = "huoban";//gongji
                break;
            case SceneType.ES:
                esPrompt.SetActive(true);
                isES = true;
                isDelay = true;
                sPrompt.spriteName = "biezhua";
                break;
            case SceneType.TD:
                isDelay = true;
                sPrompt.spriteName = "baohu";
                break;
            default:
                return;
        }
        if (null != SceneUIManager.instance.bubble)
            SceneUIManager.instance.bubble.transform.parent.gameObject.SetActive(false);
        if (type != SceneType.ES)
        {
            StartPrompt();
        }
        else
        {
            esTimer = Time.realtimeSinceStartup;
            Time.timeScale = 0;
        }
    }

    void StartPrompt()
    {
        esPrompt.SetActive(false);
        sPrompt.keepAspectRatio = UIWidget.AspectRatioSource.Free;
        sPrompt.MakePixelPerfect();
        sPrompt.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
        sPrompt.height = 70;
        delay.enabled = isDelay;
        gameDuration = Time.realtimeSinceStartup;
        tTween.gameObject.SetActive(true);
        mask.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void SwitchBloodScreen(bool b, bool isPlayer = true, SceneType sceneType = SceneType.NONE)
    {
        if (bloodScreen.activeSelf != b)
            bloodScreen.SetActive(b);
        bloodScreen.GetComponent<TweenAlpha>().ResetToBeginning();
        foreach (Transform item in bloodScreen.transform)
            item.gameObject.SetActive(false);

        if (!isPlayer)
        {
            if (tTween.gameObject.activeSelf != b)
            {
                switch (sceneType)
                {
                    case SceneType.KV:
                    case SceneType.ACT_EXP:
                    case SceneType.ACT_GOLD:
                        sPrompt.spriteName = "gongji";
                        break;
                    case SceneType.TD:
                        sPrompt.spriteName = "jidi";
                        break;
                }
                tTween.gameObject.SetActive(true);
            }
            CancelInvoke("ClosePromptTween");
            Invoke("ClosePromptTween", 2f);
        }
        CancelInvoke("CloseBloodScreen");
        Invoke("CloseBloodScreen", 2f);
    }

    void CloseBloodScreen()
    {
        bloodScreen.SetActive(false);
    }

    void ClosePromptTween()
    {
        tTween.gameObject.SetActive(false);
    }

}
