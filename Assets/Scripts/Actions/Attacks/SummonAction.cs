using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SummonAction : BaseAction 
{
    [Header("Summon Settings")]
    [SerializeField] private int maxSummonDistance = 3;
    [SerializeField] private int maxSummonsPerUse = 2;
    [SerializeField] private Unit summonableEnemyPrefab;
    [SerializeField] private int summonCooldownTurns = 4;
    
    [Header("AI Parameters")]
    [SerializeField] private int baseAIValue = 120;
    [SerializeField] private int allyBonusValue = 30;
    [SerializeField] private int enemyPenaltyValue = -20;
    
    private int currentCooldownTurns = 0;
    private int currentSummons = 0;

    private void Start() {
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
    }

    private void OnDestroy() {
        if (TurnSystem.Instance != null) {
            TurnSystem.Instance.onTurnChange -= TurnSystem_OnTurnChange;
        }
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        if (currentCooldownTurns > 0) {
            currentCooldownTurns--;
        }
    }

    public override string GetActionName() => "Summon Allies";

    public override void Action() {
        if (!unit.IsEnemy()) return;
        
        SummonEnemies();
        currentCooldownTurns = summonCooldownTurns;
        ActionFinish();
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
        // if (summonableEnemyPrefab == null) return;

        // Unit summonedEnemy = Instantiate(
        //     summonableEnemyPrefab, 
        //     LevelGrid.Instance.GetWorldPosition(position), 
        //     Quaternion.identity);

        // summonedEnemy.GetComponent<HealthSystem>().SetMaxHP(summonedEnemy.GetUnitStats().GetMaxHP());
        
        // TurnSystem.Instance.GetComponent<TurnSystem>().AddUnitsToUnitOrderList(summonedEnemy);
    }

    private List<GridPosition> GetOptimalSummonPositions() {
        List<GridPosition> validPositions = new List<GridPosition>();
        GridPosition summonerPos = unit.GetGridPosition();

        for (int x = -maxSummonDistance; x <= maxSummonDistance; x++) {
            for (int z = -maxSummonDistance; z <= maxSummonDistance; z++) {
                GridPosition testPos = summonerPos + new GridPosition(x, z, 0);
                
                if (IsValidSummonPosition(testPos)) {
                    validPositions.Add(testPos);
                }
            }
        }
        validPositions.Sort((a, b) => 
            GetPositionPriority(b).CompareTo(GetPositionPriority(a)));

        return validPositions;
    }

    private bool IsValidSummonPosition(GridPosition position) {
        if (!LevelGrid.Instance.IsValidGridPosition(position)) return false;
        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(position)) return false;
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
        return new List<GridPosition> { unit.GetGridPosition() };
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

        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = actionValue
        };
    }

    private int CountAlliesInRange(int range) {
        return TurnSystem.Instance.GetComponent<TurnSystem>().GetTurnOrder()
            .Count(u => u.IsEnemy() == unit.IsEnemy() && 
                   CalculateDistance(unit.GetGridPosition(), u.GetGridPosition()) <= range);
    }

    private int CountEnemiesInRange(int range) {
        return TurnSystem.Instance.GetComponent<TurnSystem>().GetTurnOrder()
            .Count(u => u.IsEnemy() != unit.IsEnemy() && 
                   CalculateDistance(unit.GetGridPosition(), u.GetGridPosition()) <= range);
    }

    private float CalculateDistance(GridPosition a, GridPosition b) {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2));
    }

    public override bool GetOnCooldown() => currentCooldownTurns > 0;

    public override void IsAnotherRound() {
        currentSummons = 0;
    }
}