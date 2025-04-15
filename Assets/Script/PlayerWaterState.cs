using DiasGames.Components;
using DiasGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//外部调用状态系统：private PlayerWaterState  
//
public class PlayerWaterState : MonoBehaviour
{   
    /// <summary>
    ///配置参数,可以在外部修改
    /// </summary>
    [Header("Water Settings")]
    [SerializeField] private float _maxWater = 100f;
    public float MaxWater => _maxWater; // 只读属性
    [SerializeField] private float defaultSpray = 10f; // 默认单次流失量
    [SerializeField] private float criticalThreshold = 0.2f; // 危险阈值（百分比）

    /// <summary>
    /// 对外事件接口 由系统——>外部
    /// </summary>
    public UnityEvent<float> OnWaterChanged;       // 水量变化（参数：当前水量百分比）
    public UnityEvent OnWaterCritical;             // 进入危险状态
    public UnityEvent OnWaterDepleted;             // 水量耗尽
   

    /// <summary>
    /// 公共属性
    /// </summary>
    public float CurrentWater { get; private set; }
    public bool IsInCritical { get; private set; }
    private bool _isDepleted = false;
    //初始化数据
    private void Awake()
    {
        OnWaterChanged ??= new UnityEvent<float>();
        OnWaterCritical = OnWaterCritical ?? new UnityEvent();
        OnWaterDepleted = OnWaterDepleted ?? new UnityEvent();
      
        CurrentWater = MaxWater;
    }

    /// <summary>
    /// 外部调用的标准洒水接口（使用默认流失量）
    /// QTE失败时,直接调用
    /// </summary>
    public void ChangeWater()
    {
        ModifyWater(-defaultSpray);
    }

    /// <summary>
    /// 带参数的洒水接口（可正可负）
    /// 可以自定义洒水量,适用于摔落等情况
    /// </summary>
    /// <param name="amount">正数增加，负数减少</param>
    public void ChangeWater(float amount)
    {
        ModifyWater(amount);
    }

    /// <summary>
    /// 重置水量到最大值
    /// 重新加载或存档点回复?
    /// </summary>
    public void ResetWater()
    {
        _isDepleted = false;
        CurrentWater = MaxWater;
        IsInCritical = false;
        OnWaterChanged?.Invoke(1f);
    }

    /// <summary>
    /// 实际水量修改方法(私有化确保不被外部修改)
    /// 核心逻辑
    /// </summary>
    /// <param name="delta"></param>

    private void ModifyWater(float delta)
    {
        if (_isDepleted) return; // 已耗尽时不再处理
        float previous = CurrentWater;
        CurrentWater = Mathf.Clamp(CurrentWater + delta, 0, MaxWater);

        // 触发水量变化事件
        if (!Mathf.Approximately(previous, CurrentWater))
        {
            float percent = CurrentWater / MaxWater;
            //触发水量变化事件,对外部如UI等更新
            OnWaterChanged?.Invoke(percent);
        }
        //是否需要触发危险报警
        CheckCriticalState();
        //检测水量是否耗尽
        CheckDepletion();
 
    
    }


    /// <summary>
    ///  // 危险状态检测 百分之20预警
    /// </summary>
    private void CheckCriticalState()
    {
        float currentPercent = CurrentWater / MaxWater;
        bool isNowCritical = currentPercent <= criticalThreshold;

        if (isNowCritical != IsInCritical)
        {
            IsInCritical = isNowCritical;
            if (IsInCritical)
                OnWaterCritical?.Invoke();
        }
    }

    /// <summary>
    /// 燃尽了,要似了呢
    /// </summary>
    private void CheckDepletion()
    {
        if (!_isDepleted && CurrentWater <= Mathf.Epsilon)
        {
            _isDepleted = true; // 标记为已耗尽
            OnWaterDepleted?.Invoke();
        }
        else if (CurrentWater > Mathf.Epsilon)
        {
            _isDepleted = false; // 重置状态
        }
    }
    /// <summary>
    /// 场景切换等情况,销毁监听
    /// </summary>
    private void OnDestroy()
    {
        OnWaterChanged.RemoveAllListeners();
        OnWaterCritical.RemoveAllListeners();
        OnWaterDepleted.RemoveAllListeners();
    }
}


