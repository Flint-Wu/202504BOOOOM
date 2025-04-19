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
    void Start()
    {
        WindTime = 0;
        //设置vfx的边界和风区的大小一致
        WindVfx.transform.localScale = this.GetComponent<BoxCollider>().size;
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
            PlayerEnterPhysicalStrength = other.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength;
            float maxPhysicalStrength = other.GetComponent<PlayerPhysicalStrength>().maxPhysicalStrength;
            other.GetComponent<PlayerPhysicalStrength>().stopRecovering();
            if (PlayerEnterPhysicalStrength/maxPhysicalStrength >PlayerEnterWindStrengthPer)
            {
                other.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = PlayerEnterWindStrengthPer * maxPhysicalStrength;
            }
            else
            {
                //如果玩家的体力值小于风区消减的体力值，则对玩家体力不做任何处理
                return;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //离开风区时，恢复玩家的体力值
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = PlayerEnterPhysicalStrength;
            //other.GetComponent<PlayerPhysicalStrength>().startRecovering();
        }
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
            }
            else if (IsWindBegin == false)
            {
                WindVfx.SetActive(true);
                IsWindBegin = true;
                WindTime = 0;
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
