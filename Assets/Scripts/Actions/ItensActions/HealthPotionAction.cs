using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPotionAction : BaseAction {
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
        
        if (!LevelGrid.Instance.IsInBattleMode()) {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGrid = UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition();

            for (int x = -1; x <= 1; x++) {
                for (int z = -1; z <= 1; z++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                    GridPosition testGridPosition = unitGrid + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        continue;
                    }


                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                        continue;
                    }

                    if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).GetHealthSystem().GetHealthState() == HealthSystem.HealthState.FAINT) {
                        continue;
                    }

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    if (targetUnit.IsEnemy()) {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }

            return validGridPositionList;
        }

        unit = UnitActionSystem.Instance.GetSelectedUnit();
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        this.actionType = ActionType.ITEM;
        this.unit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        this.healthSystem = unit.GetComponent<HealthSystem>();
        ActionStart(onActionComplete);
        AudioManager.instance?.PlaySFX("Potion");
        Action();
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

}