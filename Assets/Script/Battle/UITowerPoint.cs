using UnityEngine;
using System.Collections;

public class UITowerPoint : MonoBehaviour
{

    public UISlider BlueSlider;
    public UISlider RedSlider;
    public UILabel BlueLabel;
    public UILabel RedLabel;

    //int currentValueB = 0;
    //int additionB = 0;
    //int currentValueR = 0;
    //int additionR = 0;

    void Start()
    {
        BlueSlider.value = 0;
        RedSlider.value = 0;
    }

    public void RefreshUI(float blueRate, float redRate)
    {
        blueRate *= 2;
        redRate *= 2;
        if (blueRate>=100) blueRate = 100;
        if (redRate >= 100) redRate = 100;
        BlueLabel.text = (int)blueRate + "%";
        RedLabel.text = (int)redRate + "%";
        BlueSlider.value = blueRate * 0.01f;
        //additionB = (int)blueRate - currentValueB;
        //currentValueB = (int)blueRate;
        //BlueSlider.transform.Find("WhiteBars").localPosition -= new Vector3(additionB * 1.2f, 0, 0);
        RedSlider.value = redRate * 0.01f;
        //additionR = (int)redRate - currentValueR;
        //currentValueR = (int)redRate;
        //RedSlider.transform.Find("WhiteBars").localPosition += new Vector3(additionR * 1.2f, 0, 0);
    }

}