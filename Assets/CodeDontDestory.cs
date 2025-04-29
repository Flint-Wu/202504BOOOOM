using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeDontDestory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //Debug.Log("DontDestroyOnLoad: " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
