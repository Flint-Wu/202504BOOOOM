using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaxLoc : MonoBehaviour
{
    public ActRecord actRcord;
    public Transform[] gameObjects;

    void Start()
    {
        actRcord = GetComponent<ActRecord>();
        gameObjects = new Transform[transform.childCount];
        gameObjects = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if( int.Parse(actRcord.LocState) == 1 )//判定如果场景状态等于多少，切换到对应状态
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].gameObject.SetActive(false);
            }
        }
    }
}
