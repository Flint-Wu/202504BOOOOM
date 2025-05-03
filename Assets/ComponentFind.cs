 
using UnityEngine;
 
public class ComponentFind : MonoBehaviour
{
    
    void Start()
    {
        FindObjectWithCompanent<AudioListener>();
    }
 
    void FindObjectWithCompanent<T>() where T : Component
    {
        foreach (var go in Resources.FindObjectsOfTypeAll<T>())
        {
            Debug.LogFormat("gameObject:{0},scene:{1}", go.gameObject.name, go.gameObject.scene.name);
        }
    }
}