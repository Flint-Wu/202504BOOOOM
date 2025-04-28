using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

public class TriDic : MonoBehaviour
{
    public string[] locStates;
    public string[] locNums;
    public List<string> locZbID = new List<string>();

    [Header("更新loc信息")]
    public bool isUpID= true;
    public bool isUpLoc;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpID)
        {
            UpdataLocZbID();
            locZbID = locZbID.Distinct().ToList();
        }
        if (isUpLoc)
        {
            UpdateLoc();
            locNums = locNums.Distinct().ToArray();
            locStates = locStates.ToArray();
        }
    }
public void UpdataLocZbID()
    {
        if (ReCodeHander.Instance.idToLocationMap == null)

        { return; }

        foreach (var zbID in ReCodeHander.Instance.idToLocationMap.Keys)
        {
           locZbID.Add(zbID);

        }
        isUpID = false;
    }
 public void UpdateLoc()
    {       if(ReCodeHander.Instance.idToLocationMap==null)
        {
            return;
        }
          
       locNums = ReCodeHander.Instance.GetLocationInfoByID(locZbID[locZbID.Count-1]).locunNums;
        locStates = ReCodeHander.Instance.GetLocationInfoByID(locZbID[locZbID.Count - 1]).locunStates;

        if (isUpLoc)
        {
            isUpLoc = false;
        }
          
       
    }
}
