using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(BlockState))]
[RequireComponent(typeof(ActRecord))]
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
        ActRecord.LocState = tmp.ToString();//ʹactrecord�е�״̬��ֵ��ͬ����������������ֵ
        StateChange();
    }

    public void CountNum()//�����������г���״̬�ж��ٸ�1
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
            Debug.Log("���ؿ鳡��״̬�л�Ϊ" + tmp);
        }
    }

    

}
