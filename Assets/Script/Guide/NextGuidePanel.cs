using UnityEngine;
using System.Collections;
using Tianyu;

public class NextGuidePanel : MonoBehaviour {

    public GameObject GuideModel;

    public GameObject GuideDialogWin;

    public UILabel GuideDialogWinLabel;
    public GameObject content;
    public bool isInit = false;
    private static NextGuidePanel single;

    public static NextGuidePanel Single()
    {
       
        return single;
    }

    public NextGuidePanel()
    {
        single = this;
    }
    void Start () {
        //StartCoroutine(setCameraGuideLayer());
        if (!isInit && !Globe.isFightGuide)
        {
            Init();
            isInit = true;
        }
    }
	public void Close()
    {
        if(content.activeSelf)
        {
            content.SetActive(false);
        }
    }
	
	public void Init () {

        //Debug.Log("Nexguid  init()");
        int guideId = 0;
        if (playerData.GetInstance().guideData.uId != 0 && GameLibrary.UI_Major == Application.loadedLevelName)
        {
            switch (playerData.GetInstance().guideData.uId)
            {
                case 919:
                    guideId = 9;
                    break;

                case 906:
                    guideId = 8;
                    break;

                case 1092:
                    guideId = 10;
                    break;

                case 1204:
                    guideId = 11;
                    break;

                case 1219:
                    guideId = 12;
                    break;

                //case 2125:
                //    guideId = 21;
                //    break;

                case 2319:
                    guideId = 23;
                    break;

                case 2719:
                    guideId = 27;
                    break;

                case 2919:
                    guideId = 29;
                    break;

                case 3019:
                    guideId = 30;
                    break;

                case 2269:
                    guideId = 22;
                    break;

                case 2471:
                    guideId = 24;
                    break;

                case 2871:
                    guideId = 28;
                    break;

                case 25110:
                    guideId = 25;
                    break;

                case 2697:
                    guideId = 26;
                    break;

                //case 1419:
                //    guideId = 14;
                //    break;

                case 1331:
                    guideId = 13;
                    break;
                    
                case 3219:
                    guideId = 32;
                    break;

                case 33120:
                    guideId = 33;
                    break;

                case 34120:
                    guideId = 34;
                    break;

                case 35120:
                    guideId = 35;
                    break;

                case 36120:
                    guideId = 36;
                    break;

                case 37120:
                    guideId = 37;
                    break;

                case 38120:
                    guideId = 38;
                    break;

                case 3938:
                    guideId = 39;
                    break;

                case 4031:
                    guideId = 40;
                    break;

                case 4119:
                    guideId = 41;
                    break;

                case 4297:
                    guideId = 42;
                    break;

                case 4397:
                    guideId = 43;
                    break;

                case 4419:
                    guideId = 44;
                    break;

                case 4531:
                    guideId = 45;
                    break;

                case 4638:
                    guideId = 46;
                    break;

                case 4731:
                    guideId = 47;
                    break;

                case 4897:
                    guideId = 48;
                    break;

                case 4997:
                    guideId = 49;
                    break;
                default:
                    break;
            }

            Init(guideId);

           //引导指引NPC模型处理
           //Vector2 vec = FSDataNodeTable<GuideNode>.GetSingleton().DataNodeList[playerData.GetInstance().guideData.stepId].modelPos;
           //GuideModel.transform.localPosition = new Vector3(vec.x, vec.y, 0);
           //switch (FSDataNodeTable<GuideNode>.GetSingleton().DataNodeList[playerData.GetInstance().guideData.stepId].orientations)
           //{
           //    case 2:
           //        NextGuidePanel.Single().GuideModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
           //        break;
           //    case 1:
           //        NextGuidePanel.Single().GuideModel.transform.localRotation = Quaternion.Euler(0, 180, 0);
           //        break;
           //    default:
           //        break;
           //}


            if (UIGuidePanel.Single() != null)
                UIGuidePanel.Single().InitGuide();
        }
        else
        {
            content.SetActive(false);
        }

        //NextGuidePanel.Single().GuideModel.transform.localPosition = new Vector3(-422, -332, 0);
        //    NextGuidePanel.Single().GuideModel.transform.localRotation = Quaternion.Euler(0, 180, 0);
        //    NextGuidePanel.Single().GuideDialogWin.transform.localPosition = new Vector3(-62, 128, 0);
        //UIGuidePanel.Single().InitGuide((int)UIPanleID.UILevel);
    }

