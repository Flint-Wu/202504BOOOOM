using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelInit : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject MatureFruit;
    void Start()
    {
        FindAnyObjectByType<TranslationCheatCode>().TranslateCheatCode();
        List<TranslationCheatCode.PlayerLocMap> totalLocunNums = FindAnyObjectByType<TranslationCheatCode>().TotalLocunNums;
        
        ActRecord[] actRecords = FindObjectsOfType<ActRecord>();
        //对actRecords进行排序

        //找到所有locunstates = "1"的actrecord，赋值给其transform挂载的物体
        for(int i = 0; i<actRecords.Length; i++)
        {
            for(int j = 0; j<totalLocunNums.Count; j++)
            {
                if (actRecords[i].LocNum == totalLocunNums[j].LocNum && int.Parse(actRecords[i].LocNum) > 200)
                {
                    actRecords[i].transform.GetComponent<GrowUpController>().PlayerIDs.Add(totalLocunNums[j].PlayerID);
                    actRecords[i].transform.GetComponent<GrowUpController>().PouNum += 1;
                    Debug.Log("树木ID为"+actRecords[i].LocNum+"的玩家ID为"+totalLocunNums[j].PlayerID+"浇水次数为"+actRecords[i].transform.GetComponent<GrowUpController>().PouNum);
                }
                else if (actRecords[i].LocNum == totalLocunNums[j].LocNum && int.Parse(actRecords[i].LocNum) < 200)
                {
                    Ray ray = new Ray( actRecords[i].transform.position - actRecords[i].transform.right*Random.Range(0.8f,1.2f)+actRecords[i].transform.forward, -actRecords[i].transform.forward); // 射线从玩家位置发出
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f)) // 射线检测
                    {
                        GameObject matureFruit = Instantiate(MatureFruit, hit.point+hit.normal*0.2f, Quaternion.identity); // 在碰撞点生成钉子
                        matureFruit.GetComponentInChildren<WaterBottleFruit>().PlayerID = totalLocunNums[j].PlayerID;
                        Debug.Log("果实ID为"+actRecords[i].LocNum+"的玩家ID为"+totalLocunNums[j].PlayerID);

                        matureFruit.transform.rotation = Quaternion.LookRotation(hit.normal); // 根据碰撞点的法线设置钉子的旋转
                    }
            }
        }
        }
    }

}


