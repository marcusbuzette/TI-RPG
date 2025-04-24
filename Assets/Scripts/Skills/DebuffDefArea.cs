using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DEbuffDefArea : BaseSkills {
    [SerializeField] private List<Unit> targetsList = new List<Unit>();
    [SerializeField] private int maxIntimidateDistance = 1;
    [SerializeField] private int debufDefenceAmount = 1;
    [SerializeField] private BuffType buffType = BuffType.DEFENCE;


    public override void Action() {
        // AudioManager.instance?.PlaySFX("Intimidar");
        foreach (Unit target in targetsList) {
            target.GetModifiers().Debuff(buffType, debufDefenceAmount);
        }
        ActionFinish();
        ActiveCoolDown();
    }

    public override string GetActionName() {
        return "Area de fraquesa";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        if (targetsList != null) {
            targetsList.Clear();
        }
        GridPosition unitGridPosition = unit.GetGridPosition();
        List<GridPosition> affectedPositions = new List<GridPosition>();
        affectedPositions.Add(unitGridPosition);
        int i = 0;
        for (int x = -maxIntimidateDistance; x <= maxIntimidateDistance; x++) {
            for (int z = -maxIntimidateDistance; z <= maxIntimidateDistance; z++) {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

               if ((Mathf.Abs(x) > maxIntimidateDistance) || ( Mathf.Abs(z) > maxIntimidateDistance)) {
                    continue;
                }

                affectedPositions.Add(testGridPosition);
                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) != null) {
                    if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsEnemy()) {
                        targetsList.Add(LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition));
                        i++;
                    }
                }
            }
        }

        return affectedPositions;
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
            onEndEffect.Invoke(this, EventArgs.Empty);
            onCoolDown = false;
        }
    }

    public override bool GetOnCooldown() { return onCoolDown; }

    public int GetMaxIntimidateDistance() { return maxIntimidateDistance; }

    public List<Unit> GetTargetList() { return targetsList; }

    public override BuffType? GetBuffType() {return this.buffType;}
}