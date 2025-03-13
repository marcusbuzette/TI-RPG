using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GridSystemVisual;
using UnityEngine.SocialPlatforms;

public class FireAttack : BaseAction {
    private enum State {
        Aiming, Shooting, Cooloff
    }
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private float aimingTimer = .1f;
    [SerializeField] private float shootingTimer = .3f;
    [SerializeField] private float cooloffTimer = .1f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private int shootDamage = 100;
    [SerializeField] private int areaDamage = 3;

    private State currentState;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShoot;
    public bool isAiming = false;

    GridPosition mouseGridPosition;

    private void Start() {
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); //add layer mask to don't shoot through obstacles
    }
    public override string GetActionName() {
        return "Fogo";
    }

    private void Update() {
        if(isAiming) {
            mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            ViewAreaDamage(mouseGridPosition);
        }
    }

    public override void Action() {
        stateTimer -= Time.deltaTime;
        switch (currentState) {
            case State.Aiming:
                Vector3 moveDirection = (targetUnit.transform.position - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShoot) {
                    AudioManager.instance?.PlaySFX("Arrows");
                    Shoot();
                    isAiming = false;
                    canShoot = false;
                }
                break;
            case State.Cooloff:

                break;

        }
        if (stateTimer <= 0) {
            NextState();
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
                /*Vector3 shootDir = (LevelGrid.Instance.GetWorldPosition(testGridPosition) - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask)) {d
                    //Blocked by an Obstacle
                    continue;
                }*/


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        isAiming = false;
        currentState = State.Aiming;
        stateTimer = aimingTimer;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        canShoot = true;

        ActionStart(onActionComplete);
    }

    private void NextState()
    {
        switch (currentState)
        {
            case State.Aiming:
                currentState = State.Shooting;
                stateTimer = shootingTimer;
                break;
            case State.Shooting:
                currentState = State.Cooloff;
                stateTimer = shootingTimer;
                break;
            case State.Cooloff:
                ActionFinish();
                break;

        }
    }

    private void Shoot() {
        targetUnit.Damage(shootDamage);
        animator?.SetTrigger("Attack");
        AudioManager.instance?.PlaySFX("Arrows");
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
            Debug.Log(targetUnit);
            return new EnemyAIAction {
                gridPosition = gridPosition,
                actionValue = 1000 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
            };
        }
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) {
        return GetValidGridPositionList(gridPosition).Count;
    }


    public Unit GetTargetUnit() { return targetUnit; }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

    public int GetDamage() {
        int damage = shootDamage;
        AudioManager.instance?.PlaySFX("DamageTaken");
        return damage;
    }

    public void ViewAreaDamage(GridPosition mousePosition) {
        GridSystemVisual.Instance.UpdateGridVisual();
        List<GridPosition> validGridPositionList = GetValidGridPositionList();
        List<GridPosition> attackGridPositionList = new List<GridPosition>();
        Vector3 mouseWorldPos = LevelGrid.Instance.GetWorldPosition(mousePosition);

        if (validGridPositionList.Contains(mousePosition)) {
            for (int x = -areaDamage; x <= areaDamage; x++) {
                for (int z = -areaDamage; z <= areaDamage; z++) {
                    GridPosition testGridPosition = mousePosition + new GridPosition(x, z, 0);

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > areaDamage) {
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

        GridSystemVisual.Instance.ShowGridPositionList(attackGridPositionList, GridVisualType.Red);
    }
}
