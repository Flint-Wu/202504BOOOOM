using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaterSystemTest : MonoBehaviour
{
    
   
        [SerializeField] private PlayerWaterState waterState;
        [SerializeField] private Slider waterSlider;
        [SerializeField] private Text percentText;
        
        [SerializeField] private GameObject gameOverPanel;
        
    [SerializeField] private Button QTEbutton;
    [SerializeField] private Button WaterBottle;
    [SerializeField] private Button RestWater;


    [Header("ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½")]
        [SerializeField] private Color normalColor = Color.cyan;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private int maxLogEntries = 5;

    private void Start()
    {
        // ï¿½ï¿½Ê¼ï¿½ï¿½UI
        UpdateWaterUI(waterState.CurrentWater / waterState.MaxWater);


        // °ó¶¨ÊÂ¼þ
        waterState.OnWaterChanged.AddListener(UpdateWaterUI);
        waterState.OnWaterDepleted.AddListener(ShowGameOver);
        QTEbutton.onClick.AddListener(() =>
        {
            Debug.Log("°´Å¥±»µã»÷£¡", this);
            waterState.ChangeWater(); // µ÷ÓÃÄ¬ÈÏÁ÷Ê§Á¿


       
        // ï¿½ï¿½ï¿½Â¼ï¿½
        waterState.OnWaterChanged.AddListener(UpdateWaterUI);
          waterState.OnWaterDepleted.AddListener(ShowGameOver);
        // QTEbutton.onClick.AddListener(() =>
        // {
        //     Debug.Log("ï¿½ï¿½Å¥ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿?, this);
        //     waterState.ChangeWater(); // ï¿½ï¿½ï¿½ï¿½Ä¬ï¿½ï¿½ï¿½ï¿½Ê§ï¿½ï¿½

        // });
        // WaterBottle.onClick.AddListener(() =>
        // {
        //     waterState.ChangeWater(20f); // ï¿½ï¿½ï¿½ï¿½20ï¿½ï¿½Î»


        });
        WaterBottle.onClick.AddListener(() =>
        {
            waterState.ChangeWater(20); // Ôö¼Ó20µ¥Î»

        });
    }
    

    // ====== UIï¿½ï¿½ï¿½Â·ï¿½ï¿½ï¿½ ======
    private void UpdateWaterUI(float percent)
        {
            // ï¿½ï¿½ï¿½Â½ï¿½ï¿½ï¿½ï¿½ï¿½
            waterSlider.value = percent;
            percentText.text = $"{(percent * 100):F1}%";

            // ï¿½ï¿½É«ï¿½ï¿½ï¿½ï¿½
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
    // Ó¦ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ð¼ï¿½ï¿½ï¿½ï¿½ï¿?
}
    
}
