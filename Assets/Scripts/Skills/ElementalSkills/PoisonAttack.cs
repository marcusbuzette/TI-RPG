using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAttack : BaseSkills 
{
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private int damage = 100;
    [SerializeField] private GameObject PoisonFbx;

    public string poisonArrowSFX;
    private Unit targetUnit;
    private bool canShoot;


    private void Start() {
        GameObject PoisonFbx = GameObject.Find("Poison Arrow Cast");
        // Garantir que o VFX comece desativado
        if (PoisonFbx != null) {
            PoisonFbx.SetActive(false);
        }
        obstaclesLayerMask = LayerMask.GetMask("Obstacles"); //add layer mask to don't shoot through obstacles
    }
    public override string GetActionName() {
        return "Veneno";
    }

    public override void Action() {
        if (canShoot) {
            canShoot = false;
            GridSystemVisual.Instance.HideAllGridPosition();
            StartCoroutine(RotateTowardsAndExecute(targetUnit.transform, () => {
                Shoot();
            }));
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
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        canShoot = true;


        // Ativa o VFX quando começa a ação
        if (PoisonFbx != null) {
            PoisonFbx.SetActive(true);
        }

        if (!string.IsNullOrEmpty(poisonArrowSFX)) {
            AudioManager.instance?.PlaySFX(poisonArrowSFX);
        }
        ActionStart(onActionComplete);
    }

    private void Shoot() {
        unit.SpawnProjectile(targetUnit.transform.position, Color.green);

        if (targetUnit.gameObject.GetComponent<PoisonEffect>() != null) {
            targetUnit.gameObject.GetComponent<PoisonEffect>().CurePoison();
            targetUnit.gameObject.AddComponent<PoisonEffect>().SetPoisonEffect(targetUnit, damage, coolDown);
        }
        else targetUnit.gameObject.AddComponent<PoisonEffect>().SetPoisonEffect(targetUnit, damage, coolDown);
        // animator?.SetTrigger("Attack");
        unit.PlayAnimation("Attack");
        //AudioManager.instance?.PlaySFX("Arrows");

        if (PoisonFbx != null) {
            PoisonFbx.SetActive(false);
        }
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
