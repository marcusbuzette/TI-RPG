using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportSkill : BaseSkills
{
    [SerializeField] private int maxTeleportDistance = 4;
    private GridPosition targetGrid;

    private float teleportDelay = 0.55f;

    public override void Action() {
        StartCoroutine(DelayedTeleport());
    }

    private IEnumerator DelayedTeleport() {
        unit.PlayAnimation("Teleport");

        yield return new WaitForSeconds(teleportDelay);
        AudioManager.instance?.PlaySFX("Teleport");

        Teleport();
        ActiveCoolDown();
        ActionFinish();
    }

    public override string GetActionName() {
        return "Teleportar";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxTeleportDistance; x <= maxTeleportDistance; x++) {
            for (int z = -maxTeleportDistance; z <= maxTeleportDistance; z++) {
                for (int floor = -maxTeleportDistance; floor <= maxTeleportDistance; floor++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;
                    if (unitGridPosition == testGridPosition) continue;
                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;
                    if (!PathFinding.Instance.IsWalkableGridPosition(testGridPosition)) continue;
                    if (!PathFinding.Instance.HasPath(unitGridPosition, testGridPosition)) continue;

                    validGridPositionList.Add(testGridPosition);
                }
            }
        }

        return validGridPositionList;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        targetGrid = mouseGridPosition;
        PlaySkillSFX();
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
        
    }

    public override bool GetOnCooldown() { return onCoolDown; }
    public int GetMaxTeleportDistance() { return maxTeleportDistance; }

    public override void CopyFrom(BaseSkills other) {
        base.CopyFrom(other);

        maxTeleportDistance = (other as TeleportSkill).GetMaxTeleportDistance();
    }
}
