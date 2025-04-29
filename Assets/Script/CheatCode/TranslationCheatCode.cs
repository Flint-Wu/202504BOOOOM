using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
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
    public void TranslateCheatCode()
    {
        // GetComponent<ReadCode>().SetCheatCode();
        GetComponent<ReadCode>().SplitCC();
        GetComponent<ReMadeID>().ReTrans();
        GetComponent<ReMadeLoc>().ReTrans();
        //TotalLocunNums.Add<ReMadeLoc>().LocunNums.ToArray().ToList();
        
        //TotalLocunNums add HelperID and LocNum
        for (int i = 0; i < GetComponent<ReMadeLoc>().LocunNums.Length; i++)
        {
            PlayerLocMap playerLocMap = new PlayerLocMap();
            playerLocMap.PlayerID = GetComponent<ReMadeID>().HelperID;
            playerLocMap.LocNum = GetComponent<ReMadeLoc>().LocunNums[i];
            TotalLocunNums.Add(playerLocMap);
        }
    }
}
