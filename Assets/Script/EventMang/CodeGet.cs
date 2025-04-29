using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CodeGet : MonoBehaviour
{
    public Text[] inputCode;
    public ReMadeID ReMadeID;
    public ReMadeLoc ReMadeLoc;
    public ReadCode ReadCode;

    public string[,] LocNums;
    public string[,] LocStates;
    public string[] names;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AllReTrans()
    {
        names = new string[inputCode.Length];
        LocNums = new string[inputCode.Length, inputCode[0].text.Count(c => c == ':')];//生成作弊码中行列的二维数组
        LocStates = new string[inputCode.Length, inputCode[0].text.Count(c => c == ':')];//生成作弊码中行列的二维数组


        for (int i = 0; i < inputCode.Length; i++)
        {
            ReadCode.CheatCode = inputCode[i].text;

            ReMadeID.ReTrans();
            names[i] = ReMadeID.HelperID.ToString();//获取所有作弊码中名字部分

            ReMadeLoc.ReTrans();
            for (int j = 0; j < ReMadeLoc.LocunNums.Length; j++)
            {
                LocNums[i,j] = ReMadeLoc.LocunNums[j];//获取所有作弊码的编码
                LocStates[i,j] = ReMadeLoc.LocunStates[j];//获取所有作弊码的场景状态
            }
        }


    }
}
