using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActRecord : MonoBehaviour
{
    public string LocState;
    public string LocNum;

    void Start()
    {
        //LocNum = int.Parse(gameObject.name.Substring(3));
        LocNum = gameObject.name.Substring(3);
    }

    
}
