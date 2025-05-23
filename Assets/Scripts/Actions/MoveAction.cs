using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using static System.Collections.Specialized.BitVector32;

public class MoveAction : BaseAction {

    [SerializeField] private float moveSpeed = 4f;
    private float rotateSpeed = 4f;
    [SerializeField] private float stopDistance = .05f;
    private float lastDistance = 0;
    private bool hastLastDistance = false;
    private Vector3 moveDirControl = Vector3.zero;

    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;
    // private bool changedBattleZone = false;
    private int startZone = 0;
    private bool hasStartZone = false;



    protected override void Awake() {
        base.Awake();
        this.actionType = ActionType.MOVE;
    }

    private void Start() {
        this.maxMoveDistance = GetComponent<Unit>().GetUnitStats().GetMaxMove(unit);
    }

    public override void Action() {
        if (currentPositionIndex > positionList.Count - 1) return;
        Vector3 targetPosition = positionList[currentPositionIndex];
        if (Vector3.Distance(targetPosition, transform.position) > stopDistance &&
            (hastLastDistance == false || Vector3.Distance(targetPosition, transform.position) < lastDistance)) {
            this.hastLastDistance = true;
            this.lastDistance = Vector3.Distance(targetPosition, transform.position);
            Vector3 moveDirection = (targetPosition - transform.position).normalized;


            if (currentPositionIndex <= positionList.Count - 1) {
                moveDirControl = moveDirection;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                // animator?.SetBool("IsWalking", true);
                // unit.PlayAnimation("IsWalking", true);
            }

        }
        else {
            currentPositionIndex++;
            this.hastLastDistance = false;
            lastDistance = 0;
            moveDirControl = Vector3.zero;
            if (currentPositionIndex >= positionList.Count) {
                transform.position = positionList[currentPositionIndex - 1];
                ActionFinish();
                this.hasStartZone = false;
                unit.EndAnimation("IsWalking", true);
                // animator?.SetBool("IsWalking", false);
            }

            if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE &&
                 startZone != unit.GetGridPosition().zone) {
                ActionFinish();
                // animator?.SetBool("IsWalking", false);
                unit.EndAnimation("IsWalking", true);
                LevelGrid.Instance.BattleMode(unit.GetGridPosition().zone);
            }
        }
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        if (GetComponent<Unit>().GetGridPosition() == mouseGridPosition) return;
        List<GridPosition> pathGridPositionList = PathFinding.Instance.FindPath(unit.GetGridPosition(), mouseGridPosition, out int pathLenght);

        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE) {
            if (positionList == null || positionList.Count < 1) {
                positionList = new List<Vector3>();
                currentPositionIndex = 0;
                this.startZone = unit.GetGridPosition().zone;

                foreach (GridPosition pathGridPosition in pathGridPositionList) {
                    positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
                }
                unit.PlayAnimation("IsWalking", true);
                ActionStart(onActionComplete);
            }
            else if (LevelGrid.Instance.GetWorldPosition(mouseGridPosition) != positionList[positionList.Count - 1]) {
                positionList = new List<Vector3>() { positionList[0] };
                currentPositionIndex = 1;
                if (!this.hasStartZone) {
                    this.startZone = unit.GetGridPosition().zone;
                    this.hasStartZone = true;
                }
                if (pathGridPositionList.Count > 2) {
                    pathGridPositionList.RemoveAt(0);
                }

                foreach (GridPosition pathGridPosition in pathGridPositionList) {
                    positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
                }
                unit.PlayAnimation("IsWalking", true);
                ActionStart(onActionComplete);
            }
        }
        else if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE) {
            positionList = new List<Vector3>();
            currentPositionIndex = 0;
            this.startZone = unit.GetGridPosition().zone;
            foreach (GridPosition pathGridPosition in pathGridPositionList) {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
            }
            unit.PlayAnimation("IsWalking", true);
            ActionStart(onActionComplete);
        }
        // ActionStart(onActionComplete);
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
        int valueGridPosition = 0;

        //Verifica se o inimigo est� com pouca vida e se ele consegue se curar
        if ((unit.GetHealthPoints() * 100) / unit.GetHealthSystem().maxHealthPoints < 15 &&
            unit.GetComponent<HealAction>()) {
            valueGridPosition = unit.GetComponent<HealAction>().GetEnemyAIAction(gridPosition).actionValue;
            return new EnemyAIAction {
                gridPosition = gridPosition,
                actionValue = valueGridPosition * 10,
            };
        }

        //Pega as a��es do inimigo
        List<BaseAction> actions = unit.GetActionsArray().ToList();
        List<BaseAction> attackActions = new List<BaseAction>();

        //Tira a a��o de mover e de se curar da escolha de a��es
        for(int i = 0; i < actions.Count; i++) {
            if (actions[i].GetActionType() != ActionType.MOVE) {
                if (!unit.GetComponent<HealAction>() || actions[i] != unit.GetComponent<HealAction>()) {
                    attackActions.Add(actions[i]);
                }
            }
        }

        //Escolhe uma a��o aleat�ria para performar
        if(attackActions.Count > 0) {
            //valueGridPosition = attackActions[Random.Range(0, attackActions.Count)].GetEnemyAIAction(gridPosition).actionValue;
            valueGridPosition = attackActions[0].GetEnemyAIAction(gridPosition).actionValue;
        }

        //Retorna a melhor a��o possivel do inimigo
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = valueGridPosition * 10,
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

    public List<Vector3> GetMovePathList() {
        return this.positionList;
    }
}