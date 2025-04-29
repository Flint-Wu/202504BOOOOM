using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationCheatCode : MonoBehaviour
{
    public MadeConcateAll madeConcateAll;
    public void TranslateCheatCode()
    {
        // GetComponent<ReadCode>().SetCheatCode();
        GetComponent<ReadCode>().SplitCC();
        GetComponent<ReMadeID>().ReTrans();
        GetComponent<ReMadeLoc>().ReTrans();
    }
}
