using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class BakTranButton : MonoBehaviour
{   //解决输入作弊码空引用问题,而生成作弊码strTrans因为场景上已经有挂载了Act脚本的物体,所以无空引用问题???可能吧
    private ReadCode rec;
    private ReMadeID reI;
    private ReMadeLoc reMloc;
    private InputField inputZB;
    // Start is called before the first frame update
    void Start()
    {
        rec = GetComponent<ReadCode>();
        reI = GetComponent<ReMadeID>();
        reMloc = GetComponent<ReMadeLoc>();
        GameObject InputZB = GameObject.Find("InputZB");
        if (InputZB != null )
        {
            inputZB = InputZB.GetComponent<InputField>();
        }
        
    }
 public void OnClickBakTrans()
    {
        if (inputZB.text != null)
        {
            BakTrans();
           
        }
        else
        {
            return;
        }
    }
public void BakTrans()
    {
        rec.SplitCC();
        reI.ReTrans();
        reMloc.ReTrans();
    }
  
}
