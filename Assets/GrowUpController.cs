using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrowUpController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Material> materials = new List<Material>();
    [Range(0, 20)] public float GrowUp;
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
                }
            }
        }
        //遍历所有材质球
        
    }

    // Update is called once per frame
    void Update()
    {
        //遍历所有材质球
        foreach (Material mat in materials)
        {
            //设置材质球的GrowUp属性
            mat.SetFloat("_grow", GrowUp);
        }
    }
}
