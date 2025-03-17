using System;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { MOVE, ACTION, INVENTORY, ITEM, SKILL };

public abstract class BaseAction : MonoBehaviour {

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;
    protected ActionType actionType;
    public Animator animator;
    public float animationSpeed = 1f;

    public void SetAnimationSpeed(float speed)
    {
        if (animator != null)
        {
            animator.speed = speed;
        }
        else
        {
            Debug.LogWarning("Animator não encontrado!");
        }
    }

    protected virtual void Awake() {
        unit = GetComponent<Unit>();
        actionType = ActionType.ACTION;
        animator = GetComponentInChildren<Animator>();
        SetAnimationSpeed(animationSpeed);
    }

    protected virtual void Update() {
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

    protected void ActionStart(Action onActionComplete) {
        isActive = true;
        this.onActionComplete += onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionFinish() {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public EnemyAIAction GetBestEnemyAIAction() {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        List<GridPosition> validActionGridPositionList = GetValidGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList) {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0) {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        }
        else {
            //No possible Enemy AI Actions
            return null;
        }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    public Unit GetUnit() {
        return unit;
    }

    public abstract bool GetOnCooldown();

    public abstract void IsAnotherRound();
}
