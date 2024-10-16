using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction {

    private enum State {
        Aiming, Shooting, Cooloff
    }
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

    public override string GetActionName() {
        return "Shoot";
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
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }


                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy()) {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        ActionStart(onActionComplete);
        currentState = State.Aiming;
        stateTimer = aimingTimer;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        canShoot = true;
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
                ActionFinish();
                break;

        }
    }

    private void Shoot() {
        targetUnit.Damage(shootDamage);
    }
}
