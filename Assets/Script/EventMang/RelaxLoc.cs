using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlockState))]
[RequireComponent(typeof(ActRecord))]
public class RelaxLoc : MonoBehaviour
{
    public ActRecord actRcord;
    public Transform[] gameObjects;

    //ÐèÒª×é¼þBlockState£¬ActRecord
    void Start()
    {
        actRcord = GetComponent<ActRecord>();
        gameObjects = new Transform[transform.childCount];
        gameObjects = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if( int.Parse(actRcord.LocState) == 1 )//ï¿½Ð¶ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½×´Ì?ï¿½ï¿½ï¿½Ú¶ï¿½ï¿½Ù£ï¿½ï¿½Ð»ï¿½ï¿½ï¿½ï¿½ï¿½Ó¦×´Ì¬
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].gameObject.SetActive(false);
            }
        }
    }
}
