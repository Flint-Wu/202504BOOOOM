using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadeAct : MonoBehaviour
{
    public CodeTrans TransTool;

    public ActRecord[] actRecords;
    public string[] LocStates;
    public string[] LocNums;
    public string[] PushLoc;

    private string[] sign = { "$", "%", "^", "&", "*" };

    void Start()
    {
        
    }

    public void GiveState()
    {
        LocStates = new string[actRecords.Length];
        LocNums = new string[actRecords.Length];
        PushLoc = new string[actRecords.Length];

        for (int i = 0; i < actRecords.Length; i++)
        {
            LocStates[i] = actRecords[i].LocState;
            LocNums[i] = actRecords[i].LocNum;
        }
    }

    public void TransLoc()
    {
        GiveState();


        for (int i = 0; i < actRecords.Length; i++)
        {
            int randomIndex = Random.Range(0, sign.Length);
            string randomElement = sign[randomIndex];

            PushLoc[i] = TransTool.StrTransCode(LocNums[i]) + randomElement + TransTool.StrTransCode(LocStates[i]);
        }
    }
}
