using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager Instance { get; private set; }

    private List<Unit> unitList;
    private List<Unit> friendlyList;
    private List<Unit> enemyList;

    private void Awake() {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

        unitList = new List<Unit>();
        friendlyList = new List<Unit>();
        enemyList = new List<Unit>();
    }

    private void Start() {
        Unit.OnAnyUnitSpawn += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnityDead;
        Unit.OnAnyUnitRevive += Unit_OnFriendlyUnitRevived;
    }

    private void Unit_OnAnyUnitSpawned(object sander, EventArgs e) {
        Unit unit = sander as Unit;

        unitList.Add(unit);

        if (unit.IsEnemy()) {
            enemyList.Add(unit);
        }
        else {
            friendlyList.Add(unit);
            if (!GameController.controller.HasUnitRecords(unit.GetUnitId())) {
                GameController.controller.AddUnitToRecords(unit);
            }
        }
    }

    private void Unit_OnAnyUnityDead(object sander, EventArgs e) {
        Unit unit = sander as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy()) {
            foreach (Unit friendlyUnit in unit.GetHealthSystem().GetDamagedByList()) {
                friendlyUnit.AddXp(unit.GetUnitStats().GetXpSpoil());
            }
            enemyList.Remove(unit);
            LevelGrid.Instance.RemoveUnitAtGridPosition(unit.GetGridPosition(), unit);
            Destroy(unit.gameObject);
        }
        else {
            friendlyList.Remove(unit);
        }
    }

    private void Unit_OnFriendlyUnitRevived(object sander, EventArgs e) {
        Unit unit = sander as Unit;

        unitList.Add(unit);
        friendlyList.Add(unit);
    }

    public List<Unit> GetUnitList() {
        return unitList;
    }

    public List<Unit> GetFriendlyList() {
        return friendlyList;
    }

    public List<Unit> GetEnemyList() {
        return enemyList;
    }
}