    public void Init(int guideId)
    {
        if (content != null && !content.activeSelf)
            content.SetActive(true);

        if (Globe.isFightGuide)
            content.SetActive(true);

        if (FSDataNodeTable<GuideNode>.GetSingleton().DataNodeList.ContainsKey(guideId))
        {
            GuideNode guideNode = FSDataNodeTable<GuideNode>.GetSingleton().DataNodeList[guideId];

            if (guideNode.guide_content == "0")
            {
                GuideDialogWinLabel.gameObject.SetActive(false);
                GuideDialogWin.gameObject.SetActive(false);
            }
            else
            {
                if (!GuideDialogWinLabel.gameObject.activeSelf)
                    GuideDialogWinLabel.gameObject.SetActive(true);
                if (!GuideDialogWin.gameObject.activeSelf)
                    GuideDialogWin.gameObject.SetActive(true);

                UISprite sprite = GuideDialogWin.GetComponent<UISprite>();
                UILabel label = GuideDialogWin.transform.Find("Label").GetComponent<UILabel>();
                switch (guideNode.orientations)
                {
                    case 1:
                        sprite.spriteName = "zhiyingkuang";
                        sprite.flip = UIBasicSprite.Flip.Horizontally;
                        label.transform.localPosition = new Vector3(10, 0, 0);
                        break;
                    case 2:
                        sprite.spriteName = "zhiyingkuang";
                        sprite.flip = UIBasicSprite.Flip.Nothing;
                        label.transform.localPosition = new Vector3(-10, 0, 0);
                        break;
                    case 3:
                        sprite.spriteName = "zhiyingkuang_down";
                        sprite.flip = UIBasicSprite.Flip.Horizontally;
                        label.transform.localPosition = new Vector3(0, 8, 0);
                        break;
                    case 4:
                        sprite.spriteName = "zhiyingkuang_up";
                        sprite.flip = UIBasicSprite.Flip.Horizontally;
                        label.transform.localPosition = new Vector3(0, -8, 0);
                        break;
                }

                GuideDialogWinLabel.text = guideNode.guide_content.Replace("|", "");
                Vector2 vec1 = guideNode.dialogPos;
                GuideDialogWin.transform.localPosition = new Vector3(vec1.x, vec1.y, 0);
                if (playerData.GetInstance().guideData.uId == 906 || playerData.GetInstance().guideData.uId == 919 || playerData.GetInstance().guideData.uId == 1219 || playerData.GetInstance().guideData.uId == 4419
                 || playerData.GetInstance().guideData.uId == 2319 || playerData.GetInstance().guideData.uId == 2719 || playerData.GetInstance().guideData.uId == 2919 || playerData.GetInstance().guideData.uId == 3019
                  || playerData.GetInstance().guideData.uId == 3219)
                {
                    NextGuidePanel.Single().content.GetComponent<UIPanel>().depth = 0;
                }
            }
        }
    }

    //生成引导GuideModel
    IEnumerator setCameraGuideLayer()
    {
        yield return null;
        HeroPosEmbattle.instance.DetailPos.gameObject.SetActive(true);
        GameObject GuideModel = Instantiate(Resources.Load("Prefab/Character/NPC/akasha_show")) as GameObject;
        GuideModel.transform.parent = HeroPosEmbattle.instance.DetailPos;
        GuideModel.transform.localPosition = Vector3.zero;
    }
}
