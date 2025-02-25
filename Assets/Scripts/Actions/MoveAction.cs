using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class MoveAction : BaseAction {

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private float stopDistance = .1f;

    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    public Animator animator;
    bool isWalking = false;
    

    protected override void Awake() {
        base.Awake();
        this.actionType = ActionType.MOVE;
        animator = GetComponentInChildren<Animator>();
    }

    public override void Action() {
        Vector3 targetPosition = positionList[currentPositionIndex];

        if (Vector3.Distance(targetPosition, transform.position) > stopDistance) {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            animator.SetBool("isWalking", true);
        } else {
            currentPositionIndex++;
            if(currentPositionIndex >= positionList.Count) {
                ActionFinish();
                animator.SetBool("isWalking", false);
            }
        }
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        List<GridPosition> pathGridPositionList = PathFinding.Instance.FindPath(unit.GetGridPosition(), mouseGridPosition, out int pathLenght);
        
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach(GridPosition pathGridPosition in pathGridPositionList) {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

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

                if (!PathFinding.Instance.IsWalkableGridPosition(testGridPosition)) {
                    continue;
                }

                if (!PathFinding.Instance.HasPath(unitGridPosition, testGridPosition)) {
                    continue;
                }

                int pathFindingDistanceMultiplier = 10;
                if (PathFinding.Instance.GetPathLenght(unitGridPosition, testGridPosition) > maxMoveDistance * pathFindingDistanceMultiplier) {
                    //Path Lenght is to low
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        // Debug.Log(unit);
        // Debug.Log(gridPosition);
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }
}
