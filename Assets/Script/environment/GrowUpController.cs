using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DiasGames.Abilities;
using System;
//[ExecuteInEditMode]
public class GrowUpController : MonoBehaviour
{
    // Start is called before the first frame update
    //public List<Material> materials = new List<Material>();
    
    //[Range(0, 20)] public float GrowUp;
    public bool isPour;
    public bool CanBeUsed;
    public int PourNumMax = 0;
    public int PouNum = 0;
    public List<string> PlayerIDs ;
    public float InitGrow = 5f;
    public bool isTrigerEffect = false;
    public GameObject HealEffectPrefab;
    public SkinnedMeshRenderer[] smrs;
    void Start()
    {
        // //得到所有子物体的材质球
        // materials.Clear();
        // Renderer[] renderers = GetComponentsInChildren<Renderer>();
        // foreach (Renderer renderer in renderers)
        // {
        //     //获取材质球
        //     Material[] mats = renderer.materials;
        //     foreach (Material mat in mats)
        //     {
        //         if (!materials.Contains(mat))
        //         {
        //             materials.Add(mat);
        //             mat.SetFloat("_grow", InitGrow); // 初始化GrowUp属性
        //         }
        //     }
        // }
        //遍历所有材质球
        
    }

    void Awake()
    {
        // for (int i = 0; i < materials.Count; i++)
        // {
        //     materials[i].SetFloat("_grow", InitGrow+ PouNum * 5f); // 初始化GrowUp属性
        // }
        if(PouNum == PourNumMax)
        {
            CanBeUsed = true; // 可以使用
        }
        else
        {
            CanBeUsed = false; // 不可以使用
        }
        switch (PouNum)
        {
            // case 1:
            //     // 通过名称查找BlendShape索引
            //     // 通过名称查找第一个BlendShape索引
            //     //smrs[0]下所有的blendshape都设置为0
            //     string blendShapeName1 = "blendShape1.pCube5";
            //     int blendShapeIndex1 = smrs[0].sharedMesh.GetBlendShapeIndex(blendShapeName1);
                
            //     break;
            // case 2:
            //     // 通过名称查找BlendShape索引
            //     blendShapeName1 = "blendShape2.pCube4"; // 替换为您的BlendShape名称
            //     blendShapeIndex1 = smrs[1].sharedMesh.GetBlendShapeIndex(blendShapeName1);
            //         // 使用DOTween实现平滑过渡
                
            //     blendShapeName2 = "blendShape2.pCube3"; // 替换为您的BlendShape名称
            //     blendShapeIndex2 = smrs[1].sharedMesh.GetBlendShapeIndex(blendShapeName2);
            //         // 使用DOTween实现平滑过渡
            //     smrs[1].SetBlendShapeWeight(blendShapeIndex1, 0f); // 设置初始值为0
            //     smrs[1].SetBlendShapeWeight(blendShapeIndex2, 100f); // 设置初始值为100
            //     break;
            // case 3:
            //     // 通过名称查找BlendShape索引
            //     blendShapeName1 = "blendShape1.pCube4"; // 替换为您的BlendShape名称
            //     blendShapeIndex1 = smrs[0].sharedMesh.GetBlendShapeIndex(blendShapeName1);
                                
            //     blendShapeName2 = "blendShape1.pCube3"; // 替换为您的BlendShape名称
            //     blendShapeIndex2 = smrs[0].sharedMesh.GetBlendShapeIndex(blendShapeName2);
            //         // 使用DOTween实现平滑过渡
            //     smrs[0].SetBlendShapeWeight(blendShapeIndex1, 0f); // 设置初始值为0
            //     smrs[0].SetBlendShapeWeight(blendShapeIndex2, 100f); // 设置初始值为100
            //     break;
        }
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
        GetComponent<ActRecord>().LocState = "1"; // 设置记录仪的状态为钉子

    }

    public void RecoverAllPhysicalStrength()
    {

        if(isTrigerEffect) return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject healEffect = Instantiate(HealEffectPrefab, player.transform.position, Quaternion.identity);
        player.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = player.GetComponent<PlayerPhysicalStrength>().maxPhysicalStrength;
        isTrigerEffect = true;
        Annotation.Instance.Reset(); // 显示注释
    }

