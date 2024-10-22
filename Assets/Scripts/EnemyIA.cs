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
                        Debug.Log("enemy turn end");
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
        if (!TurnSystem.Instance.IsPlayerTurn()) {
            state = State.TakingTurn;
            timer = 2f;
        }
        timer = 2f;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete) {
        Debug.Log("Take enemy AI action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyList()) {
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) {
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

        Debug.Log(bestBaseAction);

        if( bestEnemyAIAction != null && enemyUnit.TryToPerformAction(bestBaseAction)) {
            bestBaseAction.TriggerAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else {
            return false;
        }
    }
}