using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActRecord : MonoBehaviour
{
    public string LocState;
    public string LocNum;

    public List<string> givenNames;
    public List<string> givenValues;

    void Start()
    {
        try
        {
            //LocState = gameObject.name.Substring(0, 2);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("ActRecord: " + e.Message);
        }
    }
    
}
