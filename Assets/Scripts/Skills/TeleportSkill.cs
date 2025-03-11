using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportSkill : BaseSkills
{
    [SerializeField] private int maxTeleportDistance = 4;
    private GridPosition targetGrid;

    private float totalSpinAmmount = 0;
    [SerializeField] private float MAX_SPIN = 360f;

    public override void Action() {
        float spinAddAmmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmmount, 0);
        totalSpinAmmount += spinAddAmmount;
        bool teleported = false;

        if(totalSpinAmmount > MAX_SPIN / 2 && !teleported) {
            Teleport();
        }

        if (totalSpinAmmount > MAX_SPIN) {
            ActiveCoolDown();
            ActionFinish();
        }
    }

    public override string GetActionName() {
        return "Teleportar";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxTeleportDistance; x <= maxTeleportDistance; x++) {
            for (int z = -maxTeleportDistance; z <= maxTeleportDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        targetGrid = mouseGridPosition;

        ActionStart(onActionComplete);

    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override void IsAnotherRound() {
        if (currentCoolDown != 0) {
            currentCoolDown--;
        }
        if (currentCoolDown == 0) {
            onCoolDown = false;
        }
    }

    private void Teleport() {
        unit.transform.position = LevelGrid.Instance.GetWorldPosition(targetGrid);
        LevelGrid.Instance.UnitMovedGridPosition(unit, unit.GetGridPosition(), targetGrid);
        AudioManager.instance?.PlaySFX("Teleport");
    }

    public override bool GetOnCooldown() { return onCoolDown; }
}
