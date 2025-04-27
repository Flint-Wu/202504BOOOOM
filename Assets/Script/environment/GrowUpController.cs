using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DiasGames.Abilities;
[ExecuteInEditMode]
public class GrowUpController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Material> materials = new List<Material>();
    
    //[Range(0, 20)] public float GrowUp;
    public bool isPour;
    public int PouNumMax = 0;
    public int PouNum = 0;
    public string[] PlayerIDs ;
    void Start()
    {
        //得到所有子物体的材质球
        materials.Clear();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            //获取材质球
            Material[] mats = renderer.materials;
            foreach (Material mat in mats)
            {
                if (!materials.Contains(mat))
                {
                    materials.Add(mat);
                    mat.SetFloat("_grow", 0f); // 初始化GrowUp属性
                }
            }
        }
        //遍历所有材质球
        
    }

    // Update is called once per frame

    public void PourWater()
    {
        if(isPour) return;
        PouNum++;
        isPour = true;
        Debug.Log("浇水了！");
        JudgeIsGrowUp(); // 判断是否可以生长
        InteractionManger.PourWater -= PourWater; // 取消事件订阅
        Annotation.Instance.Reset(); // 显示注释
        Annotation.Instance.TreePouContributorJumpOut(PlayerIDs); // 显示注释

    }

    void JudgeIsGrowUp()
    {
        if (PouNum >= PouNumMax)
        {
            //如果浇水次数大于等于最大浇水次数，则调用树木生长方法
            TreeGrowUp();
        }
    }
    public void TreeGrowUp()
    {
        foreach (Material mat in materials)
        {
            //设置材质球的GrowUp属性
            mat.DOFloat(20f, "_grow", 8f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(isPour) return;
        InteractionManger.PourWater += PourWater; // 订阅事件
        Annotation.Instance.AnnotationPourWater(); // 显示注释
        Debug.Log("可以浇水了！");
    }

    void OnTriggerExit(Collider other)
    {
        InteractionManger.PourWater -= PourWater; // 取消事件订阅
        Annotation.Instance.Reset(); // 显示注释

    }
}
