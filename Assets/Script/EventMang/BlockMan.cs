using System.Collections;
using System.Collections.Generic;
using DiasGames.Climbing;
using UnityEditor;
using UnityEngine;

public class BlockMan : MonoBehaviour
{
    public CodeGet CodeGet;

    public ActRecord[] actRecords;

<<<<<<< HEAD
    void Start()
=======
    public string strings;

    void Awake()
>>>>>>> main
    {
        actRecords = FindObjectsOfType<ActRecord>();
        SortAllLocation();
        InitAllActRecord();
    }
    void InitAllActRecord()
    {
        int TreeNum = 0;
        int RelaxNum = 0;
        //初始化所有record,并赋值给各个ActRecord
        for (int i = 0; i < actRecords.Length; i++)
        {
            //如果其transform挂载有Tree，LocNum以2xx+1为准
            if (actRecords[i].gameObject.GetComponent<GrowUpController>() != null)
            {
                TreeNum +=1;
                actRecords[i].LocNum = "20" + TreeNum.ToString();
            }
            else if (actRecords[i].gameObject.GetComponent<Ledge>() != null)
            {
                RelaxNum += 1;
                actRecords[i].LocNum = RelaxNum.ToString();
            }
        }
        Debug.Log("所有记录仪初始化完毕！");
    }

    void SortAllLocation()
    {
        //根据actRecords每个元素的transform position从小到大进行排序
        //使用冒泡排序算法进行排序
        for (int i = 0; i < actRecords.Length - 1; i++)
        {
            for (int j = 0; j < actRecords.Length - i - 1; j++)
            {
                if (actRecords[j].transform.position.y > actRecords[j + 1].transform.position.y)
                {
                    //交换位置
                    ActRecord temp = actRecords[j];
                    actRecords[j] = actRecords[j + 1];
                    actRecords[j + 1] = temp;
                }
            }
        }    
    }
    
    public void GiveName()
    {
        for (int i = 0; i < actRecords.Length; i++)
        {//初始化所有记录仪的数组长度
            actRecords[i].givenNames = new List<string>(CodeGet.LocNums.GetLength(0));
            actRecords[i].givenValues = new List<string>(CodeGet.LocStates.GetLength(0));
        }

        for (int i = 0; i < actRecords.Length; i++)//遍历场景地块
        {
            actRecords[i].givenNames = new List<string>(CodeGet.names);//给予所有状态作弊码主人名字
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
