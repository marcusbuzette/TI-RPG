using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystemVisual;

public class HealAreaObject : MonoBehaviour {
    [SerializeField] private LayerMask obstaclesLayerMask;

    [SerializeField] private int healPoints;
    [SerializeField] private int areaHeal;
    [SerializeField] private int coolDown;

    private GameObject particleHeal;

    private HealArea healArea;

    public void SetHealAreaObject(HealArea healArea, GameObject particleHeal, int healPoints, int areaHeal, int coolDown) {
        this.healArea = healArea;
        this.coolDown = coolDown;
        this.healPoints = healPoints;
        this.areaHeal = areaHeal;
        this.particleHeal = particleHeal;

        obstaclesLayerMask = LayerMask.GetMask("Obstacles");
        TurnSystem.Instance.onTurnChange += TurnSystem_onTurnChange;
        ViewHealArea();
    }

    private void TurnSystem_onTurnChange(object sender, EventArgs e) {
        if (coolDown == healArea.GetCoolDown()) {
            HealOnArea(healPoints);
        }
        else {
            HealOnArea(healPoints);
        }

        coolDown--;
        if (coolDown == 0) {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        TurnSystem.Instance.onTurnChange -= TurnSystem_onTurnChange;
    }

    private void HealOnArea(int _healPoints) {
        List<Unit> HealUnitList = new List<Unit>();

        GridPosition thisGrid = LevelGrid.Instance.GetGridPosition(transform.position);
        Vector3 position = transform.position;

        for (int x = -areaHeal; x <= areaHeal; x++) {
            for (int z = -areaHeal; z <= areaHeal; z++) {
                GridPosition testGridPosition = thisGrid + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > areaHeal) {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy()) {
                    continue;
                }

                Vector3 targetPos = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Vector3 shootDir = (targetPos - position).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(position + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(position, targetPos),
                    obstaclesLayerMask)) {

                    //Blocked by an Obstacle
                    continue;
                }

                HealUnitList.Add(LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition));
            }
        }

        foreach (Unit units in HealUnitList) {
            units.GetHealthSystem().Heal(_healPoints);
        }
    }

    private void ViewHealArea() {
        List<GridPosition> attackGridPositionList = new List<GridPosition>();
        GridPosition thisGrid = LevelGrid.Instance.GetGridPosition(transform.position);
        Vector3 position = transform.position;

        for (int x = -areaHeal; x <= areaHeal; x++) {
            for (int z = -areaHeal; z <= areaHeal; z++) {
                GridPosition testGridPosition = thisGrid + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > areaHeal) {
                    continue;
                }

                Vector3 targetPos = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Vector3 shootDir = (targetPos - position).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(position + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(position, targetPos),
                    obstaclesLayerMask)) {

                    //Blocked by an Obstacle
                    continue;
                }

                attackGridPositionList.Add(testGridPosition);
            }
        }

        CreateParticles(attackGridPositionList);
    }

    private void CreateParticles(List<GridPosition> postions) {
        for (int i = 0; i < postions.Count; i++) {
            Instantiate(particleHeal, LevelGrid.Instance.GetWorldPosition(postions[i]), Quaternion.identity)
                .transform.parent = transform;
        }
    }
}
