using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveAction : BaseAction {

    public PathArrowMesh currentArrow;

    [SerializeField] private float moveSpeed = 4f;
    private float rotateSpeed = 4f;
    [SerializeField] private float stopDistance = .05f;

    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float exploreSpeed = 5.5f;

    private List<Vector3> positionList;
    private int currentPositionIndex;
    private int startZone = 0;
    private bool hasStartZone = false;

    public EventHandler OnFinishedWalking;

    private bool inMouseEvent = false;

    List<GridPosition> gridList = new List<GridPosition>();

    protected override void Awake() {
        base.Awake();
        this.actionType = ActionType.MOVE;
    }

    private void Start() {
        this.maxMoveDistance = GetComponent<Unit>().GetUnitStats().GetMaxMove(unit);
        if (currentArrow == null) currentArrow = PathFinding.Instance.pathArrow.GetComponent<PathArrowMesh>();
    }

    private void Update() {
        if (!unit.IsEnemy() && !unit.GetHasMoved() && !isActive) {
            if (UnitActionSystem.Instance.GetSelectedAction() == this) {
                if (TurnSystem.Instance.GetTurnUnit() == unit && LevelGrid.Instance.IsInBattleMode()) {
                    if (!inMouseEvent) {
                        inMouseEvent = true;
                        GridSystemVisual.Instance.OnMouseChangeGridPosition += UpdateArrowPath;
                    }
                }
            }
            else if (inMouseEvent) {
                inMouseEvent = false;
                currentArrow.gameObject.SetActive(false);
                GridSystemVisual.Instance.OnMouseChangeGridPosition -= UpdateArrowPath;
            }
        }
        else if (inMouseEvent) {
            inMouseEvent = false;
            currentArrow.gameObject.SetActive(false);
            GridSystemVisual.Instance.OnMouseChangeGridPosition -= UpdateArrowPath;
        }

        if (!isActive) {
            return;
        }

        Action();
    }

    public override void Action() {
        if (!isActive || positionList == null || positionList.Count == 0) return;

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float currentSpeed = moveSpeed;

        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE) {
            currentSpeed = this.exploreSpeed; // exemplo: velocidade normalizada fixa para exploração
            if (this.unit.unitId == "hero") {
                currentSpeed += 0.5f;
            } 
        }

        // Move suavemente
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );

        // Rotaciona suavemente em direção ao alvo
        if (moveDirection != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }

        // Verifica se chegou no ponto atual
        float reachedDistance = 0.05f;
        if (Vector3.Distance(transform.position, targetPosition) < reachedDistance) {
            currentPositionIndex++;

            if (currentPositionIndex >= positionList.Count) {
                transform.position = targetPosition; // Garante precisão
                unit.EndAnimation("IsWalking", true);
                ActionFinish();
                OnFinishedWalking?.Invoke(this, EventArgs.Empty);

                if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE &&
                    startZone != unit.GetGridPosition().zone) {
                    LevelGrid.Instance.BattleMode(unit.GetGridPosition().zone);
                }
            }
        }
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        if (currentArrow.gameObject.activeSelf) currentArrow.gameObject.SetActive(false);
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

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                    if (unitGridPosition == testGridPosition) continue;
                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;
                    if (!PathFinding.Instance.IsWalkableGridPosition(testGridPosition)) continue;
                    if (!PathFinding.Instance.HasPath(unitGridPosition, testGridPosition)) continue;

                    int pathFindingDistanceMultiplier = 10;
                    if (PathFinding.Instance.GetPathLenght(unitGridPosition, testGridPosition) > maxMoveDistance * pathFindingDistanceMultiplier) {
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

        if ((unit.GetHealthPoints() * 100) / unit.GetHealthSystem().maxHealthPoints < 15 &&
            unit.GetComponent<HealAction>()) {
            valueGridPosition = unit.GetComponent<HealAction>().GetEnemyAIAction(gridPosition).actionValue;
            return new EnemyAIAction {
                gridPosition = gridPosition,
                actionValue = valueGridPosition * 10,
            };
        }

        List<BaseAction> actions = unit.GetActionsArray().ToList();
        List<BaseAction> attackActions = new List<BaseAction>();

        for (int i = 0; i < actions.Count; i++) {
            if (actions[i].GetActionType() != ActionType.MOVE) {
                if (!unit.GetComponent<HealAction>() || actions[i] != unit.GetComponent<HealAction>()) {
                    attackActions.Add(actions[i]);
                }
            }
        }

        if (attackActions.Count > 0) {
            valueGridPosition = attackActions[0].GetEnemyAIAction(gridPosition).actionValue;
        }

        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = valueGridPosition * 10,
        };
    }

    public void SetMaxMoveDistance(int maxDistance) {
        this.maxMoveDistance = maxDistance;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

    public float GetMovementSpeed() => moveSpeed;

    public int GetMaxDistanceMovement() => maxMoveDistance;

    public void SetMovementSpeed(float moveSpeed) => this.moveSpeed = moveSpeed;

    public void SetMaxDistanceMovement(int maxDistanceMovement) => this.maxMoveDistance = maxDistanceMovement;

    public List<Vector3> GetMovePathList() => this.positionList;

    private void DestroyPathArrow() {
        Destroy(currentArrow);
    }

    public void UpdateArrowPath(object sender, EventArgs e) {
        currentArrow.gameObject.SetActive(true);
        gridList = PathFinding.Instance.FindPath(unit.GetGridPosition(), LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()), out int pathLenght);
        currentArrow.DrawPath(gridList);
    }
}
