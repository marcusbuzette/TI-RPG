using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAttack : BaseSkills {
    private enum State {
        Aiming, Shooting, Cooloff
    }
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private float aimingTimer = .1f;
    [SerializeField] private float shootingTimer = .3f;
    [SerializeField] private float cooloffTimer = .1f;
    [SerializeField] private float rotateSpeed = 10f;
     [SerializeField] private GameObject IceFbx;

    public string freezeArrowSFX;

    private State currentState;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShoot;



    private void Start() {
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); //add layer mask to don't shoot through obstacles
        GameObject IceFbx = GameObject.Find("Ice Magic Cast");
        // Garantir que o VFX comece desativado
        if (IceFbx != null) {
            IceFbx.SetActive(false);
        }
    }
    public override string GetActionName() {
        return "Congelar";
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
                    Shoot();
                    canShoot = false;
                    NextState();
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


                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy()) {
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

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        currentState = State.Aiming;
        stateTimer = aimingTimer;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        canShoot = true;

        // Ativa o VFX quando começa a ação
        if (IceFbx != null) {
            IceFbx.SetActive(true);
        }

        if (!string.IsNullOrEmpty(freezeArrowSFX)) {
            AudioManager.instance?.PlaySFX(freezeArrowSFX);  // vai tocar o sfx q ta no inspector da skill favor n mudar nada sem avisar
        }
        ActionStart(onActionComplete);
    }

    private void NextState() {
        switch (currentState) {
            case State.Aiming:
                currentState = State.Shooting;
                stateTimer = shootingTimer;
                break;
            case State.Shooting:
                currentState = State.Cooloff;
                stateTimer = shootingTimer;
                break;
            case State.Cooloff:
             if (IceFbx != null) {
                    IceFbx.SetActive(false);
                }
                ActionFinish();
                break;

        }
    }

    private void Shoot() {
        if (targetUnit.gameObject.GetComponent<FreezeEffect>() != null) {
            targetUnit.gameObject.GetComponent<FreezeEffect>().CureFreeze();
            targetUnit.gameObject.AddComponent<FreezeEffect>().SetFreezeEffect(targetUnit, coolDown);
        }
        else targetUnit.gameObject.AddComponent<FreezeEffect>().SetFreezeEffect(targetUnit, coolDown);
        // animator?.SetTrigger("Attack");
        unit.PlayAnimation("Attack");
        //AudioManager.instance?.PlaySFX("Arrows");
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
        // Debug.Log("called");
        return GetValidGridPositionList(gridPosition).Count;
    }


    public Unit GetTargetUnit() { return targetUnit; }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }
}
