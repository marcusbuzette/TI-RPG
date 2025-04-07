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
    private float lastDistance = 0;
    private bool hastLastDistance = false;

    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;
    // private bool changedBattleZone = false;
    private int startZone = 0;



    protected override void Awake() {
        base.Awake();
        this.actionType = ActionType.MOVE;
    }

    private void Start() {
        this.maxMoveDistance = GetComponent<Unit>().GetUnitStats().GetMaxMove();
    }

    public override void Action() {
        Vector3 targetPosition = positionList[currentPositionIndex];
        if (Vector3.Distance(targetPosition, transform.position) > stopDistance &&
        (hastLastDistance == false || Vector3.Distance(targetPosition, transform.position) < lastDistance)) {
            this.hastLastDistance = true;
            this.lastDistance = Vector3.Distance(targetPosition, transform.position);
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            animator?.SetBool("IsWalking", true);
        }
        else {
            currentPositionIndex++;
            this.hastLastDistance = false;
            lastDistance = 0;
            if (currentPositionIndex >= positionList.Count) {
                transform.position = positionList[currentPositionIndex - 1];
                ActionFinish();
                animator?.SetBool("IsWalking", false);
            }

            if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE &&
                 startZone != unit.GetGridPosition().zone) {
                ActionFinish();
                animator?.SetBool("IsWalking", false);
                LevelGrid.Instance.BattleMode(unit.GetGridPosition().zone);
            }
        }
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        List<GridPosition> pathGridPositionList = PathFinding.Instance.FindPath(unit.GetGridPosition(), mouseGridPosition, out int pathLenght);

        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE) {
            if (positionList == null || positionList.Count < 1) {
                positionList = new List<Vector3>();
                currentPositionIndex = 0;
                this.startZone = unit.GetGridPosition().zone;

                foreach (GridPosition pathGridPosition in pathGridPositionList) {
                    positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
                }
            }
            else if (LevelGrid.Instance.GetWorldPosition(mouseGridPosition) != positionList[positionList.Count - 1]) {
                positionList = new List<Vector3>() { positionList[0] };
                currentPositionIndex = 1;
                this.startZone = unit.GetGridPosition().zone;

                foreach (GridPosition pathGridPosition in pathGridPositionList) {
                    positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
                }
            }
        }
        else if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE) {
            positionList = new List<Vector3>();
            currentPositionIndex = 0;
            this.startZone = unit.GetGridPosition().zone;
            foreach (GridPosition pathGridPosition in pathGridPositionList) {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
            }
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
                for (int floor = -maxMoveDistance; floor <= maxMoveDistance; floor++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
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
        }

        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        int targetCountAtGridPosition = unit.GetAction<TargetAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }

    public void SetMaxMoveDistance(int maxDistance) { this.maxMoveDistance = maxDistance; }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

    public float GetMovementSpeed() {
        return moveSpeed;
    }

    public int GetMaxDistanceMovement() {
        return maxMoveDistance;
    }

    public void SetMovementSpeed(float moveSpeed) {
        this.moveSpeed = moveSpeed;
    }

    public void SetMaxDistanceMovement(int maxDistanceMovement) {
        this.maxMoveDistance = maxDistanceMovement;
    }
}