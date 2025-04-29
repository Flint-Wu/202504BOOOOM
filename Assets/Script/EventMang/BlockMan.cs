using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlockMan : MonoBehaviour
{
    public CodeGet CodeGet;

    public ActRecord[] actRecords;

    void Start()
    {
        actRecords = FindObjectsOfType<ActRecord>();
    }

    public void GiveName()
    {
        for (int i = 0; i < actRecords.Length; i++)
        {//初始化所有记录仪的数组长度
            actRecords[i].givenNames = new string[CodeGet.LocNums.GetLength(0)];
            actRecords[i].givenValues = new string[CodeGet.LocStates.GetLength(0)];
        }

        for (int i = 0; i < actRecords.Length; i++)//遍历场景地块
        {
            actRecords[i].givenNames = CodeGet.names;//给予所有状态作弊码主人名字
            for (int j = 0; j < CodeGet.LocNums.GetLength(1); j++)//遍历作弊码长度
            {
                if (actRecords[i].gameObject.name.Substring(3) == CodeGet.LocNums[0,j])
                {//如果地块编码匹配                    
                    for (int k = 0; k < CodeGet.LocNums.GetLength(0); k++)
                    {
                        actRecords[i].givenValues[k] = CodeGet.LocStates[k, j];//给予所有状态作弊码地块状态
                    }
                }
            }
        }
    }
}
