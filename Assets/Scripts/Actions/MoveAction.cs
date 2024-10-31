using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveAction : BaseAction {

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private float stopDistance = .1f;

    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;


    protected override void Awake() {
        base.Awake();
        targetPosition = transform.position;
        this.actionType = ActionType.MOVE;
    }

    public override void Action() {
        if (Vector3.Distance(targetPosition, transform.position) > stopDistance) {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        } else {
            ActionFinish();
        }
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(mouseGridPosition);
        ActionStart(onActionComplete);
    }

    public override string GetActionName() {
        return "Mover";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                if (unitGridPosition == testGridPosition) {
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        Debug.Log(unit);
        Debug.Log(gridPosition);
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
