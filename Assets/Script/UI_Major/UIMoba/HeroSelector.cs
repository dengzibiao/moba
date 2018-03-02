using UnityEngine;

public class HeroSelector : MonoBehaviour 
{
    public int heroId;
    public bool isEnemy;
    public static HeroSelector SelfSelect;
    public static HeroSelector EnemySelect;
    static UISprite SpSelfFight;
    static UISprite SpEnemyFight;

    void Awake ()
    {
        if(name == "SpriteBingnv")
            SpSelfFight = transform.FindChild("Sprite").GetComponent<UISprite>();
        if(name == "SpriteBingnv_e")
            SpEnemyFight = transform.FindChild("Sprite").GetComponent<UISprite>();
    }

    void Start () 
	{
        ClickItem ci = GetComponent<ClickItem>();
        ci.Id = heroId;
        ci.ClickWithId += ChangeSelect;
        OnEnable();
    }

    void OnEnable ()
    {
        if(!isEnemy && SpSelfFight != null && heroId == ToInt(GameLibrary.player))
            ChangeSelect(ToInt(GameLibrary.player), 0);
    }

    public static int ToInt (long id)
    {
        return Mathf.FloorToInt(100 + ( id - 201000000 ) / 100);
    }

    public static int ToLong ( int id )
    {
        return 201000000 + 100 * ( id - 100 );
    }

    void ChangeSelect ( int id, int mapId)
    {
        if(isEnemy)
        {
            EnemySelect = this;
            if(EnemySelect != null)
            {
                SpEnemyFight.transform.parent = EnemySelect.transform;
                SpEnemyFight.transform.localPosition = Vector3.zero;
            }
            GameLibrary.emeny = ToLong(id);
        }
        else
        {
            SelfSelect = this;
            if(SelfSelect != null)
            {
                SpSelfFight.transform.parent = SelfSelect.transform;
                SpSelfFight.transform.localPosition = Vector3.zero;
            }
            GameLibrary.player = ToLong(id);
        }
    }
}