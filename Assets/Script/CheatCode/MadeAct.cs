using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadeAct : MonoBehaviour
{
    public CodeTrans TransTool;

    public ActRecord[] actRecords;
    public string[] LocStates;
    public string[] LocNums;
    public string[] TranedAll;
    public string PushLoc;

    private string[] sign = { "$", "%", "^", "&", "*" };

    void Start()
    {
        
    }
    public void GetActRecordNotNull()
    {
        actRecords = FindObjectsOfType<ActRecord>();
        //找出里面所有GiveValues不为空的物体
        List<ActRecord> actRecordList = new List<ActRecord>();
        foreach (ActRecord actRecord in actRecords)
        {
            if (actRecord.LocState == "1")
            {
                actRecordList.Add(actRecord);
            }
        }
        actRecords = actRecordList.ToArray();

    }

    public void GiveState()
    {
        LocStates = new string[actRecords.Length];
        LocNums = new string[actRecords.Length];
        TranedAll = new string[actRecords.Length];

        for (int i = 0; i < actRecords.Length; i++)
        {
            LocStates[i] = actRecords[i].LocState;
            LocNums[i] = actRecords[i].LocNum;
        }
    }

    public void TransLoc()
    {
        GetActRecordNotNull();
        GiveState();


        for (int i = 0; i < actRecords.Length; i++)
        {
            int randomIndex = Random.Range(0, sign.Length);
            string randomElement = sign[randomIndex];

            TranedAll[i] = TransTool.StrTransCode(LocNums[i]) + randomElement + TransTool.StrTransCode(LocStates[i]);

            PushLoc = string.Join(":",TranedAll);
        }

    }
}
