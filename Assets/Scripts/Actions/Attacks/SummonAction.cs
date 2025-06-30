using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SummonAction : BaseAction {
    [Header("Summon Settings")]
    [SerializeField] private int maxSummonDistance = 3;
    [SerializeField] private int maxSummonsPerUse = 2;
    [SerializeField] private Unit summonableEnemyPrefab;
    [SerializeField] private int summonCooldownTurns = 3;

    [Header("AI Parameters")]
    [SerializeField] private int baseAIValue = 10000;
    [SerializeField] private int allyBonusValue = 30;
    [SerializeField] private int enemyPenaltyValue = -20;

    private int currentCooldownTurns = 0;
    private int currentSummons = 0;
    public int Attack = 1;


    private void Start() {
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
    }

    private void OnDestroy() {
        if (TurnSystem.Instance != null) {
            TurnSystem.Instance.onTurnChange -= TurnSystem_OnTurnChange;
        }
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {

    }

    public override string GetActionName() => "Summon Allies";

    public override void Action() {
        if (!unit.IsEnemy()) return;
        if (Attack == 1) {
            SummonEnemies();
            Attack = 0;
        }
        currentCooldownTurns = summonCooldownTurns;
        StartCoroutine(AtrasarFimDaAcao());
    }

    public IEnumerator AtrasarFimDaAcao() {
        yield return new WaitForSeconds(2f);
        ActionFinish();
        Attack = 1;
    }

    private void SummonEnemies() {
        List<GridPosition> validPositions = GetOptimalSummonPositions();
        currentSummons = 0;

        foreach (GridPosition position in validPositions) {
            if (currentSummons >= maxSummonsPerUse) break;

            CreateEnemyUnit(position);
            currentSummons++;
        }
    }

    private void CreateEnemyUnit(GridPosition position) {
        if (summonableEnemyPrefab == null) return;

        Unit summonedEnemy = Instantiate(
            summonableEnemyPrefab,
          LevelGrid.Instance.GetWorldPosition(position),
           Quaternion.identity);

        summonedEnemy.GetComponent<HealthSystem>().SetMaxHP(summonedEnemy.GetUnitStats().GetMaxHP());

        TurnSystem.Instance.GetUnitsOrderList().Add(summonedEnemy);
        TurnSystem.Instance.NotifyOrderChange();

    }

    private List<GridPosition> GetOptimalSummonPositions() {
        List<GridPosition> allPossibleSummonPositions = new List<GridPosition>();
        GridPosition summonerPos = unit.GetGridPosition();


        foreach (GridPosition testPos in LevelGrid.Instance.GetZoneList()) {
            if (testPos == summonerPos) {
                continue;
            }

            if (IsValidSummonPosition(testPos)) {
                allPossibleSummonPositions.Add(testPos);
            }
        }


        allPossibleSummonPositions.Sort((a, b) =>
            GetPositionPriority(b).CompareTo(GetPositionPriority(a)));

        return allPossibleSummonPositions;
    }

    private bool IsValidSummonPosition(GridPosition position) {
        if (!LevelGrid.Instance.IsValidGridPosition(position)) return false;

        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(position)) return false;

        if (position.zone != LevelGrid.Instance.GetCurrentBattleZone()) return false;


        return HasAllyAdjacent(position);
    }

    private bool HasAllyAdjacent(GridPosition position) {
        GridPosition[] directions = {
            new GridPosition(1, 0, 0),
            new GridPosition(-1, 0, 0),
            new GridPosition(0, 1, 0),
            new GridPosition(0, -1, 0)
        };

        foreach (var dir in directions) {
            GridPosition neighbor = position + dir;
            if (HasUnitOfType(neighbor, unit.IsEnemy())) {
                return true;
            }
        }
        return false;
    }

    private bool HasUnitOfType(GridPosition gridPosition, bool isEnemy) {
        Unit unitAtPosition = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return unitAtPosition != null && unitAtPosition.IsEnemy() == isEnemy;
    }

    private int GetPositionPriority(GridPosition position) {
        int priority = 0;
        priority += CountAdjacentAllies(position) * 20;
        priority -= CountAdjacentEnemies(position) * 15;

        return priority;
    }

    private int CountAdjacentAllies(GridPosition position) {
        return CountAdjacentUnitsOfType(position, unit.IsEnemy());
    }

    private int CountAdjacentEnemies(GridPosition position) {
        return CountAdjacentUnitsOfType(position, !unit.IsEnemy());
    }

    private int CountAdjacentUnitsOfType(GridPosition position, bool isEnemy) {
        GridPosition[] directions = {
            new GridPosition(1, 0, 0),
            new GridPosition(-1, 0, 0),
            new GridPosition(0, 1, 0),
            new GridPosition(0, -1, 0)
        };
        int count = 0;

        foreach (var dir in directions) {
            GridPosition neighbor = position + dir;
            if (HasUnitOfType(neighbor, isEnemy)) {
                count++;
            }
        }
        return count;
    }

    public override List<GridPosition> GetValidGridPositionList() {

        List<GridPosition> validZonePositions = new List<GridPosition>();
        foreach (GridPosition testPos in LevelGrid.Instance.GetZoneList()) {
            if (IsValidSummonPosition(testPos)) {
                validZonePositions.Add(testPos);
            }
        }
        return validZonePositions;

    }

    public override void TriggerAction(GridPosition gridPosition, Action onActionComplete) {
        ActionStart(onActionComplete);
        Action();
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        int alliesNearby = CountAlliesInRange(maxSummonDistance * 2);
        int enemiesNearby = CountEnemiesInRange(maxSummonDistance * 2);

        int actionValue = baseAIValue +
                         (alliesNearby * allyBonusValue) +
                         (enemiesNearby * enemyPenaltyValue);


        if (currentCooldownTurns > 0) {
            actionValue = 0;
        }

        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = actionValue
        };
    }

    private int CountAlliesInRange(int range) {
        return TurnSystem.Instance.GetUnitsOrderList()
            .Count(u => u.IsEnemy() == unit.IsEnemy() &&
                   CalculateDistance(unit.GetGridPosition(), u.GetGridPosition()) <= range);
    }

    private int CountEnemiesInRange(int range) {
        return TurnSystem.Instance.GetUnitsOrderList()
            .Count(u => u.IsEnemy() != unit.IsEnemy() &&
                   CalculateDistance(unit.GetGridPosition(), u.GetGridPosition()) <= range);
    }

    private float CalculateDistance(GridPosition a, GridPosition b) {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2));
    }

    public override bool GetOnCooldown() => currentCooldownTurns > 0;

    public override void IsAnotherRound() {
        currentSummons = 0;
        if (currentCooldownTurns > 0) {
            currentCooldownTurns--;
        } else {
            currentCooldownTurns = 0;
        }
    }
}