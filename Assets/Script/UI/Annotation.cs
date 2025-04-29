using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public class Annotation : MonoBehaviour
{
    // Start is called before the first frame update
    public static Annotation Instance;
    public TextMeshProUGUI annotationText; // 显示注释的UI文本
    public GameObject waterFruitAnnotationUI; // 水果注释UI
    public GameObject treePourAnnotationUI; // 倒水注释UI
    void Start()
    {
        annotationText = GetComponent<TextMeshProUGUI>(); // 获取UI文本组件
    }
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Reset()
    {
        annotationText.text = ""; // 重置UI文本
    }
    public void AnnotationFruit()
    {
        //遍历所有材质球
        annotationText.text = "press E to pick up the fruit"; // 更新UI文本
    }
    public void TreePouContributorJumpOut(List<string> playerIDs)
    {
        //遍历所有材质球
        string playerID = string.Join(",", playerIDs); // 将玩家ID数组转换为字符串
        //treePourAnnotationUI.SetActive(true); // 隐藏水果注释UI
        RectTransform rectTransform = treePourAnnotationUI.GetComponent<RectTransform>();
        float originalPosX = rectTransform.anchoredPosition.x; // 获取UI原始位置
        rectTransform.DOKill(true); // 杀死之前的动画，避免重叠
        Sequence sequence = DOTween.Sequence(); // 创建序列
        sequence.Append(rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack)); // 1. 移动到屏幕中央(X=0位置)
        sequence.AppendInterval(2.0f); // 2. 停留2秒
        sequence.Append(rectTransform.DOAnchorPosX(originalPosX, 0.5f).SetEase(Ease.InBack)); // 3. 返回原始位置
        // 设置文本
        treePourAnnotationUI.GetComponentInChildren<TextMeshProUGUI>().text = playerID; // 更新UI文本
    }

    public void waterFruitContributorJumpOut(string playerID)
    {
        // 获取UI原始位置
        RectTransform rectTransform = waterFruitAnnotationUI.GetComponent<RectTransform>();
        float originalPosX = rectTransform.anchoredPosition.x;
        // 杀死之前的动画，避免重叠
        rectTransform.DOKill(true);
        // 创建序列
        Sequence sequence = DOTween.Sequence();
        // 1. 移动到屏幕中央(X=0位置)
        sequence.Append(rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack));
        // 2. 停留2秒
        sequence.AppendInterval(2.0f);
        // 3. 返回原始位置
        sequence.Append(rectTransform.DOAnchorPosX(originalPosX, 0.5f).SetEase(Ease.InBack));
        // 设置文本
        waterFruitAnnotationUI.GetComponentInChildren<TextMeshProUGUI>().text = playerID;
    }

    public void AnnotationPourWater()
    {
        //遍历所有材质球
        annotationText.text = "press E to pour water"; // 更新UI文本
    }

    public void AnnotationRecoverOnTree()
    {
        //遍历所有材质球
        annotationText.text = "press E to Recover On Tree"; // 更新UI文本
    }
}
