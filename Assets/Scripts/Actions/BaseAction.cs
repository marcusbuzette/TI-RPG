using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType { MOVE, ACTION, INVENTORY, ITEM, SKILL };

public abstract class BaseAction : MonoBehaviour {

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;
    protected ActionType actionType;
    [SerializeField] protected Sprite actionImage;
    [SerializeField] protected Sprite actionImageBlocked;
    public Animator animator;
    public float speed;
    [SerializeField] private float rotSpeed = 180f;


    protected virtual void Awake() {
        unit = GetComponent<Unit>();
        actionType = ActionType.ACTION;
        animator = GetComponentInChildren<Animator>();
        rotSpeed = 180f;
    }

    protected virtual void Update() {
        if (!isActive) return;
        Action();
    }

    public void SetAnimationSpeed()
    {
        speed = 1f;
        animator.SetFloat("SpeedMultiplier", speed);
    }

    public abstract void Action();
    public abstract string GetActionName();
    public Sprite GetActionImage() {return this.actionImage;}
    public Sprite GetActionBlockedImage() {return this.actionImageBlocked;}

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
        if(LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE) {
            GridSystemVisual.Instance.HideAllGridPosition();
        }

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

    protected IEnumerator RotateTowardsAndExecute(Transform target, System.Action onComplete) {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotSpeed * Time.deltaTime
            );
            yield return null;
        }

        onComplete?.Invoke();
    }

    protected IEnumerator RotateTowardsAndExecute(Vector3 target, System.Action onComplete) {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotSpeed * Time.deltaTime
            );
            yield return null;
        }

        onComplete?.Invoke();
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    public Unit GetUnit() {
        return unit;
    }

    public abstract bool GetOnCooldown();

    public abstract void IsAnotherRound();
}
