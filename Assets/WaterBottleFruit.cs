using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBottleFruit : MonoBehaviour
{
    // Start is called before the first frame update
    public float waterAmount = 20f; // 水果的水量
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 获取玩家的 PlayerWaterState 组件
            PlayerWaterState playerWaterState = other.GetComponent<PlayerWaterState>();
            if (playerWaterState != null)
            {
                // 增加水量
                playerWaterState.ChangeWater(waterAmount);
                Debug.Log("Player water increased by " + waterAmount);
                // 销毁水果对象
                Destroy(gameObject);
            }
        }
    }
}
