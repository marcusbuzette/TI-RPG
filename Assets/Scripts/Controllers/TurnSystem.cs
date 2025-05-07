using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour {

    private int turnNumber = 0;
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private List<Unit> unitiesOrderList = new List<Unit>();
    private List<Unit> allEnemies = new List<Unit>();

    public static TurnSystem Instance { get; private set; }
    public event EventHandler onTurnChange;
    public event EventHandler onOrderChange;
    private CameraController cameraController;

    [SerializeField] private int[] turnSpeeds;
    private int turnSpeedIndex;

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
        turnNumber = 0;
        unitiesOrderList = FindObjectsOfType<Unit>(false)
            .Where(unit => unit.GetGridPosition().zone == LevelGrid.Instance.GetCurrentBattleZone()).ToList<Unit>();
        allEnemies = FindObjectsOfType<Unit>(false).Where(unit => unit.IsEnemy()).ToList<Unit>();
        unitiesOrderList.Sort((x, y) => y.GetUnitSpeed().CompareTo(x.GetUnitSpeed()));
        isPlayerTurn = !unitiesOrderList[turnNumber].IsEnemy();
        unitiesOrderList[turnNumber].StartUnitTurn();
        onOrderChange.Invoke(this, EventArgs.Empty);

    }

    public void SetUpBattleNewZone() {
        List<Unit> playerUnits = FindObjectsOfType<Unit>(false).Where(unit => unit.IsEnemy() == false).ToList<Unit>();
        for (int i = 0; i < playerUnits.Count; i++) {
            UnitActionSystem.Instance.MoveUnitToGridPosition(playerUnits[i],
            LevelGrid.Instance.GetZoneSpawnList(LevelGrid.Instance.GetCurrentBattleZone())[i]);

        }

    }

    public void StartBattleNewZone() {
        turnNumber = 0;
        unitiesOrderList = FindObjectsOfType<Unit>(false)
            .Where(unit => (unit.GetGridPosition().zone == LevelGrid.Instance.GetCurrentBattleZone() || !unit.IsEnemy())).ToList<Unit>();
        unitiesOrderList.Sort((x, y) => y.GetUnitSpeed().CompareTo(x.GetUnitSpeed()));
        isPlayerTurn = !unitiesOrderList[turnNumber].IsEnemy();
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
        //Place the camera in the unit position of the turn
        Vector3 unitTurnTransform = unitiesOrderList[turnNumber].transform.position;
        if (isPlayerTurn) { cameraController.LockCameraOnSelectedUnit(unitiesOrderList[turnNumber]); }
        else cameraController.GoToPosition(unitTurnTransform);

        unitiesOrderList[turnNumber].StartUnitTurn();
    }

    private void ComboKill() {
        isPlayerTurn = !unitiesOrderList[turnNumber].IsEnemy();
        onTurnChange.Invoke(this, EventArgs.Empty);
        unitiesOrderList[turnNumber].StartUnitTurn();
    }

    public int GetTurnNumber() { return turnNumber; }

    public void RemoveUnitFromList(Unit unitDead) {
        int unitDeadIndex = unitiesOrderList.FindIndex((u) => u.transform == unitDead.transform);
        if (unitDead.IsEnemy()) {
            this.unitiesOrderList[this.turnNumber]
                .AddXp(this.unitiesOrderList[unitDeadIndex].GetUnitStats().GetXpSpoil());
                
            allEnemies.Remove(unitDead);
        }
        unitiesOrderList.Remove(unitDead);
        if (turnNumber > unitDeadIndex) { turnNumber--; }
        if (isPlayerTurn && CheckEnemiesLeftInTheBattleZone()) {
            ComboKill();
        }
        else if (isPlayerTurn && !CheckEnemiesLeftInTheBattleZone() && CheckEnemiesLeft()) {
            LevelGrid.Instance.RemoveZoneFromGrid(LevelGrid.Instance.GetCurrentBattleZone());
            List<Unit> playerUnits = FindObjectsOfType<Unit>(false).Where(unit => unit.IsEnemy() == false).ToList<Unit>();
            foreach (Unit unit in playerUnits) {
                unit.UpdateGridPositionZone(0);
            }
            ComboKill();
            LevelGrid.Instance.ExploreMode();
        }
        else if (!isPlayerTurn && !CheckPlayerCharsLeft()) {
            ResetTurnSpeed();
            GameController.controller.GameOver();
        }
        else if (isPlayerTurn && !CheckEnemiesLeft()) {
            ResetTurnSpeed();
            foreach (Unit u in unitiesOrderList) {
                if (!u.IsEnemy()) u.AddXp(2);
            }

            GameController.controller.NextLevel();
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

    public Unit GetTurnUnit() {
        return unitiesOrderList[turnNumber];
    }

    private bool CheckEnemiesLeft() {
        return allEnemies.Count > 0;
    }
    private bool CheckEnemiesLeftInTheBattleZone() {
        foreach (Unit unit in unitiesOrderList) {
            if (unit.IsEnemy() && unit.GetGridPosition().zone == LevelGrid.Instance.GetCurrentBattleZone()) return true;
        }
        return false;
    }
    private bool CheckPlayerCharsLeft() {
        foreach (Unit unit in unitiesOrderList) {
            if (!unit.IsEnemy()) return true;
        }
        return false;
    }

    public void ChengeTurnSpeed() {
        if (turnSpeedIndex == turnSpeeds.Length - 1) { turnSpeedIndex = 0; }
        else turnSpeedIndex++;

        Time.timeScale = turnSpeeds[turnSpeedIndex];
    }

    public void ResetTurnSpeed() {
        turnSpeedIndex = 0;
        Time.timeScale = turnSpeeds[turnSpeedIndex];
    }




    //test
    public Unit GetPlayerUnitToExplore() {
        foreach (Unit unit in unitiesOrderList) {
            if (!unit.IsEnemy()) {
                return unit;
            }
        }
        return null;
    }
    public void SetCameraController(CameraController controller) { cameraController = controller; }
}
