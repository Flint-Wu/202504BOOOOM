using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCheatCode : MonoBehaviour
{
    // Start is called before the first frame update
    public void GenCheatCode()
    {
        GetComponent<MadeID>().TransID();
        GetComponent<MadeAct>().TransLoc();
        GetComponent<MadeConcateAll>().ConcatAll();
        Debug.Log("CheatCode: " + GetComponent<MadeConcateAll>().CheatCode);
    }
}
