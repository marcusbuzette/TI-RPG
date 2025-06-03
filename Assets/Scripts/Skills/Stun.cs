using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stun : BaseSkills {
    public string quickAttackSFX;

    public int Attack = 1;
    private Unit targetUnit;
    [SerializeField] private int hitDamage = 15;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxHitDistance = 1;

    private void Start() {
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); 
    }

    public override void Action() {
        if (Attack == 1) {
            Attack = 0;
            RotateAndAttack();
        }
        StartCoroutine(DelayActionFinish());
    }

    private IEnumerator DelayActionFinish() {
        yield return new WaitForSeconds(0.5f); // Ajuste o tempo conforme necessário
        ActionFinish();
        Attack = 1;
        ActiveCoolDown();
    }

    public override string GetActionName() {
        return "Atordoar";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidGridPositionList(GridPosition unitGridPosition) {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxHitDistance; x <= maxHitDistance; x++) {
            for (int z = -maxHitDistance; z <= maxHitDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
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

                if (targetUnit.GetHealthSystem().GetHealthState() == HealthSystem.HealthState.FAINT) {
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

    public void RotateAndAttack() {
        StartCoroutine(RotateTowardsAndExecute(targetUnit.transform, () => {
            Damage();
        }));
    }

    public Unit GetTargetUnit() {
        return targetUnit;
    }

    protected void Damage() {
        // animator?.SetTrigger("Attack");
        unit.PlayAnimation("Attack");
        targetUnit?.Damage(hitDamage, false, this.GetComponent<Unit>());
        targetUnit?.StunUnit();
    }


    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        if (!string.IsNullOrEmpty(quickAttackSFX)) {
            AudioManager.instance?.PlaySFX(quickAttackSFX);  // vai tocar o sfx q ta no inspector da skill favor n mudar nada sem avisar
        }
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

    public override bool GetOnCooldown() { return onCoolDown; }

    public int GetMaxHitDistance() {
        return maxHitDistance;
    }
}
