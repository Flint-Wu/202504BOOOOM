using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReMadeID : MonoBehaviour
{
    public ReadCode readCode;
    public CodeTrans TransTool;

    public string IDunRe;
    public string[] IDData;

    public string HelperID;

    public void ReTrans()
    {
        IDData = new string[2];
        IDunRe = readCode.IDunRe;

        if (IDunRe.Contains("`"))
            IDData = IDunRe.Split("`");
        else if (IDunRe.Contains("~"))
            IDData = IDunRe.Split("~");
        else if (IDunRe.Contains("!"))
            IDData = IDunRe.Split("!");
        else if (IDunRe.Contains("@"))
            IDData = IDunRe.Split("@");
        else if (IDunRe.Contains("#"))
            IDData = IDunRe.Split("#");

        HelperID = TransTool.BakTransCode(IDData[1]);
    }
}
