using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using UnityEngine;

public class SnowZone : MonoBehaviour
{
    // Start is called before the first frame update
    public float SlipTime = 0.5f; //光标滑动的时间
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
            QTEUI.Instance.decayTime = SlipTime;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //离开风区时，恢复玩家的体力值
        if (other.CompareTag("Player"))
        {
            QTEUI.Instance.decayTime = 0f;
        }
    }

    void OnDrawGizmos()
    {
        // Draw a wire sphere to visualize the trigger area in the editor
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, this.GetComponent<BoxCollider>().size);
    }

}
