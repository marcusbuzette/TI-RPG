using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GridSystemVisual;

public class HealArea : BaseSkills {
    [SerializeField] private LayerMask obstaclesLayerMask;
    private GameObject healAreaObject;
    [SerializeField] private GameObject particleHeal;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private int areaHeal = 3;
    [SerializeField] private int healPoints = 3;

    private bool canShoot;
    public Vector3 selectedGrid;
    public bool isAiming = false;
    public string healArrowSFX;
    GridPosition mouseGridPosition;

    private void Start() {
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); //add layer mask to don't shoot through obstacles
    }
    public override string GetActionName() {
        return "Flecha cura em area";
    }

    private void Update() {
        if (isAiming) {
            if (UnitActionSystem.Instance.GetSelectedAction() != this) return;
            mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            ViewAreaHeal(mouseGridPosition);
        }
        if (!isActive) return;
        Action();
    }

    public override void Action() {
        ActiveCoolDown();

        if (canShoot) {
            canShoot = false;
            GridSystemVisual.Instance.HideAllGridPosition();
            StartCoroutine(RotateTowardsAndExecute(selectedGrid, () => {
                Shoot();
            }));
        }
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidGridPositionList(GridPosition unitGridPosition) {
        isAiming = true;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance) {
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

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        isAiming = false;
        selectedGrid = LevelGrid.Instance.GetWorldPosition(mouseGridPosition);
        canShoot = true;
        if (!string.IsNullOrEmpty(healArrowSFX)) {
            AudioManager.instance?.PlaySFX(healArrowSFX);
        }
        ActionStart(onActionComplete);
    }

    private void Shoot() {
        unit.SpawnProjectile(selectedGrid, Color.green);

        healAreaObject = Instantiate(new GameObject(), selectedGrid, Quaternion.identity);
        healAreaObject.AddComponent<HealAreaObject>().SetHealAreaObject(this, particleHeal, healPoints, areaHeal, coolDown);
        ActionFinish();
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        if (!targetUnit.GetEnemyFocus()) {
            return new EnemyAIAction {
                gridPosition = gridPosition,
                actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
            };
        }
        else {
            return new EnemyAIAction {
                gridPosition = gridPosition,
                actionValue = 1000 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
            };
        }
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) {
        return GetValidGridPositionList(gridPosition).Count;
    }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() {
        if (currentCoolDown != 0) {
            currentCoolDown--;
        }
        if (currentCoolDown == 0) {
            onCoolDown = false;
        }
    }

    public void ViewAreaHeal(GridPosition mousePosition) {
        GridSystemVisual.Instance.UpdateGridVisual();
        List<GridPosition> validGridPositionList = GetValidGridPositionList();
        List<GridPosition> attackGridPositionList = new List<GridPosition>();
        Vector3 mouseWorldPos = LevelGrid.Instance.GetWorldPosition(mousePosition);

        if (validGridPositionList.Contains(mousePosition)) {
            for (int x = -areaHeal; x <= areaHeal; x++) {
                for (int z = -areaHeal; z <= areaHeal; z++) {
                    GridPosition testGridPosition = mousePosition + new GridPosition(x, z, 0);

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > areaHeal) {
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
            }
        }

        GridSystemVisual.Instance.ShowGridPositionList(attackGridPositionList, GridVisualType.Green);
    }
}