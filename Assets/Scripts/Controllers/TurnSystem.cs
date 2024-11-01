using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour {

    private int turnNumber = 0;
    private bool isPlayerTurn = true;
    [SerializeField] private List<Unit> unitiesOrderList = new List<Unit>();

    public static TurnSystem Instance { get; private set; }
    public event EventHandler onTurnChange;
    public event EventHandler onOrderChange;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("More than one Turn System");
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    private void Start() {
        unitiesOrderList = FindObjectsOfType<Unit>(false).ToList<Unit>();
        unitiesOrderList.Sort((x, y) => y.GetUnitSpeed().CompareTo(x.GetUnitSpeed()));
        unitiesOrderList[turnNumber].StartUnitTurn();
        onOrderChange.Invoke(this, EventArgs.Empty);

    }



    public void NextTurn() {
        turnNumber++;
        if (turnNumber >= unitiesOrderList.Count) {
            turnNumber = 0;
        }
        isPlayerTurn = !unitiesOrderList[turnNumber].IsEnemy();
        onTurnChange.Invoke(this, EventArgs.Empty);
        UnitActionSystem.Instance.ChangeSelectedUnit(unitiesOrderList[turnNumber]);
        unitiesOrderList[turnNumber].StartUnitTurn();
    }

    public int GetTurnNumber() { return turnNumber; }

    public void RemoveUnitFromList(Unit unitDead) {
        unitiesOrderList.Remove(unitDead);
        if (turnNumber > 0) { turnNumber--; }
        if (!CheckEnemiesLeft()) {
            SceneManager.LoadScene("HUB");
        }
    }

    public bool IsPlayerTurn() {
        return isPlayerTurn;
    }

    public List<Unit> GetTurnOrder() {
        List<Unit> currentTurnList = new(unitiesOrderList);
        for (int i = 0; i < turnNumber; i++) {
            Unit first = currentTurnList[0];
            currentTurnList.RemoveAt(0);
            currentTurnList.Add(first);
        }
        return currentTurnList;

    }

    private bool CheckEnemiesLeft() {
        foreach (Unit unit in unitiesOrderList) {
            if(unit.IsEnemy()) return true;
        }
        return false;
    }
}
