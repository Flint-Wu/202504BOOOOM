using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WindRegion : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject WindVfx;
    public float PlayerEnterPhysicalStrength;//记录玩家进入风区的的体力
    public float WindPeiod;//风区的持续时间
    public bool IsWindBegin = false;//是否开始吹风
    //玩家进入风区的体力的百分比
    [Header("玩家进入风区的体力的消减百分比")]
    public float PlayerEnterWindStrengthPer;//记录玩家进入风区的的体力,百分比
    public float WindTime = 0;//开始时间
    public Material[] grassMaterial;
    public GlobalGrassRenderer grassRenderer;
    public bool isPlayerIn;
    private GameObject player;
    void Start()
    {
        WindTime = 0;
        //设置vfx的边界和风区的大小一致
        WindVfx.transform.localScale = this.GetComponent<BoxCollider>().size;
        player = GameObject.FindGameObjectWithTag("Player");
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
        }
    }

    void ReducePlayerPhysical()
    {

        PlayerEnterPhysicalStrength = player.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength;
        float maxPhysicalStrength = player.GetComponent<PlayerPhysicalStrength>().maxPhysicalStrength;
        player.GetComponent<PlayerPhysicalStrength>().stopRecovering();
        if (PlayerEnterPhysicalStrength/maxPhysicalStrength >PlayerEnterWindStrengthPer)
        {
            player.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = PlayerEnterWindStrengthPer * maxPhysicalStrength;
        }
        else
        {
            //如果玩家的体力值小于风区消减的体力值，则对玩家体力不做任何处理
            return;
        }
        
    }

    void RecoverPlayerPhysical()
    {
        if (player.CompareTag("Player"))
        {
            player.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = PlayerEnterPhysicalStrength;
            //other.GetComponent<PlayerPhysicalStrength>().startRecovering();
        }
    }

    void OnTriggerExit(Collider other)
    {
        //离开风区时，恢复玩家的体力值
        isPlayerIn = false;

    }

    void InverseWind()
    {
        if (WindTime > WindPeiod)
        {
            if (IsWindBegin == true)
            {
                WindVfx.SetActive(false);
                IsWindBegin = false;
                WindTime = 0;
                for (int i = 0; i < grassMaterial.Length; i++)
                {
                    grassMaterial[i].SetFloat("_WindSpeed", 2);
                }
                grassRenderer.ForceRefresh();
                if(!isPlayerIn)return;
                RecoverPlayerPhysical();
            }
            else if (IsWindBegin == false)
            {
                WindVfx.SetActive(true);
                IsWindBegin = true;
                WindTime = 0;
                for (int i = 0; i < grassMaterial.Length; i++)
                {
                    grassMaterial[i].SetFloat("_WindSpeed", 12);
                }
                grassRenderer.ForceRefresh();
                if(!isPlayerIn)return;
                ReducePlayerPhysical();
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
