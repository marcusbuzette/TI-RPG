using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inspire : BaseSkills {
    [SerializeField] private List<Unit> targetsList = new List<Unit>();
    [SerializeField] private int maxInspireDistance = 1;
    [SerializeField] private int buffAttackAmount = 2;
    [SerializeField] private BuffType buffType = BuffType.ATTACK;
    public string inspireSFX;

    public override void Action() {

        
        foreach (Unit target in targetsList) {
            target.GetModifiers().Buff(buffType, buffAttackAmount, target, this);
        }

        ActionFinish();
        ActiveCoolDown();

    }

    public override string GetActionName() {
        return "Insprar";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        if (targetsList != null) {
            targetsList.Clear();
        }
        GridPosition unitGridPosition = unit.GetGridPosition();
        List<GridPosition> affectedPositions = new List<GridPosition>();
        affectedPositions.Add(unitGridPosition);
        int i = 0;
        for (int x = -maxInspireDistance; x <= maxInspireDistance; x++) {
            for (int z = -maxInspireDistance; z <= maxInspireDistance; z++) {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                if ((Mathf.Abs(x) > maxInspireDistance) || ( Mathf.Abs(z) > maxInspireDistance)) {
                    continue;
                }

                affectedPositions.Add(testGridPosition);
                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) != null) {
                    if (!LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsEnemy()) {
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
        if (!string.IsNullOrEmpty(inspireSFX)) {
            AudioManager.instance?.PlaySFX(inspireSFX);  // vai tocar o sfx q ta no inspector da skill favor n mudar nada sem avisar
        }
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
        if (currentCoolDown == 0 && onCoolDown) {
            onEndEffect.Invoke(this, EventArgs.Empty);
            onCoolDown = false;
        }
    }

    public override bool GetOnCooldown() { return onCoolDown; }

    public int GetMaxInspireDistance() { return maxInspireDistance; }

    public List<Unit> GetTargetList() { return targetsList; }

    public override BuffType? GetBuffType() { return this.buffType; }

}
