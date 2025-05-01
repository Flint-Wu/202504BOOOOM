using System;
using System.Collections;
using System.Collections.Generic;
using DiasGames.Climbing;
using UnityEngine;
using UnityEngine.Events;
namespace DiasGames.Abilities
{
public class InteractionManger : AbstractAbility
{
    // Start is called before the first frame update
    [Header("攀爬点的预制体")]
    public GameObject BuildingPrefabs;
    [Header("需要放置的建筑物")]
    public GameObject CurrentBuildingPrefab;
    public AbilityScheduler scheduler;
    public LayerMask NotBuildingLayerMask; // Layer mask for the building layer
    public bool isBuilding = false; // Flag to check if the building is being placed
    public BoxCollider InteractZone; // 交互范围
    public GameObject currentInteractableObject; //当前可互动的物体
    void Awake()
    {
        scheduler = GetComponent<AbilityScheduler>();
                // 手动设置 action 引用
        SetActionReference(ref scheduler.characterActions);
    }

        // Update is called once per frame
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set the gizmo color to red
        Gizmos.DrawWireCube(InteractZone.transform.position, InteractZone.size); // Draw a wireframe cube to represent the interaction zone
        //文字注释DectZone
        Gizmos.color = Color.white; // Set the gizmo color to white

    }
    void Update()
    {
        //检测InteractDistance所有挂载CanBeInteract脚本的物体
        Collider[] colliders = Physics.OverlapBox(InteractZone.transform.position, InteractZone.size, Quaternion.identity); // Get all colliders in the interaction zone

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<CanBeInteract>() != null)
            {
                if (colliders[i].GetComponent<CanBeInteract>().BeInteract) continue; // Skip if the object is not interactable
                currentInteractableObject = colliders[i].gameObject; //获取当前可互动的物体
                Debug.Log("当前可互动的物体: " + currentInteractableObject.name); // Log the name of the interactable object
                break; // Exit the loop after finding the first interactable object
            }
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            InventoryManager.Instance.CostNail(); // Add the building prefab to the inventory
        }
        if (currentInteractableObject == null) 
        {
            Annotation.Instance.Reset(); // Reset the annotation if no interactable object is found
            return; // Exit if no interactable object is found
        }
        else
        {
            if (currentInteractableObject.GetComponent<GrowUpController>() != null)
            {
                if(currentInteractableObject.GetComponent<GrowUpController>().CanBeUsed)
                {
                    Annotation.Instance.AnnotationRecoverOnTree(); // Show the annotation for the interactable object
                }
                else
                {
                    Annotation.Instance.AnnotationPourWater(); // Show the annotation for the interactable object
                }
            }
            else if(currentInteractableObject.GetComponentInChildren<WaterBottleFruit>() != null)
            {
                Annotation.Instance.AnnotationFruit(); // Show the annotation for the interactable object
            }
        }


        if(_action.interact)
        {
            if(currentInteractableObject == null) return; // Exit if no interactable object is found
            

            if(currentInteractableObject.GetComponent<GrowUpController>() != null)
            {
                if(currentInteractableObject.GetComponent<GrowUpController>().CanBeUsed)
                {
                    currentInteractableObject.GetComponent<GrowUpController>().RecoverAllPhysicalStrength(); // Call the Interact method on the interactable object
                    currentInteractableObject.GetComponent<CanBeInteract>().BeInteract = true; // Set the object to not interactable
                    currentInteractableObject = null; // Reset the current interactable object
                }
                else
                {
                    currentInteractableObject.GetComponent<GrowUpController>().PourWater(); // Call the Interact method on the interactable object
                    currentInteractableObject.GetComponent<CanBeInteract>().BeInteract = true; // Set the object to not interactable}
                    currentInteractableObject = null; // Reset the current interactable object
                }
            }
            else if(currentInteractableObject.GetComponentInChildren<WaterBottleFruit>() != null)
            {
                currentInteractableObject.GetComponentInChildren<WaterBottleFruit>().GetFruit(); // Call the Interact method on the interactable object
                currentInteractableObject.GetComponent<CanBeInteract>().BeInteract = true; // Set the object to not interactable
                currentInteractableObject = null; // Reset the current interactable object
            }
            else
            {
                Debug.LogError("当前可互动的物体: " + currentInteractableObject.name+ " 不能互动"); // Log the name of the interactable object

            }

            //UseNail?.Invoke(); // Invoke the Interact event
            // {
            //     StartBuilding(); // Start building when interact is pressed
            // }
        }


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
    void StartBuilding()
    {
        isBuilding = true;
        if(CurrentBuildingPrefab != null)
        {
            CurrentBuildingPrefab.SetActive(isBuilding); // Show or hide the building prefab
        }
    }

    void StopBuilding()
    {
        isBuilding = false;
        if(CurrentBuildingPrefab != null)
        {
            CurrentBuildingPrefab.SetActive(false); // Hide the building prefab
        }
    }
}
}