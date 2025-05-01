using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Beginnig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public string GetUserName()
    {
        string userName = System.Environment.UserName;
        return userName;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Cavebottom");
            EffectSoundController.Instance.PlayUIConfirmAudioClip();
        }    
    }
}