    void JudgeIsGrowUp()
    {

        TreeGrowUp(InitGrow + PouNum * 5f); // 设置材质球的GrowUp属性
        
    }
    public void TreeGrowUp(float grow)
    {
        switch (PouNum)
        {
            case 1:
                
                // 通过名称查找BlendShape索引
                // 通过名称查找第一个BlendShape索引
                string blendShapeName1 = "blendShape1.pCube5";
                int blendShapeIndex1 = smrs[0].sharedMesh.GetBlendShapeIndex(blendShapeName1);

                // 通过名称查找第二个BlendShape索引
                string blendShapeName2 = "blendShape1.pCube4";
                int blendShapeIndex2 = smrs[0].sharedMesh.GetBlendShapeIndex(blendShapeName2);

                // 同时启动两个DOTween动画，它们会并行执行
                DOTween.To(() => smrs[0].GetBlendShapeWeight(blendShapeIndex1), 
                        x => smrs[0].SetBlendShapeWeight(blendShapeIndex1, x), 
                        0f, 3f);

                DOTween.To(() => smrs[0].GetBlendShapeWeight(blendShapeIndex2), 
                        x => smrs[0].SetBlendShapeWeight(blendShapeIndex2, x), 
                        100f, 3f);
                break;
            case 2:
                // 通过名称查找BlendShape索引
                blendShapeName1 = "blendShape2.pCube4"; // 替换为您的BlendShape名称
                blendShapeIndex1 = smrs[1].sharedMesh.GetBlendShapeIndex(blendShapeName1);
                    // 使用DOTween实现平滑过渡
                
                blendShapeName2 = "blendShape2.pCube3"; // 替换为您的BlendShape名称
                blendShapeIndex2 = smrs[1].sharedMesh.GetBlendShapeIndex(blendShapeName2);
                    // 使用DOTween实现平滑过渡
                DOTween.To(() => smrs[1].GetBlendShapeWeight(blendShapeIndex1), x => smrs[1].SetBlendShapeWeight(blendShapeIndex1, x), 0f, 3f);
                DOTween.To(() => smrs[1].GetBlendShapeWeight(blendShapeIndex2), x => smrs[1].SetBlendShapeWeight(blendShapeIndex2, x), 100f, 3f);
                break;  
            case 3:
                // 通过名称查找BlendShape索引
                blendShapeName1 = "blendShape1.pCube4"; // 替换为您的BlendShape名称
                blendShapeIndex1 = smrs[0].sharedMesh.GetBlendShapeIndex(blendShapeName1);
                                
                blendShapeName2 = "blendShape1.pCube3"; // 替换为您的BlendShape名称
                blendShapeIndex2 = smrs[0].sharedMesh.GetBlendShapeIndex(blendShapeName2);
                    // 使用DOTween实现平滑过渡
                DOTween.To(() => smrs[0].GetBlendShapeWeight(blendShapeIndex1), x => smrs[0].SetBlendShapeWeight(blendShapeIndex1, x), 0f, 3f);

                    // 使用DOTween实现平滑过渡
                DOTween.To(() => smrs[0].GetBlendShapeWeight(blendShapeIndex2), x => smrs[0].SetBlendShapeWeight(blendShapeIndex2, x), 100f, 3f);
                break;   

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(CanBeUsed) 
        {    // 其他逻辑
            Annotation.Instance.AnnotationRecoverOnTree();
            InteractionManger.RecoverPhysicalStrength += RecoverAllPhysicalStrength; // 订阅事件
            return;
        }
        if(isPour) return;
        InteractionManger.PourWater += PourWater; // 订阅事件
        Annotation.Instance.AnnotationPourWater(); // 显示注释
        Debug.Log("可以浇水了！");

    }

    void OnTriggerExit(Collider other)
    {
        try
        {
            InteractionManger.RecoverPhysicalStrength -= RecoverAllPhysicalStrength; // 取消事件订阅
        }
        catch {}
        
        InteractionManger.PourWater -= PourWater; // 取消事件订阅
        Annotation.Instance.Reset(); // 显示注释

    }
}
