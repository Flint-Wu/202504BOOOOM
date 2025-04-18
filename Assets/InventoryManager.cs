using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 只用来存储钉子物品
    public static InventoryManager Instance;
    public GameObject nailPrefab; // 钉子预制体
    public int maxNailCout = 3; // 最大钉子数量
    private int currentNailCount = 0; // 当前钉子数量
    public TextMeshProUGUI nailCountText; // 显示钉子数量的UI文本
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // 初始化钉子数量
        currentNailCount = maxNailCout;

    }
    public void CostNail()
    {
        if (currentNailCount > 0)
        {
            currentNailCount--;
            Debug.Log("钉子数量减少，当前数量：" + currentNailCount);
        }
        else
        {
            Debug.Log("没有足够的钉子！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
