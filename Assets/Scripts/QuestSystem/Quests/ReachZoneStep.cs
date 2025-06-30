using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReachZoneStep : QuestStep {
    [SerializeField] private int zoneId = 2;

    public EventHandler onFinishQuestStep;


    private void OnEnable() {
       LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
    }

    private void OnDisable() {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e) {
        Unit unit = (e as LevelGridEventArgs).unit;
        GridPosition pos = (e as LevelGridEventArgs).currentGridPos;
        if(!unit.IsEnemy() && pos.zone == zoneId) {
            onFinishQuestStep?.Invoke(this, EventArgs.Empty);
            this.FinishQuestStep();
        }
    }


}
