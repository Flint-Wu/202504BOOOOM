using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActRecord : MonoBehaviour
{
    public string LocState;
    public string LocNum;

    public string[] givenNames;
    public string[] givenValues;

    void Start()
    {
        LocNum = gameObject.name.Substring(3);
    }

    
}
