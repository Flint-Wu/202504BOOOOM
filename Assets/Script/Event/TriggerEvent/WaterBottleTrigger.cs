using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterBottleTrigger : MonoBehaviour
{
     private PlayerWaterState waterState;

  

   private ActRecord actR;
    private void Start()
    {
        actR = GetComponent<ActRecord>();

        GameObject player = GameObject.Find("WaterManager");
        if (player != null)
        {
            waterState = player.GetComponent<PlayerWaterState>();
        }
    
      
    }
    private void OnTriggerEnter(Collider other)
    {
        // 检查自身或触发对象是否是Player标签
        bool isPlayerInvolved =  tag == "Player" ||  other.CompareTag("Player");
        //print(ReCodeHander.Instance.GetHelperID());
        if (isPlayerInvolved)
        {
            Debug.Log($"Player触发进入 - 对象: {other.name}");
            HandlePlayerTriggerEnter(other); // 触发具体逻辑
        }
    }

    private void HandlePlayerTriggerEnter(Collider other)
    {
        waterState.ChangeWater(20f);
        Destroy(gameObject, 1f);
    }
}
