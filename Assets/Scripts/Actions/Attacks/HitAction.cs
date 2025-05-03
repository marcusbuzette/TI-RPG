using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class HitAction : BaseAction {

    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxHitDistance = 1;
    [SerializeField] private int hitDamage = 50;
    [SerializeField] private float rotateSpeed = 10f;
    public string meleeSFX;
    public int Attack = 1;

    private Unit targetUnit;

    private void Start() {
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); //add layer mask to don't shoot through obstacles
    }

    public override string GetActionName() {
        return "Ataque";
    }

    public override void Action() {
        if (Attack == 1) {
            // animator?.SetTrigger("Attack");
            unit.PlayAnimation("Attack");

            
            Attack = 0;
            targetUnit?.Damage(hitDamage, this.GetComponent<Unit>());
        }
        StartCoroutine(DelayActionFinish());



    }

     //public void Damaged(){
            //targetUnit?.Damage(hitDamage);}

    private IEnumerator DelayActionFinish() {
        yield return new WaitForSeconds(0.5f); // Ajuste o tempo conforme necessário
        ActionFinish();
        Attack = 1;

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
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        if (!string.IsNullOrEmpty(meleeSFX)) {
            AudioManager.instance?.PlaySFX(meleeSFX); // para cada personagem com esta ação, tem que colocar o respectivo som no inspector do prefab. Exemplo o protagonista tem no sound manager o ProtagHit, então esse nome tem que estar no inspector de seu prefab.
        }
        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((GetTargetCountAtPosition(gridPosition)) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) {
        // Debug.Log("called");
        return GetValidGridPositionList(gridPosition).Count;
    }

    public Unit GetTargetUnit() {
        return targetUnit;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

    public int GetDamage() {
        int damage = hitDamage;
        //AudioManager.instance?.PlaySFX("DamageTaken");
        return damage;

    }

    public int GetMaxHitDistance() {
        return maxHitDistance;
    }
}
