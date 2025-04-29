using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
public class WindRegion : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject WindVfx;
    public float PlayerEnterPhysicalStrength;//记录玩家进入风区的的体力
    public float WindPeiod;//风区的持续时间
    public bool IsWindBegin = false;//是否开始吹风
    //玩家进入风区的体力的百分比
    [Header("玩家进入风区的体力的消减百分比")]
    public float PlayerEnterWindStrengthPer = 0.05f;//记录玩家进入风区的的体力,百分比
    public float WindTime = 0;//开始时间
    public Material[] grassMaterial;
    public GlobalGrassRenderer grassRenderer;
    //public bool isPlayerIn;
    private GameObject player;
    private PlayerPhysicalStrength playerPhysicalStrength;
    public bool isPlayerIn = false;
    void Start()
    {
        WindTime = 0;
        //设置vfx的边界和风区的大小一致
        //WindVfx.transform.localScale = this.GetComponent<BoxCollider>().size;
        player = GameObject.FindGameObjectWithTag("Player");
        playerPhysicalStrength = player.GetComponent<PlayerPhysicalStrength>();
    }

    // Update is called once per frame
    void Update()
    {
        WindTime += Time.deltaTime;
        InverseWind();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
            if(IsWindBegin)
            {
                playerPhysicalStrength.isInWinZone = true;
            }
            PlayerEnterPhysicalStrength = playerPhysicalStrength.currentPhysicalStrength;
        }
    }

    void LockPlayerPhysical()
    {

        float maxPhysicalStrength = playerPhysicalStrength.maxPhysicalStrength;
        playerPhysicalStrength.stopRecovering();
        playerPhysicalStrength.currentPhysicalStrength = PlayerEnterWindStrengthPer * maxPhysicalStrength;

        
    }

    void RecoverPlayerPhysical()
    {
        if (player.CompareTag("Player"))
        {
            playerPhysicalStrength.currentPhysicalStrength = PlayerEnterPhysicalStrength;
            //other.GetComponent<PlayerPhysicalStrength>().startRecovering();
        }
    }

    void OnTriggerExit(Collider other)
    {
        //离开风区时，恢复玩家的体力值
        isPlayerIn = false;
        playerPhysicalStrength.isInWinZone = false;
        RecoverPlayerPhysical();

    }

    void InverseWind()
    {
        if (WindTime > WindPeiod)
        {
            if (IsWindBegin)
            {
                WindVfx.SetActive(false);
                IsWindBegin = false;
                WindTime = 0;
                if(playerPhysicalStrength.isInWinZone)
                {
                    playerPhysicalStrength.isInWinZone = false;
                }
                // for (int i = 0; i < grassMaterial.Length; i++)
                // {
                //     grassMaterial[i].SetFloat("_WindSpeed", 2);
                // }
                // if(grassRenderer != null)
                //     grassRenderer.ForceRefresh();
                if(!playerPhysicalStrength.isInWinZone)return;
                RecoverPlayerPhysical();
            }
            else if (!IsWindBegin)
            {
                WindVfx.SetActive(true);
                IsWindBegin = true;
                WindTime = 0;
                // for (int i = 0; i < grassMaterial.Length; i++)
                // {
                //     grassMaterial[i].SetFloat("_WindSpeed", 12);
                // }
                // if(grassRenderer != null)
                //     grassRenderer.ForceRefresh();
                if(isPlayerIn)
                {
                    playerPhysicalStrength.isInWinZone = true;
                }
                if(!playerPhysicalStrength.isInWinZone)return;
                LockPlayerPhysical();
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw a wire sphere to visualize the trigger area in the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, this.GetComponent<BoxCollider>().size);
    }
}
