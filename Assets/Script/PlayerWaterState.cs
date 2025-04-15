using DiasGames.Components;
using DiasGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//�ⲿ����״̬ϵͳ��private PlayerWaterState  
//
public class PlayerWaterState : MonoBehaviour
{   
    /// <summary>
    ///���ò���,�������ⲿ�޸�
    /// </summary>
    [Header("Water Settings")]
    [SerializeField] private float _maxWater = 100f;
    public float MaxWater => _maxWater; // ֻ������
    [SerializeField] private float defaultSpray = 10f; // Ĭ�ϵ�����ʧ��
    [SerializeField] private float criticalThreshold = 0.2f; // Σ����ֵ���ٷֱȣ�

    /// <summary>
    /// �����¼��ӿ� ��ϵͳ����>�ⲿ
    /// </summary>
    public UnityEvent<float> OnWaterChanged;       // ˮ���仯����������ǰˮ���ٷֱȣ�
    public UnityEvent OnWaterCritical;             // ����Σ��״̬
    public UnityEvent OnWaterDepleted;             // ˮ���ľ�
   

    /// <summary>
    /// ��������
    /// </summary>
    public float CurrentWater { get; private set; }
    public bool IsInCritical { get; private set; }
    private bool _isDepleted = false;
    //��ʼ������
    private void Awake()
    {
        OnWaterChanged ??= new UnityEvent<float>();
        OnWaterCritical = OnWaterCritical ?? new UnityEvent();
        OnWaterDepleted = OnWaterDepleted ?? new UnityEvent();
      
        CurrentWater = MaxWater;
    }

    /// <summary>
    /// �ⲿ���õı�׼��ˮ�ӿڣ�ʹ��Ĭ����ʧ����
    /// QTEʧ��ʱ,ֱ�ӵ���
    /// </summary>
    public void ChangeWater()
    {
        ModifyWater(-defaultSpray);
    }

    /// <summary>
    /// ����������ˮ�ӿڣ������ɸ���
    /// �����Զ�����ˮ��,������ˤ������
    /// </summary>
    /// <param name="amount">�������ӣ���������</param>
    public void ChangeWater(float amount)
    {
        ModifyWater(amount);
    }

    /// <summary>
    /// ����ˮ�������ֵ
    /// ���¼��ػ�浵��ظ�?
    /// </summary>
    public void ResetWater()
    {
        _isDepleted = false;
        CurrentWater = MaxWater;
        IsInCritical = false;
        OnWaterChanged?.Invoke(1f);
    }

    /// <summary>
    /// ʵ��ˮ���޸ķ���(˽�л�ȷ�������ⲿ�޸�)
    /// �����߼�
    /// </summary>
    /// <param name="delta"></param>

    private void ModifyWater(float delta)
    {
        if (_isDepleted) return; // �Ѻľ�ʱ���ٴ���
        float previous = CurrentWater;
        CurrentWater = Mathf.Clamp(CurrentWater + delta, 0, MaxWater);

        // ����ˮ���仯�¼�
        if (!Mathf.Approximately(previous, CurrentWater))
        {
            float percent = CurrentWater / MaxWater;
            //����ˮ���仯�¼�,���ⲿ��UI�ȸ���
            OnWaterChanged?.Invoke(percent);
        }
        //�Ƿ���Ҫ����Σ�ձ���
        CheckCriticalState();
        //���ˮ���Ƿ�ľ�
        CheckDepletion();
 
    
    }


    /// <summary>
    ///  // Σ��״̬��� �ٷ�֮20Ԥ��
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
    /// ȼ����,Ҫ������
    /// </summary>
    private void CheckDepletion()
    {
        if (!_isDepleted && CurrentWater <= Mathf.Epsilon)
        {
            _isDepleted = true; // ���Ϊ�Ѻľ�
            OnWaterDepleted?.Invoke();
        }
        else if (CurrentWater > Mathf.Epsilon)
        {
            _isDepleted = false; // ����״̬
        }
    }
    /// <summary>
    /// �����л������,���ټ���
    /// </summary>
    private void OnDestroy()
    {
        OnWaterChanged.RemoveAllListeners();
        OnWaterCritical.RemoveAllListeners();
        OnWaterDepleted.RemoveAllListeners();
    }
}


