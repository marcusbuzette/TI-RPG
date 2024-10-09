using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { MOVE, ACTION };

public abstract class BaseAction : MonoBehaviour {

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;
    protected ActionType actionType;

    protected virtual void Awake() {
        unit = GetComponent<Unit>();
        actionType = ActionType.ACTION;
    }

    protected void Update() {
        if (!isActive) return;
        Action();

    }

    public abstract void Action();
    public abstract string GetActionName();

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition) {
        List<GridPosition> validGridPositionList = GetValidGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidGridPositionList();

    public abstract void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete);

    public ActionType GetActionType() {
        return actionType;
    }
}
