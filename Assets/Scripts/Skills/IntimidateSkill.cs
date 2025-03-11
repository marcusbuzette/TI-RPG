using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class IntimidateSkill : BaseSkills {
    [SerializeField] private List<Unit> targetsList = new List<Unit>();
    [SerializeField] private int maxIntimidateDistance = 1;

    public override void Action() {
        foreach (Unit target in targetsList) {
            target.BeIntimidate();
        }
        AudioManager.instance?.PlaySFX("Intimidar");
        ActionFinish();
        ActiveCoolDown();
    }

    public override string GetActionName() {
        return "Intimidar";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        if(targetsList != null) {
            targetsList.Clear();
        }
        GridPosition unitGridPosition = unit.GetGridPosition();
        int i = 0;
        for (int x = -maxIntimidateDistance; x <= maxIntimidateDistance; x++) {
            for (int z = -maxIntimidateDistance; z <= maxIntimidateDistance; z++) {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxIntimidateDistance) {
                    continue;
                }

                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) != null) {
                    if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsEnemy()) {
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

    public override void IsAnotherRound() {
        if (currentCoolDown != 0) {
            currentCoolDown--;
        }
        if (currentCoolDown == 0) {
            onCoolDown = false;
        }
    }

    public override bool GetOnCooldown() { return onCoolDown; }

    public int GetMaxIntimidateDistance() { return maxIntimidateDistance; }

    public List<Unit> GetTargetList() { return targetsList; }
}
