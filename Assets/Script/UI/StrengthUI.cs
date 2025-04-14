using UnityEngine;
using UnityEngine.UI;
using DiasGames.Components;


public class StrengthUI : MonoBehaviour
{
    [SerializeField] private Image StrengthBar;
    [SerializeField] private PlayerPhysicalStrength characterStrength;

    void Update()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        StrengthBar.fillAmount = (float)characterStrength.currentPhysicalStrength / characterStrength.maxPhysicalStrength;
    }
}
