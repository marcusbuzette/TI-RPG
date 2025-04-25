using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DefendAction : BaseAction {
    public int Attack = 1;

    private Unit targetUnit;


    public override string GetActionName() {
        return "Defender";
    }

    public override void Action() {
        if (Attack == 1) {
            // Add defending animation here
            Attack = 0;
        }
        StartCoroutine(DelayActionFinish());

    }

    private IEnumerator DelayActionFinish() {
        yield return new WaitForSeconds(0.5f); // Ajuste o tempo conforme necessário
        ActionFinish();
        Attack = 1;

    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>() {
            unitGridPosition
        };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        targetUnit.GetHealthSystem().SetDefenceMode(true);
        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public Unit GetTargetUnit() {
        return targetUnit;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() {
        if (this.targetUnit != null &&
            this.targetUnit.GetHealthSystem().GetDefenceMode()) {
            StopDefending();
        }
    }


    public void GetDamage() {
        AudioManager.instance?.PlaySFX("DamageTaken");
    }

    private void StopDefending() {
        this.targetUnit.GetHealthSystem().SetDefenceMode(false);
        // remove defending animation here
    }
}
