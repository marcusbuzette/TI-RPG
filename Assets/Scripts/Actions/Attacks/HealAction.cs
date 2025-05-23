using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealAction : BaseAction {
    [SerializeField] private List<Unit> targetsList = new List<Unit>();
    [SerializeField] private int maxHealDistance = 1;
    [SerializeField] private int healPoints = 10;
    public string curaMacacoSFX;
    public override void Action() {
        if(unit.IsEnemy()) {
            unit.GetHealthSystem().Heal(healPoints);
        }

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

                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) != null) {
                    if (!LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsEnemy()) {
                        if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).GetHealthSystem().GetHealthState() == HealthSystem.HealthState.ALIVE) {
                            targetsList.Add(LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition));
                            i++;
                        }
                    }
                }
            }
        }

        return new List<GridPosition> {
            unitGridPosition
        };
    }

    private int GetDistanceNearestUnit(GridPosition gridPosition) {
        int closeDistance = int.MaxValue;

        //Separa os players da lista de unidades
        List<Unit> units = TurnSystem.Instance.GetTurnOrder();
        List<Unit> playerUnits = new List<Unit>();
        foreach (Unit unit in units) {
            if (!unit.IsEnemy()) {
                playerUnits.Add(unit);
            }
        }

        //Encontra a distancia do player mais proximo
        foreach (Unit playerUnit in playerUnits) {
            var dist = PathFinding.Instance.CalculateDistance(
                gridPosition, playerUnit.GetGridPosition());

            //salva a GridPosition do player mais proximo
            if (dist < closeDistance) {
                closeDistance = dist;
            }
        }

        return closeDistance;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        ActionStart(onActionComplete);
        if (!string.IsNullOrEmpty(curaMacacoSFX)){
            AudioManager.instance?.PlaySFX(curaMacacoSFX);
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        if ((unit.GetHealthPoints() * 100) / unit.GetHealthSystem().maxHealthPoints < 15) {
            return new EnemyAIAction {
                gridPosition = gridPosition,
                actionValue = 1000 + Mathf.RoundToInt((GetDistanceNearestUnit(gridPosition)) * 100f),
            };
        }
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = -1,
        };
    }

    public int GetMaxHealDistance() { return maxHealDistance; }

    public List<Unit> GetTargetList() { return targetsList; }

    public override bool GetOnCooldown() { return false;}

    public override void IsAnotherRound() {}
}
