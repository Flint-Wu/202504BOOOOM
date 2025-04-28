using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DiasGames.Abilities;
//[ExecuteInEditMode]
public class GrowUpController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Material> materials = new List<Material>();
    
    //[Range(0, 20)] public float GrowUp;
    public bool isPour;
    public bool CanBeUsed;
    public int PourNumMax = 0;
    public int PouNum = 0;
    public string[] PlayerIDs ;
    public float InitGrow = 5f;
    public bool isTrigerEffect = false;
    public GameObject HealEffectPrefab;
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
                    mat.SetFloat("_grow", InitGrow); // 初始化GrowUp属性
                }
            }
        }
        //遍历所有材质球
        
    }

    void Awake()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetFloat("_grow", InitGrow+ PouNum * 5f); // 初始化GrowUp属性
        }
        if(PouNum == PourNumMax)
        {
            CanBeUsed = true; // 可以使用
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

    }

    public void RecoverAllPhysicalStrength()
    {

        if(isTrigerEffect) return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject healEffect = Instantiate(HealEffectPrefab, player.transform.position, Quaternion.identity);
        player.GetComponent<PlayerPhysicalStrength>().currentPhysicalStrength = player.GetComponent<PlayerPhysicalStrength>().maxPhysicalStrength;
        isTrigerEffect = true;

    }

    void JudgeIsGrowUp()
    {

        TreeGrowUp(InitGrow + PouNum * 5f); // 设置材质球的GrowUp属性
        
    }
    public void TreeGrowUp(float grow)
    {
        foreach (Material mat in materials)
        {
            //设置材质球的GrowUp属性
            mat.DOFloat(grow, "_grow", 8f);
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
