using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QucikAttack : BaseSkills {
    private float totalSpinAmmount = 0;
    [SerializeField] private float MAX_SPIN = 360f;
    public string quickAttackSFX;

    public override void Action() {
        float spinAddAmmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmmount, 0);
        totalSpinAmmount += spinAddAmmount;
        if (totalSpinAmmount > MAX_SPIN) {
            totalSpinAmmount = 0;
            ActionFinish();
            ActiveCoolDown();
        }
    }

    public override string GetActionName() {
        return "Ataque Rapido";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> {
            unitGridPosition
        };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        if (!string.IsNullOrEmpty(quickAttackSFX)) {
            AudioManager.instance?.PlaySFX(quickAttackSFX);  // vai tocar o sfx q ta no inspector da skill favor n mudar nada sem avisar
        }
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
}
