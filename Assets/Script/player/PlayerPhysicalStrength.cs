using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using UnityEngine;
using DG.Tweening;
public class PlayerPhysicalStrength : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerPhysicalStrength Instance;
    private void Awake()
    {
        Instance = this;
    }
    public int maxPhysicalStrength = 100;
    public int minPhysicalStrength = 5;
    public float currentPhysicalStrength = 100;
    [Header("攀爬中跳跃、平地奔跑所需的体力消耗")]
    public float JumpStrength = 20f;
    public float RunStrength = 10f;
    [Header("攀爬时悬挂每秒所需的体力消耗")]
    public float ClimbIdleStrength = 5f;
    [Header("平地站立时每秒恢复的体力")]
    public float StandRecoverStrength = 5f;

    public bool isRecovering = false;   
    public bool isInWinZone = false; //是否在风区内
    void Start()
    {
        currentPhysicalStrength = maxPhysicalStrength;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ExhaustPhysicalStrength();
        RecoverPhysicalStrength(StandRecoverStrength * Time.deltaTime);
    }
    public void ReducePhysicalStrength(float amount)
    {
        if (isInWinZone) 
        {
            currentPhysicalStrength = FindAnyObjectByType<WindRegion>().PlayerEnterWindStrengthPer*maxPhysicalStrength;
            return;
        }
        currentPhysicalStrength -= amount;
        if (currentPhysicalStrength <= minPhysicalStrength)
        {
            currentPhysicalStrength = minPhysicalStrength;
        }
    }
    public void stopRecovering()
    {
        isRecovering = false;
    }
    public void startRecovering()
    {
        isRecovering = true;
        EffectSoundController.Instance.PlayResetPhysicalAudioClip();
    }
    public void RecoverPhysicalStrength(float amount)
    {
        //在风吹区域内，体力恢复为0
        if(!isRecovering) return;
        currentPhysicalStrength += amount;
        if (currentPhysicalStrength > maxPhysicalStrength)
        {
            currentPhysicalStrength = maxPhysicalStrength;
            stopRecovering();
        }
    }

    public void ExhaustPhysicalStrength()
    {
        //如果当前状态机的变量Motion Speed为0
        if (currentPhysicalStrength <= 0)
        {
            //如果当前状态机的变量Motion Speed为0
            GetComponent<ClimbAbility>().OnStopAbility();
            GetComponent<ClimbAbility>().Drop();
        }
    }

}
