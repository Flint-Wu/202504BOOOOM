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
        LocNums = new string[inputCode.Length, inputCode[0].text.Count(c => c == ':')];//���������������еĶ�ά����
        LocStates = new string[inputCode.Length, inputCode[0].text.Count(c => c == ':')];//���������������еĶ�ά����


        for (int i = 0; i < inputCode.Length; i++)
        {
            ReadCode.CheatCode = inputCode[i].text;

            ReMadeID.ReTrans();
            names[i] = ReMadeID.HelperID.ToString();//��ȡ���������������ֲ���

            ReMadeLoc.ReTrans();
            for (int j = 0; j < ReMadeLoc.LocunNums.Length; j++)
            {
                LocNums[i,j] = ReMadeLoc.LocunNums[j];//��ȡ����������ı���
                LocStates[i,j] = ReMadeLoc.LocunStates[j];//��ȡ����������ĳ���״̬
            }
        }
    }
}
