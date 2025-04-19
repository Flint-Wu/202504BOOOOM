using System.Collections;
using System.Collections.Generic;
using DiasGames.Climbing;
using UnityEngine;
namespace DiasGames.Abilities
{
public class BuildingSystem : AbstractAbility
{
    // Start is called before the first frame update
    [Header("攀爬点的预制体")]
    public GameObject BuildingPrefabs;
    [Header("需要放置的建筑物")]
    public GameObject CurrentBuildingPrefab;
    public AbilityScheduler scheduler;
    public LayerMask NotBuildingLayerMask; // Layer mask for the building layer
    public bool isBuilding = false; // Flag to check if the building is being placed

    void Awake()
    {
        scheduler = GetComponent<AbilityScheduler>();
                // 手动设置 action 引用
        SetActionReference(ref scheduler.characterActions);
        CurrentBuildingPrefab = Instantiate(BuildingPrefabs, Vector3.zero, Quaternion.identity);
        //关闭建筑物的碰撞体
        CurrentBuildingPrefab.GetComponent<Collider>().enabled = false;
        CurrentBuildingPrefab.GetComponent<Ledge>().enabled = false;
        CurrentBuildingPrefab.layer = LayerMask.NameToLayer("BuildingShadow"); // Set the layer of the building prefab
        //设置材质球的颜色为透明
        CurrentBuildingPrefab.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f); // Set the color to transparent
        //NotBuildingLayerMask = LayerMask.GetMask("ClimbLayer");
    }

    // Update is called once per frame
    void Update()
    {
        if(_action.interact)
        {
            SwitchBuildingState(); // Toggle building state when interact is pressed
        }
        if(!isBuilding) return;
        PlaceBuildingPrefab();
    }
    public override bool ReadyToRun()
    {
        // Implement logic to determine if the ability is ready to run
        return true; // Default implementation, modify as needed
    }
    
    public override void OnStartAbility()
    {
        // Implement logic for when the ability starts
    }

    public override void UpdateAbility()
    {
        // Implement logic for updating the ability
    }
    void SwitchBuildingState()
    {
        isBuilding = !isBuilding;
        if(CurrentBuildingPrefab != null)
        {
            CurrentBuildingPrefab.SetActive(isBuilding); // Show or hide the building prefab
        }
    }
    public void PlaceBuildingPrefab()
    {
        //从屏幕中点发射射线(新输入系统)
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        //排除所有Climb Layer层的物体
        LayerMask buildingLayerMask = ~NotBuildingLayerMask;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingLayerMask))
        {
            //Debug.Log("Hit: " + hit.collider.name);
            //在射线碰撞点生成建筑物
            CurrentBuildingPrefab.transform.position = hit.point;
            //根据碰撞点的法线设置建筑物的旋转
            CurrentBuildingPrefab.transform.rotation = Quaternion.LookRotation(hit.normal);
            if(_action.fire)
            {
                // Instantiate the building prefab at the hit point
                if(InventoryManager.Instance != null)
                {
                    if(InventoryManager.Instance.CanBuild())
                    {
                        InventoryManager.Instance.CostNail(); // 扣除钉子数量
                    }
                    else
                    {
                        Debug.Log("没有足够的钉子！");
                        return;
                    }
                }
                GameObject building = Instantiate(BuildingPrefabs, hit.point, Quaternion.LookRotation(hit.normal));
                building.transform.rotation = Quaternion.LookRotation(hit.normal);
                // Optionally, you can set the parent of the building to the character or another object
                // building.transform.SetParent(transform); // Uncomment if needed
                _action.fire = false;
            }
        }
    }
}
}