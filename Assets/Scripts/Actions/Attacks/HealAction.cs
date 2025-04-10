using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class HealAction : BaseAction {
    [SerializeField] private List<Unit> targetsList = new List<Unit>();
    [SerializeField] private int maxHealDistance = 1;
    [SerializeField] private int healPoints = 10;

    public override void Action() {
        foreach (Unit target in targetsList) {
            target.GetHealthSystem().Heal(healPoints);
        }

        ActionFinish();
    }

    public override string GetActionName() {
        return "Cura em area";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        if (targetsList != null) {
            targetsList.Clear();
        }
        GridPosition unitGridPosition = unit.GetGridPosition();
        int i = 0;
        for (int x = -maxHealDistance; x <= maxHealDistance; x++) {
            for (int z = -maxHealDistance; z <= maxHealDistance; z++) {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxHealDistance) {
                    continue;
                }

                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) != null) {
                    if (!LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsEnemy()) {
                        targetsList.Add(LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition));
                        i++;
                    }
                }
            }
        }

        return new List<GridPosition> {
            unitGridPosition
        };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public int GetMaxHealDistance() { return maxHealDistance; }

    public List<Unit> GetTargetList() { return targetsList; }

    public override bool GetOnCooldown() { return false;}

    public override void IsAnotherRound() {}
}
