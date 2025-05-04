using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DiasGames.Components;
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

    public bool isTrigerEffect = false;
    public GameObject HealEffectPrefab;
    public SkinnedMeshRenderer[] smrs;
    public GameObject matureTreePrefab;
    public GameObject[] unmatureTreePrefab;
    public GameObject annoationImage; // 注释图片
    void Start()
    {
        UpdateInitState(); // 初始化状态
    }

    void Awake()
    {
        // for (int i = 0; i < materials.Count; i++)
        // {
        //     materials[i].SetFloat("_grow", InitGrow+ PouNum * 5f); // 初始化GrowUp属性
        // }
        //限制最大值

    }

    // Update is called once per frame
    public void UpdateInitState()
    {
        PouNum = Mathf.Clamp(PouNum, 0, PourNumMax);
        if(PouNum >= PourNumMax)
        {
            CanBeUsed = true; // 可以使用
            matureTreePrefab.SetActive(true); // 显示成熟树预制体
            for (int i = 0; i < unmatureTreePrefab.Length; i++)
            {
                unmatureTreePrefab[i].SetActive(false); // 隐藏未成熟树预制体
            }
        }
        else
        {
            CanBeUsed = false; // 不可以使用
        }
        switch (PouNum)
        {
            case 0:
                int blendShapeIndex = smrs[0].sharedMesh.GetBlendShapeIndex("blendShape1.pCube5");
                int blendShapeIndex2 = smrs[1].sharedMesh.GetBlendShapeIndex("blendShape2.pCube4");
                InitSetBlendShape(smrs[0], blendShapeIndex); // 设置BlendShape索引为0
                InitSetBlendShape(smrs[1], blendShapeIndex2); // 设置BlendShape索引为1
                break;
            case 1:
                int blendShapeIndex3= smrs[0].sharedMesh.GetBlendShapeIndex("blendShape1.pCube4");
                int blendShapeIndex4 = smrs[1].sharedMesh.GetBlendShapeIndex("blendShape2.pCube4");
                InitSetBlendShape(smrs[0], blendShapeIndex3); // 设置BlendShape索引为2
                InitSetBlendShape(smrs[1], blendShapeIndex4); // 设置BlendShape索引为3
                break;
            case 2:
                int blendShapeIndex5= smrs[0].sharedMesh.GetBlendShapeIndex("blendShape1.pCube4");
                int blendShapeIndex6 = smrs[1].sharedMesh.GetBlendShapeIndex("blendShape2.pCube3");
                InitSetBlendShape(smrs[0], blendShapeIndex5); // 设置BlendShape索引为2
                InitSetBlendShape(smrs[1], blendShapeIndex6); // 设置BlendShape索引为3
                break;
        }
    }
    public void PourWater()
    {
        if(isPour) return;
        PouNum++;
        isPour = true;
        TreeGrowUp(); // 判断是否可以生长
        Annotation.Instance.Reset(transform.gameObject); // 显示注释
        Annotation.Instance.TreePouContributorJumpOut(PlayerIDs); // 显示注释
        Annotation.Instance.singleContributorJumpOut(FindAnyObjectByType<MadeID>().ID, "pourSeed"); // 显示水果注释
        GetComponent<ActRecord>().LocState = "1"; // 设置记录仪的状态为钉子
        CharacterAudioPlayer.Instance.PlayPourWaterAudioClip(); // 播放浇水的音效
        EffectSoundController.Instance.PlayPourTreeAudioClip(PouNum); // 播放浇水的音效
    }

    public void RecoverAllPhysicalStrength()
    {

        if(isTrigerEffect) return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject healEffect = Instantiate(HealEffectPrefab, player.transform.position, Quaternion.identity);
        player.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = player.GetComponent<PlayerPhysicalStrength>().maxPhysicalStrength;
        isTrigerEffect = true;
        Annotation.Instance.Reset(); // 显示注释
        Annotation.Instance.TreePouContributorJumpOut(PlayerIDs); // 显示注释
        CharacterAudioPlayer.Instance.PlayUseTreeAudioClip(); // 播放使用树的音效
    }

    public void TreeGrowUp()
    {
        switch (PouNum)
        {
            case 1:
                int blendShapeIndex = smrs[0].sharedMesh.GetBlendShapeIndex("blendShape1.pCube4");
                DotweenBlendShape(unmatureTreePrefab[0].GetComponent<SkinnedMeshRenderer>(), blendShapeIndex); // 设置BlendShape索引为0
                break;
            case 2:
                int blendShapeIndex2 = smrs[1].sharedMesh.GetBlendShapeIndex("blendShape2.pCube3");
                DotweenBlendShape(unmatureTreePrefab[1].GetComponent<SkinnedMeshRenderer>(), blendShapeIndex2); // 设置BlendShape索引为1
                break;
            case 3:
                int blendShapeIndex3 = smrs[0].sharedMesh.GetBlendShapeIndex("blendShape1.pCube3");
                DotweenBlendShape(unmatureTreePrefab[0].GetComponent<SkinnedMeshRenderer>(), blendShapeIndex3); // 设置BlendShape索引为2
                break;
        }
    }
    public void DotweenBlendShape(SkinnedMeshRenderer smr, int blendShapeIndex)
    {
        // 先设置目标 BlendShape 为 100
        DOTween.To(() => smr.GetBlendShapeWeight(blendShapeIndex), x => smr.SetBlendShapeWeight(blendShapeIndex, x), 100, 2f);
        Debug.Log("BlendShape索引为" + blendShapeIndex + "的权重值将变为100");
        
        // 处理所有其他 BlendShape
        for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
        {
            if (i != blendShapeIndex && smr.GetBlendShapeWeight(i) > 0)
            {
                // 创建局部变量，解决闭包问题
                int index = i;
                float currentWeight = smr.GetBlendShapeWeight(index);
                
                // 只有当当前权重大于 0 时才执行动画
                if (currentWeight > 0)
                {
                    DOTween.To(
                        () => smr.GetBlendShapeWeight(index), 
                        x => smr.SetBlendShapeWeight(index, x), 
                        0, 
                        2f
                    ).OnStart(() => {
                        Debug.Log("开始动画：BlendShape索引 " + index + " 从 " + currentWeight + " 到 0");
                    }).OnComplete(() => {
                        Debug.Log("完成动画：BlendShape索引 " + index + " 现在值为 " + smr.GetBlendShapeWeight(index));
                    });
                }
            }
        }
    }
    public void InitSetBlendShape(SkinnedMeshRenderer smr, int blendShapeIndex)
    {
        smr.SetBlendShapeWeight(blendShapeIndex, 100);
        for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
        {
            if (i != blendShapeIndex)
            {
                smr.SetBlendShapeWeight(i, 0f); // 设置初始值为0
            }
        }
    }

}
