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
        // 使用协程延迟执行，确保TranslationCheatCode的初始化完成
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        // 等待2帧，确保所有Start方法都执行完毕
        yield return null;
        yield return null;
        
        // 再等待一段时间，确保异步操作完成
        yield return new WaitForSeconds(0.1f);
        
        // 执行实例化
        InstantiateAllPrefab();
    }
    void InstantiateAllPrefab()
    {        
        try
        {
            // 检查TranslationCheatCode是否存在
            TranslationCheatCode translationCheatCode = FindAnyObjectByType<TranslationCheatCode>();
            if (translationCheatCode == null)
            {
                Debug.LogError("找不到TranslationCheatCode组件");
            
            }
            
            // 检查TotalLocunNums是否有效
            List<TranslationCheatCode.PlayerLocMap> totalLocunNums = translationCheatCode.TotalLocunNums;
            if (totalLocunNums.Count == 0)
            {
                Debug.LogWarning("TotalLocunNums为空或未初始化");
            }

            
            // 获取所有ActRecord并检查
            ActRecord[] actRecords = FindObjectsOfType<ActRecord>();
            if (actRecords == null || actRecords.Length == 0)
            {
                Debug.LogWarning("场景中没有ActRecord组件");
            }
            
            // 处理所有ActRecord
            for (int i = 0; i < actRecords.Length; i++)
            {
                if (actRecords[i] == null) continue;
                
                for (int j = 0; j < totalLocunNums.Count; j++)
                {
                    // 检查条件部分
                    if (actRecords[i].LocNum == null || totalLocunNums[j].LocNum == null) 
                        continue;
                        
                    // 处理树木 (LocNum > 200)
                    if (actRecords[i].LocNum == totalLocunNums[j].LocNum && int.Parse(actRecords[i].LocNum) > 200)
                    {
                        // 确保GrowUpController存在
                        GrowUpController growController = actRecords[i].transform.GetComponent<GrowUpController>();
                        if (growController != null)
                        {
                            growController.PlayerIDs.Add(totalLocunNums[j].PlayerID);
                            growController.PouNum += 1;
                            growController.UpdateInitState();
                            Debug.Log("树木ID为" + actRecords[i].LocNum + "的玩家ID为" + totalLocunNums[j].PlayerID + "浇水次数为" + growController.PouNum);
                        }
                    }
                    // 处理果实 (LocNum < 200)
                    else if (actRecords[i].LocNum == totalLocunNums[j].LocNum && int.Parse(actRecords[i].LocNum) < 200)
                    {
                        try
                        {
                            // 创建射线
                            Ray ray = new Ray(
                                actRecords[i].transform.position - actRecords[i].transform.right * Random.Range(0.8f, 1.2f) + actRecords[i].transform.forward, 
                                -actRecords[i].transform.forward
                            );
                            
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit, 100f))
                            {
                                // 实例化果实
                                GameObject matureFruit = Instantiate(MatureFruit, hit.point + hit.normal * 0.2f, Quaternion.identity);
                                
                                // 确保WaterBottleFruit组件存在
                                WaterBottleFruit fruitComponent = matureFruit.GetComponentInChildren<WaterBottleFruit>();
                                if (fruitComponent != null)
                                {
                                    fruitComponent.PlayerID = totalLocunNums[j].PlayerID;
                                    Debug.Log("果实ID为" + actRecords[i].LocNum + "的玩家ID为" + totalLocunNums[j].PlayerID);
                                }
                                
                                // 设置旋转
                                matureFruit.transform.rotation = Quaternion.LookRotation(hit.normal);
                            }
                            else
                            {
                                Debug.LogWarning("射线没有击中任何物体，无法生成果实: ActRecord ID=" + actRecords[i].LocNum);
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("生成果实时出错: " + e.Message);
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("作弊码未成功加载: " + e.Message);
        }
    }

}


