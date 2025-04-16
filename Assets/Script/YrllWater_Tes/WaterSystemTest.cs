using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaterSystemTest : MonoBehaviour
{
    
        [Header("组件引用")]
        [SerializeField] private PlayerWaterState waterState;
        [SerializeField] private Slider waterSlider;
        [SerializeField] private Text percentText;
        
        [SerializeField] private GameObject gameOverPanel;
        
    [SerializeField] private Button QTEbutton;
    [SerializeField] private Button WaterBottle;
    [SerializeField] private Button RestWater;


    [Header("调试设置")]
        [SerializeField] private Color normalColor = Color.cyan;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private int maxLogEntries = 5;

    private void Start()
    {
        // 初始化UI
        UpdateWaterUI(waterState.CurrentWater / waterState.MaxWater);
       
        // 绑定事件
        waterState.OnWaterChanged.AddListener(UpdateWaterUI);
          waterState.OnWaterDepleted.AddListener(ShowGameOver);
        QTEbutton.onClick.AddListener(() =>
        {
            Debug.Log("按钮被点击！", this);
            waterState.ChangeWater(); // 调用默认流失量

        });
        WaterBottle.onClick.AddListener(() =>
        {
            waterState.ChangeWater(20f); // 增加20单位

        });
        RestWater.onClick.AddListener(() =>
        {
            waterState.ResetWater();
        });

        gameOverPanel.SetActive(false);
    }
    

    // ====== UI更新方法 ======
    private void UpdateWaterUI(float percent)
        {
            // 更新进度条
            waterSlider.value = percent;
            percentText.text = $"{(percent * 100):F1}%";

            // 颜色渐变
            waterSlider.fillRect.GetComponent<Image>().color =
                Color.Lerp(criticalColor, normalColor, percent);
        }

      

        private void ShowGameOver()
        {
            gameOverPanel.SetActive(true);
        
        }

      
    private void OnDestroy()
{
    waterState.OnWaterChanged.RemoveListener(UpdateWaterUI);
    // 应解绑所有监听器
}
    
}
