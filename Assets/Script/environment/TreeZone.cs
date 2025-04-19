using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeZone : MonoBehaviour
{
    public bool isTriigerEffect = false;
    public GameObject HealEffectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(isTriigerEffect) return;
        if (other.CompareTag("Player"))
        {
            GameObject healEffect = Instantiate(HealEffectPrefab, other.transform.position, Quaternion.identity);
            other.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = other.GetComponent<PlayerPhysicalStrength>().maxPhysicalStrength;
            isTriigerEffect = true;
        }
    }
}
