// BottleOutLine.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BottleOutLine : MonoBehaviour
{
    private PlayerWaterState playerWaterState;
    private Renderer renderer;
    
    void Start()
    {
        // 找到场景中的PlayerWaterState组件
        playerWaterState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWaterState>();
        renderer = GetComponent<Renderer>();
        // 订阅OnWaterCritical事件
        if (playerWaterState != null)
        {
            playerWaterState.OnWaterCritical.AddListener(WarningOutline);
            Debug.Log("成功订阅水量危机事件");
        }
        else
        {
            Debug.LogError("未找到PlayerWaterState组件");
        }
    }
    
    void OnDestroy()
    {
        // 移除监听器，避免内存泄漏
        if (playerWaterState != null)
        {
            playerWaterState.OnWaterCritical.RemoveListener(WarningOutline);
        }
    }
    
    public void WarningOutline()
    {
        Debug.Log("水量危机，开始轮廓效果");
        //Dotween实现_OutlineWidth在0.02-0.1循环变化
        renderer.material.DOFloat(0.05f, "_OutlineWidth", 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        // 在这里添加您想要在水量危机时执行的代码
        // 例如，激活水瓶轮廓效果
    }
}