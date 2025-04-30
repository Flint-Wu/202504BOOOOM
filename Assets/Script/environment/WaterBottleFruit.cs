using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using DiasGames.Components;
using UnityEngine;

public class WaterBottleFruit : MonoBehaviour
{
    // Start is called before the first frame update
    public float waterAmount = 20f; // 水果的水量
    public string PlayerID = "Player"; // 玩家ID
    public bool isGet = false; // 是否被获取
    private GameObject fruitPrefab; // 水果预制体
    void Start()
    {
        fruitPrefab = this.transform.GetChild(0).gameObject; // 获取水果预制体
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (isGet) return; // 如果已经被获取，则不再执行
        //可以拾取水果的物体
        Debug.Log("Player entered the trigger area.");
        InteractionManger.GetFruit += GetFruit; // 订阅事件
        Annotation.Instance.AnnotationFruit(); // 显示注释
    }
    void GetFruit()
    {
        // 获取玩家的 PlayerWaterState 组件
        PlayerWaterState playerWaterState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWaterState>();
        if (playerWaterState != null)
        {
            // 增加水量
            playerWaterState.ChangeWater(waterAmount);
            isGet = true; // 设置为已获取
            Debug.Log("Player water increased by " + waterAmount);
            // 销毁水果对象
            fruitPrefab.SetActive(false); // 隐藏水果对象
            InteractionManger.GetFruit -= GetFruit; // 订阅事件
            Annotation.Instance.Reset(); // 显示注释 
            Annotation.Instance.waterFruitContributorJumpOut(PlayerID); // 显示水果注释
            CharacterAudioPlayer.Instance.PlayUseFruitAudioClip(); // 播放获取水果音效
            this.GetComponent<Collider>().enabled = false; // 禁用碰撞器
        }
        
    }
    void OnTriggerExit(Collider other)
    {
        if (isGet) return; // 如果已经被获取，则不再执行
        InteractionManger.GetFruit -= GetFruit; // 取消订阅事件
        Annotation.Instance.Reset(); // 重置注释
    }

    void OnDestroy()
    {
        InteractionManger.GetFruit -= GetFruit; // 取消订阅事件
        Annotation.Instance.Reset(); // 重置注释
    }
}
