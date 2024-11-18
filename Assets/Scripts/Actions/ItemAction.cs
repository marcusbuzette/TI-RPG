using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemAction : BaseAction {
    [SerializeField] public int potionHealAmount = 20;
    private HealthSystem healthSystem;
    private string itemName = "Potion";

    public override void Action() {
        if (InventorySystem.inventorySystem != null &&
                InventorySystem.inventorySystem.HasItemNamed(itemName) && healthSystem != null) {
            InventoryItemData healthPotion = InventorySystem.inventorySystem.GetInvontoryItemNamed(itemName);
            InventorySystem.inventorySystem.Remove(healthPotion);
            healthSystem.Heal(potionHealAmount);
            Debug.Log("Used health potion");
        }
        else {
            Debug.LogWarning("HealthSystem or InventorySystem missing!");
        }
        ActionFinish();

    }

    public override string GetActionName() {
        return "Poção";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        unit = TurnSystem.Instance.GetTurnUnit();
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        this.actionType = ActionType.ITEM;
        this.unit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        this.healthSystem = unit.GetComponent<HealthSystem>();
        ActionStart(onActionComplete);
        Action();
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
}