using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReviveItemAction : BaseAction {
    [SerializeField] public int healAmount = 20;
    private HealthSystem healthSystem;
    private string itemName = "Revive";

    Unit targetUnit;

    public override void Action() {
        Use();
    }

    public override string GetActionName() {
        return "Revive";
    }

    private void Use() {
        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE) {
            if (InventorySystem.inventorySystem != null &&
                    InventorySystem.inventorySystem.HasItemNamed(itemName) && healthSystem != null) {
                InventoryItemData ReviveItem = InventorySystem.inventorySystem.GetInvontoryItemNamed(itemName);
                InventorySystem.inventorySystem.Remove(ReviveItem);
                healthSystem.Revive(healAmount);
                Debug.Log("Used revive item");
            }
            else {
                Debug.LogWarning("HealthSystem or InventorySystem missing!");
            }
        }
        else {
            Debug.Log("Este item não pode ser usado em batalha");
        }
        ActionFinish();
    }

    public override List<GridPosition> GetValidGridPositionList() {
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

                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).GetHealthSystem().GetHealthState() == HealthSystem.HealthState.ALIVE) {
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

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        this.actionType = ActionType.ITEM;
        this.targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        this.healthSystem = targetUnit.GetComponent<HealthSystem>();

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
    public int GetUseDistance() { return 1; }
    public override void IsAnotherRound() { }
}