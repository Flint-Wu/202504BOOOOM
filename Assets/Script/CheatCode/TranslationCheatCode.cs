using System.Collections;
using System.Collections.Generic;
using TMPro;

// Removing System.Diagnostics to avoid ambiguity with UnityEngine.Debug
using UnityEngine;

public class TranslationCheatCode : MonoBehaviour
{
    [System.Serializable]
    public class PlayerLocMap
    {
        public string PlayerID;
        public string LocNum;
    }
    public MadeConcateAll madeConcateAll;
    public List<PlayerLocMap> TotalLocunNums = new List<PlayerLocMap>();
    public TextMeshProUGUI TextMeshProUGUI;
    public void TranslateCheatCode()
    {
        // GetComponent<ReadCode>().SetCheatCode();
        GetComponent<ReadCode>().SplitCC();
        GetComponent<ReMadeID>().ReTrans();
        GetComponent<ReMadeLoc>().ReTrans();
        //TotalLocunNums.Add<ReMadeLoc>().LocunNums.ToArray().ToList();
        
        //如果HelperID重复，则不添加
        if(TotalLocunNums.Count > 0)
        {
            for (int i = 0; i < TotalLocunNums.Count; i++)
            {
                if (TotalLocunNums[i].PlayerID == GetComponent<ReMadeID>().HelperID)
                {
                    Debug.LogWarning("Repeat Player CheatCode: " + TotalLocunNums[i].PlayerID + " " + TotalLocunNums[i].LocNum);
                    TextMeshProUGUI.text = "Repeat Player CheatCode: " + TotalLocunNums[i].PlayerID;
                    StartCoroutine(ResetWarning());
                    return;
                }
            }
        }
        //TotalLocunNums add HelperID and LocNum
        for (int i = 0; i < GetComponent<ReMadeLoc>().LocunNums.Length; i++)
        {
            PlayerLocMap playerLocMap = new PlayerLocMap();
            playerLocMap.PlayerID = GetComponent<ReMadeID>().HelperID;
            playerLocMap.LocNum = GetComponent<ReMadeLoc>().LocunNums[i];
            
            TotalLocunNums.Add(playerLocMap);
        }
    }

    IEnumerator ResetWarning()
    {
        yield return new WaitForSeconds(2f);
        TextMeshProUGUI.text = "";

    }
}
