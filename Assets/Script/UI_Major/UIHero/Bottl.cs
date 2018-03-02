using UnityEngine;
using System.Collections;

public class Bottl : MonoBehaviour
{

    int countBottl = 100;
    bool isPress = false;
    float timer = 0;

    void Start()
    {

    }


    void OnPress(bool isPress)
    {
        this.isPress = isPress;
    }

    //int num = 1;

    float timeClode = 0;

    int a = 0;

    void FixedUpdate()
    {

        if (isPress)
        {

            timer += Time.deltaTime;
            //StartCoroutine(ConsumeBottl(0));

            //print((int)timer);
            

            //Debug.Log(timer);

            if (timer >= 0.9f && timer <= 3)
            {
                timeClode += Time.deltaTime;


                //print(timeClode);

                if (timeClode >= 1)
                {
                    a++;
                    print(a);
                    
                    //StartCoroutine(ConsumeBottl(0));
                    timeClode = 0;
                }
                //1s消耗一个
                
            }
            //else if (timer >= 4 && timer <= 6)
            //{
            //    timeClode += Time.deltaTime;

            //    if (timeClode >= 0.5f)
            //    {
            //        //StartCoroutine(ConsumeBottl(0));
            //        timeClode = 0;
            //    }
            //    //0.5s消耗一个
            //    //StartCoroutine(ConsumeBottl(0.5f));
            //}
            //else if (timer >= 7)
            //{
            //    //0.2s消耗一个
            //    //StartCoroutine(ConsumeBottl(0.2f));
            //}

            //Debug.Log(countBottl);

        }
        else
        {
            timer = 0;
        }
    }


    IEnumerator ConsumeBottl(float time)
    {
        countBottl -= 1;
        print(countBottl);
        yield return new WaitForSeconds(time);
    }

}
