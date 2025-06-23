using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static GridSystemVisual;

public class AreaDefence : BaseSkills {
    public string shieldSFX;
    public GameObject obstaclePrefab;

    private GridPosition targetPosition;

    private int Attack = 1;

    private List<GameObject> wallList = new List<GameObject>();
    private bool isAiming = false;
    private GridPosition aimingMouseGridPosition;

    private void Update() {
        if (isAiming) {
            if (UnitActionSystem.Instance.GetSelectedAction() != this) return;
            aimingMouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            ViewAreaDamage(aimingMouseGridPosition);
        }
        if (!isActive) return;
        Action();
    }

    public override void Action() {
        if (Attack == 0) return;
        Attack = 0;
        RotateAndSkill();
        StartCoroutine(DelayActionFinish());
    }

    public void RotateAndSkill() {
        StartCoroutine(RotateTowardsAndExecute(LevelGrid.Instance.GetWorldPosition(targetPosition), () => {
            PerformSkill();
        }));
    }

    private IEnumerator DelayActionFinish() {
        yield return new WaitForSeconds(0.5f); // Ajuste o tempo conforme necessário
        ActionFinish();
        Attack = 1;
    }

    private void PerformSkill() {
        GridPosition unitPos = unit.GetGridPosition();
        GridPosition rawDirection = targetPosition - unitPos;

        GridPosition direction = new GridPosition(
            Mathf.Clamp(rawDirection.x, -1, 1),
            Mathf.Clamp(rawDirection.z, -1, 1),
            0
        );

        // Garante que a direção seja válida (não permite 0,0)
        if (direction.x == 0 && direction.z == 0) {
            Debug.LogWarning("Direção inválida para gerar barreira.");
            ActionFinish();
            return;
        }

        // Gira o personagem para a direção escolhida
        Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
        if (lookDir != Vector3.zero) {
            unit.transform.forward = lookDir;
        }

        // Define direção perpendicular
        GridPosition perpDir;
        if (direction.x != 0) {
            // Jogador clicou em esquerda/direita → barreira será vertical
            perpDir = new GridPosition(0, 1, 0);
        }
        else {
            // Jogador clicou em cima/baixo → barreira será horizontal
            perpDir = new GridPosition(1, 0, 0);
        }

        // Gera barreira de 5 blocos (perpendicular à direção escolhida, mas à frente do tanque)
        for (int i = -2; i <= 2; i++) {
            GridPosition offset = new GridPosition(
                perpDir.x * i,
                perpDir.z * i,
                0
            );

            GridPosition spawnPos = unitPos + offset;

            if (!LevelGrid.Instance.IsValidGridPosition(spawnPos)) continue;
            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(spawnPos)) continue;
            if (spawnPos.floor != unitPos.floor) continue;

            this.CreateShieldWall(spawnPos);
        }

        ActiveCoolDown();
        ActionFinish();
    }


    public override string GetActionName() {
        return "Bastiao Irredutivel";
    }

    private void CreateShieldWall(GridPosition gridPos) {
        Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(gridPos);
        GameObject wall = GameObject.Instantiate(obstaclePrefab, worldPos, Quaternion.identity);
        wallList.Add(wall);
        PathFinding.Instance.SetNodeIsWalkable(worldPos, false);
    }

    public override List<GridPosition> GetValidGridPositionList() {
        isAiming = true;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -1; x <= 1; x++) {
            for (int z = -1; z <= 1; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > 1) {
                    continue;
                }

                if (testGridPosition.floor != unit.GetGridPosition().floor) {
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        isAiming = false;
        this.targetPosition = mouseGridPosition;
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
            if (wallList.Count > 0) RemoveWall();
        }
        if (currentCoolDown == 0) {
            onCoolDown = false;
        }
    }

    public override bool GetOnCooldown() { return onCoolDown; }

    private void RemoveWall() {
        foreach (GameObject wall in wallList) {
            PathFinding.Instance.SetNodeIsWalkable(wall.transform.position, false);
            Destroy(wall);
        }
    }

    private void ViewAreaDamage(GridPosition mousePosition) {
        if (!LevelGrid.Instance.IsValidGridPosition(mousePosition)) return;
        GridSystemVisual.Instance.UpdateGridVisual();
        List<GridPosition> shieldPositionList = new List<GridPosition>();
        List<GridPosition> validPositions = GetValidGridPositionList();
    if (!validPositions.Contains(mousePosition)) return;

        GridPosition unitPos = unit.GetGridPosition();
        GridPosition rawDirection = mousePosition - unitPos;

        GridPosition direction = new GridPosition(
            Mathf.Clamp(rawDirection.x, -1, 1),
            Mathf.Clamp(rawDirection.z, -1, 1),
            0
        );

        // Garante que o jogador está clicando em uma célula válida
        if (direction.x == 0 && direction.z == 0) return;

        // Define a direção perpendicular à direção do clique
        GridPosition perpDir;
        if (direction.x != 0) {
            // Clique horizontal (esq/dir) → barreira vertical
            perpDir = new GridPosition(0, 1, 0);
        }
        else {
            // Clique vertical (cima/baixo) → barreira horizontal
            perpDir = new GridPosition(1, 0, 0);
        }

        // Calcula as 5 posições ao redor do personagem na direção perpendicular
        for (int i = -2; i <= 2; i++) {
            GridPosition offset = new GridPosition(perpDir.x * i, perpDir.z * i, 0);
            GridPosition testPos = unitPos + offset;

            if (!LevelGrid.Instance.IsValidGridPosition(testPos)) continue;
            if (testPos.floor != unitPos.floor) continue;

            shieldPositionList.Add(testPos);
        }


        GridSystemVisual.Instance.ShowGridPositionList(shieldPositionList, GridVisualType.BlueSoft);
    }


    public override void CopyFrom(BaseSkills other) {
        base.CopyFrom(other);

        obstaclePrefab = (other as AreaDefence).obstaclePrefab;
    }
}
