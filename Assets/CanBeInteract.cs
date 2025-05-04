using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanBeInteract : MonoBehaviour
{
    // Start is called before the first frame update
    public bool BeInteract;
    public bool BeSelect; // 是否可以被选择

    void Update()
    {
        if (!BeSelect)
        {
            // 显示UI
            if (GetComponentInChildren<Image>() != null)
            {
                Image image = GetComponentInChildren<Image>();
                image.gameObject.SetActive(false); // 显示UI
            }
        }
    }

}
