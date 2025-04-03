using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystemVisual;

public class FireAttackObject : MonoBehaviour
{
    [SerializeField] private LayerMask obstaclesLayerMask;

    [SerializeField] private int damage;
    [SerializeField] private int areaDamage;
    [SerializeField] private int coolDown;

    private GameObject particleFire;

    private FireAttack fireAttack;

    public void SetFireAttackObject(FireAttack fireAttack, GameObject particleFire, int damage, int areaDamage, int coolDown) {
        this.fireAttack = fireAttack;
        this.coolDown = coolDown;
        this.damage = damage;
        this.areaDamage = areaDamage;
        this.particleFire = particleFire;

        obstaclesLayerMask = LayerMask.GetMask("Obstacles");
        TurnSystem.Instance.onTurnChange += TurnSystem_onTurnChange;
        ViewDamageArea();
    }

    private void TurnSystem_onTurnChange(object sender, EventArgs e) {
        if(coolDown == fireAttack.GetCoolDown()) {
            DamageOnArea(damage * 2);
        }
        else {
            DamageOnArea(damage);
        }

        coolDown--;
        if(coolDown == 0) {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        TurnSystem.Instance.onTurnChange -= TurnSystem_onTurnChange;
    }

    private void DamageOnArea(int _damage) {
        List<Unit> DamageUnitList = new List<Unit>();

        GridPosition thisGrid = LevelGrid.Instance.GetGridPosition(transform.position);
        Vector3 position = transform.position;

        for (int x = -areaDamage; x <= areaDamage; x++) {
            for (int z = -areaDamage; z <= areaDamage; z++) {
                GridPosition testGridPosition = thisGrid + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > areaDamage) {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
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

                DamageUnitList.Add(LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition));
            }
        }

        foreach(Unit units in DamageUnitList) {
            units.Damage(_damage);
        }
    }

    private void ViewDamageArea() {
        List<GridPosition> attackGridPositionList = new List<GridPosition>();
        GridPosition thisGrid = LevelGrid.Instance.GetGridPosition(transform.position);
        Vector3 position = transform.position;

        for (int x = -areaDamage; x <= areaDamage; x++) {
            for (int z = -areaDamage; z <= areaDamage; z++) {
                GridPosition testGridPosition = thisGrid + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > areaDamage) {
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
        for(int i = 0; i < postions.Count; i++) {
            Instantiate(particleFire, LevelGrid.Instance.GetWorldPosition(postions[i]), Quaternion.identity)
                .transform.parent = transform;
        }
    }
}
