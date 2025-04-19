using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadCode : MonoBehaviour
{
    public InputField InputField;

    public string CheatCode;
    public string[] CheatCodeSplit;

    public string IDunRe;
    public string[] LocunRe;

    public void SplitCC()
    {
        CheatCode = InputField.text;

        CheatCodeSplit = CheatCode.Split(":");
        LocunRe = new string[CheatCodeSplit.Length -1];

        IDunRe = CheatCodeSplit[0];

        for (int i = 1; i < CheatCodeSplit.Length; i++)
        {
            LocunRe[i-1] = CheatCodeSplit[i];
        
        }
    }
}
