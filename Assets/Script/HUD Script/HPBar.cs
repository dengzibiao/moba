using UnityEngine;
using System.Collections;

/// <summary>
/// 血条控制
/// </summary>

public class HPBar : MonoBehaviour
{
    UIProgressBar hp;            //血条显示
    UIProgressBar mask;          //缓动显示
    UIProgressBar shield;
    public UISprite SpHp;
    public UISprite SpMask;
    public UISprite SpSheild;
    public UISprite SpSheildFull;
    public UIWidget Ruler;
    public UILabel LaName;
    private UISprite mImmuneGo, mInvincibleGo;
    //private Vector3 mNameOriginPos = new Vector3(0, 16, 0), mNameChangePos = new Vector3(0, 40, 0);
    private string hpName;
    private string mInvincibleHpName = "wudigxuetiao";

    UISprite[] rulerLines;
    float currentValue = 1; //当前值

    bool isPlay = false;

    void Awake()
    {
        hp = SpHp.GetComponent<UIProgressBar>();
        mask = SpMask.GetComponent<UIProgressBar>();
        shield = SpSheild.GetComponent<UIProgressBar>();
        rulerLines = Ruler.GetComponentsInChildren<UISprite>();
        Ruler.gameObject.SetActive(false);
        SpSheild.alpha = SpSheildFull.alpha = 0f;
        mImmuneGo = transform.Find("ImmuneBg").GetComponent<UISprite>();
        mInvincibleGo = transform.Find("InvincibleBg").GetComponent<UISprite>();
    }

    public void ShowAndHideImmune(bool b)
    {
        if (b)
        {
            if (mInvincibleGo.alpha == 0  && mImmuneGo.alpha == 0)
            {
                mImmuneGo.alpha = 1;
                SetNamePos(b);
            }
        }
        else
        {
            if (mImmuneGo.alpha == 1)
            {
                mImmuneGo.alpha = 0;
                SetNamePos(b);
            }
        }
    }

    public void ShowAndHideInvincible(bool b)
    {
        if (b)
        {
            if (mInvincibleGo.alpha == 0)
            {
                mInvincibleGo.alpha = 1;
                hpName = SpHp.spriteName;
                SpHp.spriteName = mInvincibleHpName;
                SpHp.depth = SpMask.depth + 3;
                SetNamePos(b);
            }
        }
        else
        {
            if (mInvincibleGo.alpha == 1)
            {
                mInvincibleGo.alpha = 0;
                SpHp.spriteName = hpName;
                SpHp.depth = SpMask.depth + 1;
                SetNamePos(b);
            }
        }
    }

    private void SetNamePos(bool b)
    {
        // LaName.transform.localPosition = b ? mNameChangePos : mNameOriginPos;
        //mImmuneGo.transform.localPosition = mNameChangePos;
        //mInvincibleGo.transform.localPosition = mNameChangePos;
    }

    void Update()
    {
        if (isPlay)
        {
            if (mask.value > currentValue)
            {
                mask.value = Mathf.Lerp(mask.value, currentValue, 2f * Time.deltaTime);
            }
        }
    }

    public void UpdateHPBar(float newValue, float shieldValue)
    {
        hp.value = currentValue = newValue;
        isPlay = true;
        if(shieldValue > 0)
        {
            SpSheild.alpha = 1f;
            if(shieldValue + newValue > 1f)
            {
                shield.value = shieldValue;
                SpSheild.depth = SpMask.depth + 2;
                shield.fillDirection = UIProgressBar.FillDirection.LeftToRight;
                SpSheildFull.alpha = 1f;
            }
            else
            {
                shield.value = shieldValue + newValue;
                SpSheild.depth = SpMask.depth;
                shield.fillDirection = UIProgressBar.FillDirection.RightToLeft;
                SpSheildFull.alpha = 0f;
            }
        }
        else
        {
            SpSheild.alpha = SpSheildFull.alpha = 0f;
        }
    }

    //public void ShowPlayerCenterHpBar ( bool show , CharacterState cs)
    //{
    //    // AngerPoint._instance.gameObject.SetActive(show);
    //    gameObject.SetActive(show);
    //    if(show)
    //    {
    //        hp.value = mask.value = currentValue = 1f;
    //        SetRuler(cs.maxHp);
    //        LaName.text = cs.CharData.attrNode.name;
    //    }
    //}

    public void SetRuler ( float maxHp )
    {
        Ruler.gameObject.SetActive(true);
        int perLine = 100;
        int lineCount = maxHp < 2000 ? Mathf.FloorToInt(maxHp / perLine) : 20;
        float perWidth = maxHp < 2000 ? SpHp.width * perLine / maxHp : SpHp.width / 20;

        for(int i = 0; i < rulerLines.Length; i++)
        {
            if(i >= lineCount)
            {
                rulerLines[i].alpha = 0f;
            }
            else
            {
                float xPos = perWidth * ( i + 1 ) - 0.5f * SpHp.width;
                Vector3 pos = rulerLines[i].transform.localPosition;
                rulerLines[i].transform.localPosition = new Vector3(xPos, pos.y, pos.z);
                rulerLines[i].alpha = 0.6f;
                if(( i + 1 ) % 5 == 0)
                {
                    rulerLines[i].height = 12;
                    rulerLines[i].alpha = 0.4f;
                }
                if(( i + 1 ) % 10 == 0)
                {
                    rulerLines[i].height = 12;
                    rulerLines[i].alpha = 0.8f;
                }
            }
        }

    }

}
