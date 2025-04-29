using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DiasGames.Abilities;
namespace DiasGames.Controller
{
public class EndGame : MonoBehaviour
{
    [Header("结算UI设置")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI shareCodeText;
    [SerializeField] private Button copyButton;
    
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
            ShowGameOverUI(other.gameObject);
        }
    }
    
    void ShowGameOverUI(GameObject player)
    {
        isGameOver = true;
        
        // 冻结玩家
        // if (freezePlayerOnGameOver)
        // {
        var playerController = player.GetComponent<CSPlayerController>();
        if (playerController)
            playerController.enabled = false;
                
        //     var rigidbody = player.GetComponent<Rigidbody>();
        //     if (rigidbody)
        //         rigidbody.isKinematic = true;
        // }
        //tIMESCALE = 0;
        //Time.timeScale = 0;
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