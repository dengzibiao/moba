using UnityEngine;
using System.Collections;

public class AngerPoint : MonoBehaviour
{
    public PointSelf[] points;
    private float interval = 3f;
    private float timer = 0f;
    private int[] pCount;

    private GameObject effect_NuQi;//怒气点特效
    public static AngerPoint _instance;
    private static GameObject go;
    private int indexPosition;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
        angerCount = 1;
        indexPosition = 1;
        if (_instance == null || (_instance != null && _instance.gameObject.activeSelf))
        {
            _instance = this;
        }
        gameObject.SetActive(false);
    }

    void Update()
    {   
        if (angerCount < 20)
        {
            timer += Time.deltaTime;    
            if (timer >= interval)
            {
                timer = 0f;
                ChangePoint();
            }
            
        }
        if(angerCount < 0)
        {
            angerCount = 0;
        }
    }

    private void GenerateNuqi()
    {
        effect_NuQi = Resources.Load(GameLibrary.Effect_Build + "nuqi") as GameObject;
        go = Instantiate(effect_NuQi, points[0].transform.position, Quaternion.identity) as GameObject;
        go.GetComponent<RenderQueueModifier>().m_target = points[0].GetComponent<UISprite>();
    }
 
    public void ChangePoint(int digit = 1)
    {
        if (go == null)
        {
            GenerateNuqi();
        }
        angerCount += digit;
        indexPosition += digit;
     
        foreach (PointSelf item in points)
        {
            item.ChangeColor(angerCount);
        
              if ((item.index == indexPosition%10||indexPosition==10||indexPosition==20) && indexPosition != 0)
               {
          
                go.transform.position = item.transform.position;
                if (GameObject.FindGameObjectWithTag("EffectNip") != null)
                {
                    go.transform.parent = GameObject.FindGameObjectWithTag("EffectNip").transform;
                }
                else
                {
                    Debug.Log(transform.name + "  EffectNip tag is not find");
                }
                go.transform.localScale = new Vector3(0.2f, 0.2f, 0);
                ParticleSystem[] p = go.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].startSize = 0.35f;
                    p[i].startRotation = 2f;
                    if (digit > 0)
                    {
                        
                        p[i].Play();
                    }
                }


            }
        }
       
    }

    public int angerCount;

    //private int angerCount = 1;
    //public int AngerCount
    //{
    //    get { return angerCount; }
    //}

}
