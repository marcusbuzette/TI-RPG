using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KillEnemyStep : QuestStep {

    [SerializeField] private int enemiesKilled = 0;
    [SerializeField] private int enemiesToComplete = 4;
    [SerializeField] private string enemyId = "boar";

    private void OnEnable() {
        TurnSystem.Instance.onEnemyKilled += TurnSystem_OnEnemyKilled;
    }

    private void OnDisable() {
        TurnSystem.Instance.onEnemyKilled -= TurnSystem_OnEnemyKilled;
    }

    private void TurnSystem_OnEnemyKilled(object enemy, EventArgs e) {
        if(enemiesKilled < enemiesToComplete && (enemy as Unit).GetUnitId() == enemyId) {
            enemiesKilled++;
        }

        Debug.Log("killed enemy - " + enemiesKilled);

        if (enemiesKilled >= enemiesToComplete) {
            this.FinishQuestStep();
        }
    }

}
