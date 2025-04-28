using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public ActRecord ActRecord;
    public string[] GivenValues;
    public string NowState;
    public int tmp;

    void Start()
    {
        ActRecord = GetComponent<ActRecord>();
    }

    // Update is called once per frame
    void Update()
    {
        GivenValues = ActRecord.givenValues;
        CountNum();
        ActRecord.LocState = tmp.ToString();//使actrecord中的状态数值等同于作弊码中最大的数值
        StateChange();
    }

    public void CountNum()//计算作弊码中场景状态有多少个1
    {
        tmp = 0;
        for (int i = 0; i < GivenValues.Length; i++)
        {
            GivenValues[i] = "1";
            tmp++;
        }
    }

    public void StateChange()
    {
        if (ActRecord.LocState == "5") 
        {
            Debug.Log("本地块场景状态切换为" + tmp);
        }
    }

    

}
