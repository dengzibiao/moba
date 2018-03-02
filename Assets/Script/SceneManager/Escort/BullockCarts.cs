using UnityEngine;
using System.Collections.Generic;

public class BullockCarts : MonoBehaviour
{

    public delegate void OnArrivals();
    public OnArrivals OnArrDes;

    public GameObject autoPoint = null;
    CharacterState cs;
    PlayerMotion pm;

    int index = 0;
    int indexPoint = 0;

    float distance = 0;
    float proportion = 0;

    //SceneBaseManager sceneBase;

    public bool isMove { get; set; }

    public List<GameObject> purse = new List<GameObject>();
    public List<GameObject> autoPoints = new List<GameObject>();

    void Awake()
    {
        cs = GetComponent<CharacterState>();
        pm = GetComponent<PlayerMotion>();

        foreach (Transform item in transform)
        {
            if (item.name == "Purse")
                purse.Add(item.gameObject);
        }

        index = purse.Count;

        foreach (Transform item in transform.parent)
        {
            if (item.gameObject.tag == Tag.auto)
                autoPoints.Add(item.gameObject);
        }
    }

    void Start()
    {
        //sceneBase = GetComponentInParent<SceneBaseManager>();
        isMove = true;
        proportion = cs.maxHp * 0.12f;
        autoPoint = NextPoint();
        pm.nav.enabled = true;
        pm.isAutoMode = true;
    }

    void FixedUpdate()
    {
        if (!isMove)
        {
            pm.Stop();
            return;
        }
        distance = Vector3.Distance(transform.position, autoPoint.transform.position);
        if (distance > 0.5f)
        {
            pm.transform.LookAt(new Vector3(autoPoint.transform.position.x, transform.position.y, autoPoint.transform.position.z));
            pm.Move(autoPoint.transform.position);
        }
        else
        {
            autoPoint = NextPoint();
        }

        if (Vector3.Distance(transform.position, autoPoints[autoPoints.Count - 1].transform.position) <= 0.5f)
        {
            if (null != OnArrDes) OnArrDes();
            pm.Stop();
            enabled = false;
        }
        
    }

    public void BeAttacked(CharacterState cs)
    {
        SceneUIManager.instance.gamePrompt.SwitchBloodScreen(true, false, SceneBaseManager.instance.sceneType);
        //SceneUIManager.instance.SwitchBloodScreen(true, false, cs.CharData.attrNode.types);
        if (purse.Count <= 0) return;
        float count = (cs.currentHp - proportion * 4) / proportion;
        if (Mathf.Abs(index - count) >= 1)
        {
            index--;
            purse[index].SetActive(false);
            //Destroy(purse[index].gameObject);
            purse.RemoveAt(index);
        }
    }

    GameObject NextPoint()
    {
        GameObject nextPoint = null;
        if (null != autoPoints && autoPoints.Count <= 0)
        {
            pm.Stop();
            return null;
        }
        if (indexPoint > autoPoints.Count - 1)
        {
            pm.Stop();
            return null;
        }
        nextPoint = autoPoints[indexPoint];
        indexPoint++;
        return nextPoint;
    }

}