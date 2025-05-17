using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootAction : BaseAction
{
    private enum State
    {
        Aiming, Shooting, Cooloff
    }
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private int shootDamage = 100;

    public string arrowSFX;

    private State currentState;
    private Unit targetUnit;
    private bool canShoot;

    private void Start() {
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); //add layer mask to don't shoot through obstacles
        this.maxShootDistance = GetComponent<Unit>().GetUnitStats().GetRange();
    }
    public override string GetActionName()
    {
        return "Atirar";
    }

    public override void Action()
    {
        if (canShoot) {
            canShoot = false;
            StartCoroutine(RotateTowardsAndExecute(targetUnit.transform, () => {
                Shoot();
            }));
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
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
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

                if (targetUnit.GetHealthSystem().GetHealthState() == HealthSystem.HealthState.FAINT) {
                    continue;
                }

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
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        canShoot = true;
        if (!string.IsNullOrEmpty(arrowSFX)) {
            AudioManager.instance?.PlaySFX(arrowSFX);
        }
        ActionStart(onActionComplete);
    }

    protected void NextState()
    {
        switch (currentState)
        {
            case State.Aiming:
                currentState = State.Shooting;
                break;
            case State.Shooting:
                currentState = State.Cooloff;
                break;
            case State.Cooloff:
                ActionFinish();
                break;

        }
    }

    private void Shoot()
    {
        Debug.Log("SHOOT");
        targetUnit.Damage(shootDamage, true, this.GetComponent<Unit>());
        // animator?.SetTrigger("Attack");
        unit.PlayAnimation("Attack");
        AudioManager.instance?.PlaySFX("Arrows");
        ActionFinish();
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((GetTargetCountAtPosition(gridPosition)) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidGridPositionList(gridPosition).Count;
    }


    public Unit GetTargetUnit() { return targetUnit; }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

    public int GetDamage(){
        int damage = shootDamage;
        //AudioManager.instance?.PlaySFX("DamageTaken");
        return damage;
        
    }
}
