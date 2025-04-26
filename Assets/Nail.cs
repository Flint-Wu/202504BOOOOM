using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Nail : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Material> materials = new List<Material>();
    public GameObject fruitAppearVfx;
    public GameObject immatureFruit; // 预制体
    public GameObject matureFruit; // 预制体
    public enum NailType
    {
        OnlyVines,
        immatureFruit,
        matureFruit,
    }
    public NailType nailType = NailType.OnlyVines;
    void Awake()
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
    void OnEnable()
    {
        //订阅事件
        VinesGrowUp();
    }
    void VinesGrowUp()
    {
        //遍历所有材质球
        foreach (Material mat in materials)
        {
            //设置材质球的GrowUp属性
            mat.DOFloat(20f, "_grow", 8f);
        }
    }
    void FruitGrowUp()
    {
        //遍历所有材质球
        GameObject vfx = Instantiate(fruitAppearVfx, immatureFruit.transform.position, Quaternion.identity);
        vfx.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        immatureFruit.SetActive(true);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPhysicalStrength.Instance.stopRecovering();
            //取消订阅事件
            if(nailType == NailType.OnlyVines)
            {
                FruitGrowUp(); // 触发藤蔓生长
                nailType = NailType.immatureFruit; // 修改钉子类型
            }

        }
        
    }
    void MatureFruitGrowUp()
    {
        //遍历所有材质球
        matureFruit.SetActive(true);
    }

    // Update is called once per frame
}
