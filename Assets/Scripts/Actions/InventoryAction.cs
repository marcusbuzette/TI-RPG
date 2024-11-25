using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAction : BaseAction {

    protected override void Awake() {
        base.Awake();
        this.actionType = ActionType.INVENTORY;
    }


    public override void Action() {}

    public override string GetActionName() {
        return "Bolsa";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() {}

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {}
}
