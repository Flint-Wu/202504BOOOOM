using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReMadeLoc : MonoBehaviour
{
    public ReadCode readCode;
    public CodeTrans TransTool;

    public string[] LocunRe;
    public string[] LocunData;

    public string[] LocunStates;
    public string[] LocunNums;

    public void ReTrans()
    {
        LocunRe = readCode.LocunRe;
        LocunData = new string[2];
        LocunStates = new string[LocunRe.Length];
        LocunNums = new string[LocunRe.Length];

        for (int i = 0; i < LocunRe.Length; i++)
        {
            if (LocunRe[i].Contains("$"))
                LocunData = LocunRe[i].Split("$");
            else if (LocunRe[i].Contains("%"))
                LocunData = LocunRe[i].Split("%");
            else if (LocunRe[i].Contains("^"))
                LocunData = LocunRe[i].Split("^");
            else if (LocunRe[i].Contains("&"))
                LocunData = LocunRe[i].Split("&");
            else if (LocunRe[i].Contains("*"))
                LocunData = LocunRe[i].Split("*");

            LocunStates[i] = TransTool.BakTransCode(LocunData[1]);
            LocunNums[i] = TransTool.BakTransCode(LocunData[0]);
        }
    }
}
