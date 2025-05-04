using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class FadeOutUi : MonoBehaviour
{
    // Start is called before the first frame update
    public float fadeoutTime = 30f; // Fade out duration
    public float time = 0; // Time counter
    public Image[] images; // UI element to fade out
    public TextMeshProUGUI[] textMeshProUGUIs; // Text elements to fade out
    void Start()
    {
        images = GetComponentsInChildren<Image>();
        textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
        time = 0; // Reset the time counter
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime; // Increment the time counter
        // Check if the fade out time has passed
        if (time>= fadeoutTime)
        {
            // Call the fade out function
            UIFadeOut();
            //gameObject.SetActive(false); // Deactivate the game object
        }
    }

    void UIFadeOut()
    {
        // Fade out logic here
        //找到子物体所有Image以及TextMeshProUGUI组件,DoFadeOut

        foreach (Image image in images)
        {
            image.DOFade(0, 1f); // Fade out the image over 1 second
        }
        foreach (TextMeshProUGUI textMeshProUGUI in textMeshProUGUIs)
        {
            textMeshProUGUI.DOFade(0, 1f).OnComplete(() =>
            {
                // Fade out the text over 1 second and then deactivate the game object
                gameObject.SetActive(false); // Deactivate the game object after fading out
            });
        }

    }
}
