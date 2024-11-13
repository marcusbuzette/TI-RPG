using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{

    private enum State
    {
        Aiming, Shooting, Cooloff
    }
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private float aimingTimer = .1f;
    [SerializeField] private float shootingTimer = .3f;
    [SerializeField] private float cooloffTimer = .1f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private int shootDamage = 100;


    private State currentState;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShoot;

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override void Action()
    {
        stateTimer -= Time.deltaTime;
        switch (currentState)
        {
            case State.Aiming:
                Vector3 moveDirection = (targetUnit.transform.position - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShoot)
                {
                    Shoot();
                    canShoot = false;
                }
                break;
            case State.Cooloff:

                break;

        }
        if (stateTimer <= 0)
        {
            NextState();
        }



    }

    public override List<GridPosition> GetValidGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance) {
                    continue;
                }


                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask)) {
                    //Blocked by an Obstacle
                    continue;
                }
                

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete)
    {
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

    private void Shoot()
    {
        targetUnit.Damage(shootDamage);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        // Debug.Log("called");
        return GetValidGridPositionList(gridPosition).Count;
    }


    public Unit GetTargetUnit() { return targetUnit; }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }
}
