using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KillBoarQuestStep : QuestStep {

    private int boarsKilled = 0;
    private int boarsToComplete = 4;

    private void OnEnable() {
        Debug.Log("Subscribing");
        Debug.Log(TurnSystem.Instance);
        TurnSystem.Instance.onEnemyKilled += TurnSystem_OnEnemyKilled;
    }

    private void OnDisable() {
        TurnSystem.Instance.onEnemyKilled -= TurnSystem_OnEnemyKilled;
    }

    private void TurnSystem_OnEnemyKilled(object enemy, EventArgs e) {
        if(boarsKilled < boarsToComplete && (enemy as Unit).GetUnitId() == "boar") {
            boarsKilled++;
        }

        if (boarsKilled >= boarsToComplete) {
            this.FinishQuestStep();
        }
    }

}
