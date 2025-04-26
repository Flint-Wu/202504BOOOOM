using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Annotation : MonoBehaviour
{
    // Start is called before the first frame update
    public static Annotation Instance;
    public TextMeshProUGUI annotationText; // 显示注释的UI文本
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

    public void AnnotationPourWater()
    {
        //遍历所有材质球
        annotationText.text = "press E to pour water"; // 更新UI文本
    }
}
