using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class EnemyIA : MonoBehaviour {

    private enum State {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake() {
        state = State.WaitingForEnemyTurn;
    }

    private void Start() {
        TurnSystem.Instance.onTurnChange += TurnSystem_onTurnChange;
        TurnSystem.Instance.onOrderChange += TurnSystem_onTurnChange;
    }
    private void Update() {

        if (TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }

        switch (state) {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f) {
                    state = State.Busy;
                    if (TryTakeEnemyAIAction(SetStateTakingTurn)) {
                        state = State.Busy;
                    }
                    else {
                        //No more enemies have actions they can take, end enemy turn
                        // Debug.Log("enemy turn end");
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:

                break;

        }
    }

    private void SetStateTakingTurn() {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_onTurnChange(object sender, EventArgs e) {
        if (!TurnSystem.Instance.IsPlayerTurn() && state != State.TakingTurn) {
            state = State.TakingTurn;
            timer = 2f;
        }
        timer = 2f;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete) {
        // Debug.Log("Take enemy AI action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyList()) {
            if(enemyUnit.IsUnityTurn() && TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetActionsArray()) {
            if(baseAction.GetActionType() == ActionType.MOVE) {
                if (enemyUnit.GetHasMoved()) {
                    //Enemy cannot move any more
                    continue;
                }
            }
            else {
                //This action is not a movement action
                if (enemyUnit.GetHasPerformedAction()) {
                    //Enemy cannot afford this action
                    continue;
                }
            }

            if (bestEnemyAIAction == null) {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }else{
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestBaseAction != null && bestEnemyAIAction != null) {
            //Verifica se a acao escolhida foi um movimento e se for verifica se o valor é 0
            if (bestBaseAction.GetActionType() == ActionType.MOVE && bestEnemyAIAction.actionValue == 0) {
                int closeDistance = int.MaxValue;
                GridPosition closestPlayer = enemyUnit.GetGridPosition();

                //Separa os players da lista de unidades
                List<Unit> units = TurnSystem.Instance.GetTurnOrder();
                List<Unit> playerUnits = new List<Unit>();
                foreach (Unit unit in units) {
                    if (!unit.IsEnemy()) {
                        playerUnits.Add(unit);
                    }
                }

                //Encontra a distancia do player mais proximo
                foreach (Unit playerUnit in playerUnits) {
                    var dist = PathFinding.Instance.CalculateDistance(
                        enemyUnit.GetGridPosition(), playerUnit.GetGridPosition());

                    //salva a GridPosition do player mais proximo
                    if (dist < closeDistance) {
                        closeDistance = dist;
                        closestPlayer = playerUnit.GetGridPosition();
                    }
                }

                //Recebe a lista de GridPositions de locais possiveis para o inimigo se movimentar
                var moveAction = bestBaseAction.gameObject.GetComponent<MoveAction>();
                List<GridPosition> gridList = moveAction.GetValidGridPositionList();

                closeDistance = int.MaxValue;

                //Calcula qual a posição mais próxima do jogador e vai para essa posição
                foreach (GridPosition grid in gridList) {
                    var dist = PathFinding.Instance.CalculateDistance(
                        grid, closestPlayer);
                    if (dist < closeDistance) {
                        closeDistance = dist;
                        bestEnemyAIAction.gridPosition = grid;
                    }
                }
            }
        }

        if( bestEnemyAIAction != null && enemyUnit.TryToPerformAction(bestBaseAction)) {
            bestBaseAction.TriggerAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else {
            return false;
        }
    }
}