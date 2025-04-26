using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaterSystemTest : MonoBehaviour
{
    
        [Header("�������")]
        [SerializeField] private PlayerWaterState waterState;
        [SerializeField] private Slider waterSlider;
        [SerializeField] private Text percentText;
        
        [SerializeField] private GameObject gameOverPanel;
        
    [SerializeField] private Button QTEbutton;
    [SerializeField] private Button WaterBottle;
    [SerializeField] private Button RestWater;


    [Header("��������")]
        [SerializeField] private Color normalColor = Color.cyan;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private int maxLogEntries = 5;

    private void Start()
    {
        // ��ʼ��UI
        UpdateWaterUI(waterState.CurrentWater / waterState.MaxWater);
       
        // ���¼�
        waterState.OnWaterChanged.AddListener(UpdateWaterUI);
          waterState.OnWaterDepleted.AddListener(ShowGameOver);
        // QTEbutton.onClick.AddListener(() =>
        // {
        //     Debug.Log("��ť�������", this);
        //     waterState.ChangeWater(); // ����Ĭ����ʧ��

        // });
        // WaterBottle.onClick.AddListener(() =>
        // {
        //     waterState.ChangeWater(20f); // ����20��λ

        // });
        // RestWater.onClick.AddListener(() =>
        // {
        //     waterState.ResetWater();
        // });

        // gameOverPanel.SetActive(false);
    }
    

    // ====== UI���·��� ======
    private void UpdateWaterUI(float percent)
        {
            // ���½�����
            waterSlider.value = percent;
            percentText.text = $"{(percent * 100):F1}%";

            // ��ɫ����
            waterSlider.fillRect.GetComponent<Image>().color =
                Color.Lerp(criticalColor, normalColor, percent);
        }

      

        private void ShowGameOver()
        {
            //gameOverPanel.SetActive(true);
        
        }

      
    private void OnDestroy()
{
    waterState.OnWaterChanged.RemoveListener(UpdateWaterUI);
    // Ӧ������м�����
}
    
}
