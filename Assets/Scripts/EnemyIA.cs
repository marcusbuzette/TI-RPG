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
        SpinAction spinAction = enemyUnit.GetSpinAction();

        GridPosition actionGridPosition = enemyUnit.GetGridPosition();
        if (!spinAction.IsValidActionGridPosition(actionGridPosition)) {
            return false;
        }

        if (!enemyUnit.TryToPerformAction(spinAction)) {
            return false;
        }

        Debug.Log("Spin Action");
        spinAction.TriggerAction(actionGridPosition, onEnemyAIActionComplete);
        return true;
    }
}