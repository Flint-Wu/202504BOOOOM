using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public struct LocationData
{
    public string[] locunStates; // 地块状态数组（索引对应编号）
    public string[] locunNums;   // 地块编号数组（索引对应状态）

   
   
    // 构造函数（确保数组长度一致）
    public LocationData(string[] states, string[] nums)
    {
        if (states.Length != nums.Length)
        {
            Debug.LogError("地块状态和编号数组长度不一致，数据无效！");
            locunStates = new string[0];
            locunNums = new string[0];
        }
        else
        {
            locunStates = states;
            locunNums = nums;
        }
    }
}
    public class ReCodeHander : MonoBehaviour
    {
        private static ReCodeHander instance;
        public static ReCodeHander Instance => instance;
    public Button strButton;
    public InputField InputZB;
    // 自动获取场景中的 ReMadeID 和 ReMadeLoc 组件（需确保唯一性）

    //private ReMadeID targetReMadeID;
    //private ReMadeLoc targetReMadeLoc;
    private Dictionary<string, LocationData> idToLocationMap = new Dictionary<string, LocationData>();

        void Awake()
        {
            instance = this;
       
        
            
        }
    public void Start()
    { 
        var idComponents = GetComponentsInChildren<ReMadeID>();
        var locComponents = GetComponentsInChildren<ReMadeLoc>();
        strButton.onClick.AddListener(() =>
        {  if (InputZB.text == "")
            {
                return;
            }
            else
            {
                // 按组件一一对应（假设 ReMadeID 和 ReMadeLoc 一一对应，且数量相同）
                for (int i = 0; i < idComponents.Length && i < locComponents.Length; i++)
                {
                    var helperID = idComponents[i].HelperID;
                    var states = locComponents[i].LocunStates;
                    var nums = locComponents[i].LocunNums;
                    AddData(helperID, states, nums); // 调用添加数据的方法
                }
            }
        
          
        });

        }
       
    // 获取 HelperID（调用 ReTrans() 后生效）
    //public string GetHelperID()
    //{
    //    if (targetReMadeID == null) return string.Empty;
    //    targetReMadeID.ReTrans(); // 执行 ID 转换逻辑
    //    return targetReMadeID.HelperID; // 直接访问公共字段
    //}

    //// 获取 LocunStates 和 LocunNums（调用 ReTrans() 后生效）
    //public (string[], string[]) GetLocationData()
    //{
    //    if (targetReMadeLoc == null) return (null, null);
    //    targetReMadeLoc.ReTrans(); // 执行位置数据转换
    //    return (targetReMadeLoc.LocunStates, targetReMadeLoc.LocunNums);
    //}
    public void AddData(string helperID, string[] locunStates, string[] locunNums)
        {
            if (string.IsNullOrEmpty(helperID))
            {
                Debug.LogError("helperID 为空，无法添加数据");
                return;
            }
            if (locunStates.Length != locunNums.Length)
            {
                Debug.LogError($"地块数据长度不一致（状态：{locunStates.Length}，编号：{locunNums.Length}）");
                return;
            }
            // 覆盖已有数据或新增
            idToLocationMap[helperID] = new LocationData(locunStates, locunNums);
            Debug.Log($"成功添加数据：{helperID}");
        }

        // 外部调用：通过 helperID 获取地块数据
        public LocationData GetLocationDataByID(string helperID)
        {
            if (idToLocationMap.TryGetValue(helperID, out var data))
            {
                return data; // 返回有效数据
            }
            Debug.LogWarning($"未找到对应 helperID 的数据：{helperID}");
            return new LocationData(); // 返回空数据（默认长度为 0）
        }
    }

