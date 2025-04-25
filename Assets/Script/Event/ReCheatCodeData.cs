using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReCheatCodeData
{
    public string helperID;//作弊码对应的ID
    public string[] locunStates; //地块状态编号
    public string[] locunNums;//地块编号数组
    
    //构造函数 初始化数据
    public ReCheatCodeData(string id, string[] states, string[] nums)
    {
        helperID = id;
        locunStates = states;
        locunNums = nums;
    }
   
    
    
}
