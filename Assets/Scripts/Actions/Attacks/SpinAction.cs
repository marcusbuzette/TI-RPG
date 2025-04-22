using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpinAction : BaseAction {
    [SerializeField] private List<Unit> targetsList = new List<Unit>();
    [SerializeField] private int maxAttackDistance = 1;
    [SerializeField] private int attackAttenuation = 2;
    [SerializeField] private int hitDamage = 30;
    private float totalSpinAmmount = 0;
    [SerializeField] private float MAX_SPIN = 360f;


    public override void Action() {
        float spinAddAmmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmmount, 0);
        totalSpinAmmount += spinAddAmmount;
        if (totalSpinAmmount > MAX_SPIN) {
            Debug.Log(GetComponent<Unit>().GetUnitStats().GetAttack() - attackAttenuation);
            foreach (Unit target in targetsList) {
            target.Damage(this.hitDamage, this.GetComponent<Unit>());
        }
            totalSpinAmmount = 0;
            ActionFinish();
        }
    }

    public override string GetActionName() {
        return "Tornado de Aço";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        if (targetsList != null) {
            targetsList.Clear();
        }
        GridPosition unitGridPosition = unit.GetGridPosition();
        List<GridPosition> possibleAttackPositions = new List<GridPosition>();;
        int i = 0;
        for (int x = -maxAttackDistance; x <= maxAttackDistance; x++) {
            for (int z = -maxAttackDistance; z <= maxAttackDistance; z++) {
                GridPosition testGridPosition = unitGridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }
                possibleAttackPositions.Add(testGridPosition);

                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) != null) {
                    if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsEnemy()) {
                        targetsList.Add(LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition));
                        i++;
                    }
                }
            }
        }

        return possibleAttackPositions;
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

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }
}
