using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using UnityEngine;

public class PlayerPhysicalStrength : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerPhysicalStrength Instance;
    private void Awake()
    {
        Instance = this;
    }
    public int maxPhysicalStrength = 100;
    public float currentPhysicalStrength = 100;
    [Header("攀爬中跳跃、平地奔跑所需的体力消耗")]
    public float JumpStrength = 20f;
    public float RunStrength = 10f;
    [Header("攀爬时悬挂每秒所需的体力消耗")]
    public float ClimbIdleStrength = 5f;
    [Header("平地站立时每秒恢复的体力")]
    public float StandRecoverStrength = 5f;
    private Animator animator;
    void Start()
    {
        currentPhysicalStrength = maxPhysicalStrength;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ActionChangePhysicalStrength();
        ExhaustPhysicalStrength();
    }
    public void ReducePhysicalStrength(float amount)
    {
        currentPhysicalStrength -= amount;
        if (currentPhysicalStrength < 0)
        {
            currentPhysicalStrength = 0;
        }
    }
    public void RecoverPhysicalStrength(float amount)
    {
        currentPhysicalStrength += amount;
        if (currentPhysicalStrength > maxPhysicalStrength)
        {
            currentPhysicalStrength = maxPhysicalStrength;
        }
    }

    void ActionChangePhysicalStrength()
    {
        //如果当前animator的状态机为Grounded的状态
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            //如果当前状态机的变量Motion Speed为0
            if (animator.GetFloat("Motion Speed") == 0)
            {
                RecoverPhysicalStrength(StandRecoverStrength * Time.deltaTime);
            }
            else
            {
                ReducePhysicalStrength(RunStrength * Time.deltaTime);
            }
        }
        //如果当前状态机为Climb的状态 且 动画为idle
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //如果当前状态机的变量Motion Speed为0
            ReducePhysicalStrength(ClimbIdleStrength * Time.deltaTime);
        }

    }

    public void ExhaustPhysicalStrength()
    {
        //如果当前状态机的变量Motion Speed为0
        if (currentPhysicalStrength <= 0)
        {
            //如果当前状态机的变量Motion Speed为0
            GetComponent<ClimbAbility>().OnStopAbility();
        }
    }
    public void FailedOnQTE()
    {
        //如果当前状态机的变量Motion Speed为0
        GetComponent<ClimbAbility>().OnStopAbility();
    }

}
