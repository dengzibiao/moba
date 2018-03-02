using UnityEngine;

public class WdStarCondition : MonoBehaviour 
{
	public UILabel Desc;
    public UISprite star;
	
	void Awake()
	{
	
	}

	void Start () 
	{
	
	}

	void OnDestroy()
	{
	
	}

	void Update()
	{
	
	}

    public void RefreshUI(string des, bool isObtain)
    {
        Desc.text = des;
        star.spriteName = isObtain ? "xingxing" : "huisexingxing";
    }

}