using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DiasGames.Abilities;
using DG.Tweening;
using UnityEngine.Playables;
namespace DiasGames.Controller
{
public class EndGame : MonoBehaviour
{
    [Header("结算UI设置")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject endGameAnimation;
    [SerializeField] private Light RockLight;
    [SerializeField] private TextMeshProUGUI shareCodeText;
    [SerializeField] private Button copyButton;

    // [SerializeField] private AudioClip EndBgm;
    // [SerializeField] private AudioSource BgmPlayer;
    //[SerializeField] private Cinemachine.CinemachineVirtualCamera[] otherCameras;
    //通过playableDirector控制的虚拟相机终场动画
    [SerializeField] private PlayableDirector endGameAnimationDirector;


    [Header("可选设置")]
    [SerializeField] private string gameOverMessage = "GameOver!Share the Cheet Code To Your Friends:";
    
    private bool isGameOver = false;
    
    void Start()
    {
        // 确保结算界面初始隐藏
        if (endGamePanel)
            endGamePanel.SetActive(false);
            
        // 添加复制按钮事件监听
    }
    
    void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家触发
        if (other.CompareTag("Player") && !isGameOver)
        {
            Debug.Log("ENd!");
            StopPlayerMovement(other.gameObject);
            RockLight.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            endGameAnimation.SetActive(true);
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource != null && audioSource != endGameAnimationDirector.GetComponent<AudioSource>())
                {
                    audioSource.Stop();
                }
            }
            endGameAnimationDirector.Play();
            //PlayEndBgm();

            StartCoroutine(ShowGameOverUI(other.gameObject));
        }
    }
    
    void StopPlayerMovement(GameObject player)
    {
        // 停止玩家的移动和其他操作
        var playerController = player.GetComponent<CSPlayerController>();
        if (playerController)
            playerController.enabled = false;
        
        // 其他停止操作
    }
    // void PlayEndBgm()
    // {
    //     if (BgmPlayer && EndBgm)
    //     {
    //         // Fade out audio over 1 second
    //         DOTween.To(() => BgmPlayer.volume, x => BgmPlayer.volume = x, 0, 1f)
    //             .OnComplete(() => BgmPlayer.Stop());
    //         // BgmPlayer.clip = EndBgm;
    //         // BgmPlayer.Play();
    //     }
    // }
    IEnumerator ShowGameOverUI(GameObject player)
    {
        yield return new WaitForSeconds(30f);
        isGameOver = true;
        

        //显示鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // 生成分享码
        string shareCode = GenerateShareCode(player);
        
        // 显示UI
        if (endGamePanel && shareCodeText)
        {
            endGamePanel.SetActive(true);
            shareCodeText.text = gameOverMessage + "\n" + shareCode;
        }
    }
    
    string GenerateShareCode(GameObject player)
    {
        
        return FindAnyObjectByType<GenerateCheatCode>().GenCheatCode();
    }
    
    public void CopyShareCodeToClipboard()
    {
        if (string.IsNullOrEmpty(shareCodeText.text))
            return;
            
        // 提取分享码（去掉开头的提示文字）
        string codeOnly = shareCodeText.text.Substring(shareCodeText.text.LastIndexOf("\n") + 1);
        
        // 复制到剪贴板
        GUIUtility.systemCopyBuffer = codeOnly;
        
        // 提示反馈
        Debug.Log("Copy success!");
        
        // 可以在UI上显示"已复制"提示
        StartCoroutine(ShowCopiedFeedback());
    }
    
    IEnumerator ShowCopiedFeedback()
    {
        string originalText = copyButton.GetComponentInChildren<TextMeshProUGUI>().text;
        copyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Copy Success!";
        
        yield return new WaitForSeconds(1.5f);
        
        copyButton.GetComponentInChildren<TextMeshProUGUI>().text = originalText;
    }
}
}
