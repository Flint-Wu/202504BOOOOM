using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class InventoryManager : MonoBehaviour
{
    // 只用来存储钉子物品
    public static InventoryManager Instance;
    public GameObject nailPrefab; // 钉子预制体
    public int maxNailCout = 3; // 最大钉子数量
    private int currentNailCount = 0; // 当前钉子数量
    public TextMeshProUGUI nailCountText; // 显示钉子数量的UI文本
    public GameObject nailUI; // 钉子UI
    public GameObject nailUIIconPrefab; // 钉子图标UI

    private void Awake()
    {
        Instance = this;
        InteractionManger.UseNail += CostNail; // 取消事件订阅
        for (int i = 0; i < maxNailCout; i++)
        {
            GameObject nailUIIcon = Instantiate(nailUIIconPrefab, nailUI.transform.position, Quaternion.identity); // 在钉子UI图标位置生成钉子
            nailUIIcon.transform.SetParent(nailUI.transform); // 设置钉子为钉子UI图标的子物体
            nailUIIcon.transform.localScale = new Vector3(1f, 1f, 1f); // 设置钉子的缩放
        }
    }
    private void OnDestroy()
    {
        InteractionManger.UseNail -= CostNail; // 取消事件订阅
    }
    void Start()
    {
        // 初始化钉子数量
        currentNailCount = maxNailCout;
        nailCountText.text = currentNailCount.ToString(); // 更新UI文本

    }
    public void CostNail()
    {
        if(currentNailCount <= 0)
        {
            Debug.Log("没有钉子了！");
            return;
        }

        //本人transform.position加上本人transform.right位置
        Ray ray = new Ray( transform.position + transform.right+ Vector3.up, transform.forward); // 射线从玩家位置发出
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)) // 射线检测
        {
            GameObject nail = Instantiate(nailPrefab, hit.point+hit.normal*0.2f, Quaternion.identity); // 在碰撞点生成钉子
            nail.transform.rotation = Quaternion.LookRotation(hit.normal); // 根据碰撞点的法线设置钉子的旋转
            Debug.Log("射线检测到物体: " + hit.collider.name); // 输出射线检测到的物体名称
        }
        else
        {
            Debug.Log("射线没有检测到任何物体"); // 输出射线没有检测到任何物体
            return;
        }
        currentNailCount--;
        nailCountText.text = currentNailCount.ToString(); // 更新UI文本
    
        nailUI.transform.GetChild(currentNailCount).gameObject.transform.GetChild(0).gameObject.transform.DOScale(new Vector3(0f, 0f, 0f), 0.5f).OnComplete(() =>
        {
            nailUI.transform.GetChild(currentNailCount).gameObject.transform.GetChild(0).gameObject.SetActive(false); // 隐藏钉子图标
        }); // 动画缩放钉子图标
        PlayerPhysicalStrength.Instance.startRecovering(); // 开始恢复体力
        GetComponent<ClimbAbility>()._currentCollider.GetComponent<ActRecord>().LocState = "1"; // 设置记录仪的状态为钉子
        GetComponent<ClimbAbility>()._currentCollider.GetComponent<ActRecord>().givenNames.Add(FindAnyObjectByType<MadeID>().ID); // 添加玩家名字到记录仪的给定名字列表中
    }
    public bool CanBuild()
    {
        // 检查是否可以建造
        if (currentNailCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("F1被按下了！"); // 输出按下F1的提示
            FindAnyObjectByType<GenerateCheatCode>().GenCheatCode(); // 生成作弊码
        }
    }
}
