using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitAction : BaseAction
{
    private float totalSpinAmmount = 0;
    private int maxShootDistance = 1;
    [SerializeField] private float MAX_SPIN = 360f;

    public override string GetActionName()
    {
        return "Hit";
    }

      public override void Action() {
        float spinAddAmmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmmount, 0);
        totalSpinAmmount += spinAddAmmount;
        if (totalSpinAmmount > MAX_SPIN) {
            isActive = false;
            totalSpinAmmount = 0;
            onActionComplete();
        }
    }

 public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }


                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if(targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

 public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        this.onActionComplete = onActionComplete;
        isActive = true;
    }

}
