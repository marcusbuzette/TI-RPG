using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemAction : BaseAction
{
    [SerializeField] public InventoryItemData healthPotion;   
    [SerializeField] private InventorySystem inventorySystem;  
    [SerializeField] public int potionHealAmount = 20;        
    private HealthSystem healthSystem;                         

    public void Start()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();   

        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem not found!");
        }

      
        healthSystem = GetComponent<HealthSystem>();

        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem not found on the unit!");
        }
    }

    public override void Action()
    {
        if (inventorySystem != null && inventorySystem.HasItem(healthPotion) && healthSystem != null)
        {
           
            inventorySystem.Remove(healthPotion);

          
            healthSystem.Heal(potionHealAmount);
            Debug.Log("Used health potion");

           
            ActionFinish();
        }
        else
        {
            Debug.LogWarning("HealthSystem or InventorySystem missing!");
        }
    }

    public override string GetActionName()
    {
        return "Poção";
    }

    public override List<GridPosition> GetValidGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
    }
}