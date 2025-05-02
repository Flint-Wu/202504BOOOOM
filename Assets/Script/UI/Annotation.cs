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
    public GameObject waterFruitAnnotationUI; // 拿取水果注释UI
    public GameObject nailAnnotationUI; // 钉子注释UI
    public GameObject pourSeedAnnotationUI; // 水果注释UI
    public GameObject recoverOnTreeAnnotationUI; // 恢复到树上的注释UI
    public TextMeshProUGUI playerIDtext;
    void Start()
    {
        annotationText = GetComponent<TextMeshProUGUI>(); // 获取UI文本组件
        if(FindAnyObjectByType<MadeID>().GetComponent<MadeID>().ID != null)
        playerIDtext.text = FindAnyObjectByType<MadeID>().GetComponent<MadeID>().ID;
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
        //显示所有给树浇水的玩家ID
        EffectSoundController.Instance.PlaySpecailAudioClip();
        string playerID = string.Join(",", playerIDs); // 将玩家ID数组转换为字符串
        //treePourAnnotationUI.SetActive(true); // 隐藏水果注释UI
        RectTransform rectTransform = recoverOnTreeAnnotationUI.GetComponent<RectTransform>();
        float originalPosX = rectTransform.anchoredPosition.x; // 获取UI原始位置
        rectTransform.DOKill(true); // 杀死之前的动画，避免重叠
        Sequence sequence = DOTween.Sequence(); // 创建序列
        sequence.Append(rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack)); // 1. 移动到屏幕中央(X=0位置)
        sequence.AppendInterval(2.0f); // 2. 停留2秒
        sequence.Append(rectTransform.DOAnchorPosX(originalPosX, 0.5f).SetEase(Ease.InBack)); // 3. 返回原始位置
        // 设置文本
        recoverOnTreeAnnotationUI.GetComponentInChildren<TextMeshProUGUI>().text = playerID; // 更新UI文本
    }

    public void singleContributorJumpOut(string playerID,string type)
    {
        // 获取UI原始位置
        EffectSoundController.Instance.PlaySpecailAudioClip();
        RectTransform rectTransform;
        if (type == "waterFruit")
        {
            rectTransform = waterFruitAnnotationUI.GetComponent<RectTransform>();
        }
        else if (type == "nail")
        {
            rectTransform = nailAnnotationUI.GetComponent<RectTransform>();
        }
        else if (type == "pourSeed")
        {
            rectTransform = pourSeedAnnotationUI.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Invalid type: " + type);
            return;
        }
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
        rectTransform.GetComponentInChildren<TextMeshProUGUI>().text = playerID;
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
