using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using UnityEngine;

public class WaterBottleFruit : MonoBehaviour
{
    // Start is called before the first frame update
    public float waterAmount = 20f; // 水果的水量
    public string PlayerID = "Player"; // 玩家ID
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //可以拾取水果的物体
        Debug.Log("Player entered the trigger area.");
        InteractionManger.GetFruit += GetFruit; // 订阅事件
        Annotation.Instance.AnnotationFruit(); // 显示注释
    }
    void GetFruit()
    {
        // 获取玩家的 PlayerWaterState 组件
        PlayerWaterState playerWaterState = GameObject.FindGameObjectWithTag(PlayerID).GetComponent<PlayerWaterState>();
        if (playerWaterState != null)
        {
            // 增加水量
            playerWaterState.ChangeWater(waterAmount);
            Debug.Log("Player water increased by " + waterAmount);
            // 销毁水果对象
            Destroy(gameObject);
        }
        Annotation.Instance.Reset(); // 重置注释
        InteractionManger.GetFruit -= GetFruit;
        
    }
    void OnTriggerExit(Collider other)
    {
        InteractionManger.GetFruit -= GetFruit; // 取消订阅事件
        Annotation.Instance.Reset(); // 重置注释
    }
}
