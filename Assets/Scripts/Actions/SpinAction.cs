using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpinAction : BaseAction {


    private float totalSpinAmmount = 0;
    [SerializeField] private float MAX_SPIN = 360f;


    public override void Action() {
        float spinAddAmmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmmount, 0);
        totalSpinAmmount += spinAddAmmount;
        if (totalSpinAmmount > MAX_SPIN) {
            totalSpinAmmount = 0;
            ActionFinish();
        }
    }

    public override string GetActionName() {
        return "Girar";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> {
            unitGridPosition
        };
    }


    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        ActionStart(onActionComplete);
    }
}
