using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadeConcateAll : MonoBehaviour
{
    public MadeAct MadeAct;
    public MadeID MadeID;
    public string CheatCode;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ConcatAll()
    {
        CheatCode = MadeID.PushID + ":" + MadeAct.PushLoc;
    }
}
