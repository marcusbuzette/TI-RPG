using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GridSystemVisual;

public class SniperAction : BaseAction {
    private enum Direction {
        up,
        down,
        left,
        right
    }
    private Direction directionShoot;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private int hitDamage = 50;
    [SerializeField] private float rotateSpeed = 10f;
    public bool isAiming = false;

    [SerializeField] private LayerMask obstaclesLayerMask;

    GridPosition mouseGridPosition;
    public int Attack = 1;

    private SniperActionFieldOfView sniperActionFieldOfView;

    private void Start() {
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); //add layer mask to don't shoot through obstacles
    }

    private void Update() {
        if (isAiming) {
            if (UnitActionSystem.Instance.GetSelectedAction() != this) return;
            mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridSystemVisual.Instance.ShowGridPositionList(GetFieldOfView(mouseGridPosition, LevelGrid.Instance.GetGridPosition(transform.position)), GridVisualType.Red);
        }
        if (!isActive) return;
        Action();
    }

    public override string GetActionName() {
        return "Vigilia";
    }

    public override void Action() {
        if (Attack == 1) {
            // animator?.SetTrigger("Attack");
            unit.PlayAnimation("Attack");
            AudioManager.instance?.PlaySFX("Melee");
            Attack = 0;
        }
        StartCoroutine(DelayActionFinish());
    }

    private IEnumerator DelayActionFinish() {
        yield return new WaitForSeconds(0.5f); // Ajuste o tempo conforme necessário
        ActionFinish();
        Attack = 1;
    }

    public override List<GridPosition> GetValidGridPositionList() {
        {
            isAiming = true;
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            GridPosition unitGridPosition = unit.GetGridPosition();

            for (int x = -1; x <= 1; x++) {
                for (int z = -1; z <= 1; z++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > 1) {
                        continue;
                    }

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                        continue;
                    }

                    Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                    Vector3 shootDir = (LevelGrid.Instance.GetWorldPosition(testGridPosition) - unitWorldPosition).normalized;

                    float unitShoulderHeight = 1.7f;
                    if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDir,
                        Vector3.Distance(unitWorldPosition, LevelGrid.Instance.GetWorldPosition(testGridPosition)),
                        obstaclesLayerMask)) {
                        //Blocked by an Obstacle
                        continue;
                    }


                    validGridPositionList.Add(testGridPosition);
                }
            }

            return validGridPositionList;
        }
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        isAiming = false;

        List<GridPosition> fieldOfView = GetFieldOfView(mouseGridPosition, LevelGrid.Instance.GetGridPosition(transform.position));

        sniperActionFieldOfView = Instantiate(new GameObject("FieldOfView"), transform).AddComponent<SniperActionFieldOfView>();
        sniperActionFieldOfView.SetSniperFieldOfView(this, fieldOfView, hitDamage);

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public Unit GetTargetUnit() {
        return null;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() {
        sniperActionFieldOfView?.StopSniper();
    }

    public int GetDamage() {
        int damage = hitDamage;
        AudioManager.instance?.PlaySFX("DamageTaken");
        return damage;
    }

    public List<GridPosition> GetFieldOfView(GridPosition mousePosition, GridPosition unitPosition) {
        GridSystemVisual.Instance.UpdateGridVisual();
        List<GridPosition> validGridPositionList = GetValidGridPositionList();
        List<GridPosition> attackGridPositionList = new List<GridPosition>();
        Vector3 mouseWorldPos = LevelGrid.Instance.GetWorldPosition(mousePosition);

        int maxShootWidth = maxShootDistance + 2;

        if (mouseGridPosition.x == unitPosition.x) {
            if (mouseGridPosition.z > unitPosition.z) directionShoot = Direction.up;
            else directionShoot = Direction.down;
        }
        else {
            if (mouseGridPosition.x > unitPosition.x) directionShoot = Direction.right;
            else directionShoot = Direction.left;
        }

        switch (directionShoot) {
            case Direction.up:
                if (validGridPositionList.Contains(mousePosition)) {
                    for (int x = -maxShootDistance; x <= 0; x++) {
                        for (int z = -maxShootWidth; z <= maxShootWidth; z++) {
                            GridPosition testGridPosition = mousePosition + new GridPosition(z, x + maxShootDistance, 0);

                            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                                continue;
                            }

                            int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                            if (testDistance > maxShootDistance) {
                                continue;
                            }

                            Vector3 targetPos = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                            Vector3 shootDir = (targetPos - mouseWorldPos).normalized;

                            float unitShoulderHeight = 1.7f;
                            if (Physics.Raycast(mouseWorldPos + Vector3.up * unitShoulderHeight,
                                shootDir,
                                Vector3.Distance(mouseWorldPos, targetPos),
                                obstaclesLayerMask)) {

                                //Blocked by an Obstacle
                                continue;
                            }

                            attackGridPositionList.Add(testGridPosition);
                        }
                        maxShootWidth--;
                    }
                }
                break;
            case Direction.down:
                if (validGridPositionList.Contains(mousePosition)) {
                    for (int x = -maxShootDistance; x <= 0; x++) {
                        for (int z = -maxShootWidth; z <= maxShootWidth; z++) {
                            GridPosition testGridPosition = mousePosition + new GridPosition(z, -(x + maxShootDistance), 0);

                            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                                continue;
                            }

                            int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                            if (testDistance > maxShootDistance) {
                                continue;
                            }

                            Vector3 targetPos = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                            Vector3 shootDir = (targetPos - mouseWorldPos).normalized;

                            float unitShoulderHeight = 1.7f;
                            if (Physics.Raycast(mouseWorldPos + Vector3.up * unitShoulderHeight,
                                shootDir,
                                Vector3.Distance(mouseWorldPos, targetPos),
                                obstaclesLayerMask)) {

                                //Blocked by an Obstacle
                                continue;
                            }

                            attackGridPositionList.Add(testGridPosition);
                        }
                        maxShootWidth--;
                    }
                }
                break;
            case Direction.right:
                if (validGridPositionList.Contains(mousePosition)) {
                    for (int x = -maxShootDistance; x <= 0; x++) {
                        for (int z = -maxShootWidth; z <= maxShootWidth; z++) {
                            GridPosition testGridPosition = mousePosition + new GridPosition(x + maxShootDistance, z, 0);

                            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                                continue;
                            }

                            int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                            if (testDistance > maxShootDistance) {
                                continue;
                            }

                            Vector3 targetPos = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                            Vector3 shootDir = (targetPos - mouseWorldPos).normalized;

                            float unitShoulderHeight = 1.7f;
                            if (Physics.Raycast(mouseWorldPos + Vector3.up * unitShoulderHeight,
                                shootDir,
                                Vector3.Distance(mouseWorldPos, targetPos),
                                obstaclesLayerMask)) {

                                //Blocked by an Obstacle
                                continue;
                            }

                            attackGridPositionList.Add(testGridPosition);
                        }
                        maxShootWidth--;
                    }
                }
                break;
            case Direction.left:
                if (validGridPositionList.Contains(mousePosition)) {
                    for (int x = -maxShootDistance; x <= 0; x++) {
                        for (int z = -maxShootWidth; z <= maxShootWidth; z++) {
                            GridPosition testGridPosition = mousePosition + new GridPosition(-(x + maxShootDistance), z, 0);

                            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                                continue;
                            }

                            int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                            if (testDistance > maxShootDistance) {
                                continue;
                            }

                            Vector3 targetPos = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                            Vector3 shootDir = (targetPos - mouseWorldPos).normalized;

                            float unitShoulderHeight = 1.7f;
                            if (Physics.Raycast(mouseWorldPos + Vector3.up * unitShoulderHeight,
                                shootDir,
                                Vector3.Distance(mouseWorldPos, targetPos),
                                obstaclesLayerMask)) {

                                //Blocked by an Obstacle
                                continue;
                            }

                            attackGridPositionList.Add(testGridPosition);
                        }
                        maxShootWidth--;
                    }
                }
                break;
        }

        return attackGridPositionList;
    }

    public void SetFiewdOfViewNull() {
        sniperActionFieldOfView = null;
    }
}