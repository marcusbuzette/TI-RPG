using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController controller;
    public UIController uicontroller;
    public int dinheiro;
    private Dictionary<string, UnitRecords> playerUnits = new Dictionary<string, UnitRecords>();
    public GameObject MonkeyPrefab;

    [SerializeField] private bool debugMode = false;
    [SerializeField] private bool debugPathFindingMode = false;

    private void Awake() {
        if (controller == null) {
            controller = this;
            DontDestroyOnLoad(this);
        }
        else {
            DestroyImmediate(gameObject);
        }
        dinheiro = 1000;
    }

    void Start(){ 
        AddUnitToRecords(MonkeyPrefab.GetComponent<Unit>());
    }


    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public bool GetDebugMode() { return this.debugMode;}
    public bool GetPathFindingDebugMode() { return this.debugPathFindingMode; }
    public bool HasUnitRecords (string unitId) {
        return playerUnits.ContainsKey(unitId);
        }
    public void AddUnitToRecords(Unit unit) {
        UnitRecords unitRecordsAux = new UnitRecords(unit.GetUnitXpSystem().getXpAmount(), unit.GetUnitXpStats());
        playerUnits.Add(unit.GetUnitId(), unitRecordsAux);
    }

    public UnitRecords GetUnitRecords(string unitId) { return playerUnits[unitId]; }
    public void UpdateUnitRecords(Unit unit) {
        UnitRecords unitRecordsAux = new UnitRecords(unit.GetUnitXpSystem().getXpAmount(), unit.GetUnitXpStats());
        playerUnits[unit.GetUnitId()] = unitRecordsAux;
    }

    public void UpdateUnitRecordsByID(string unitId,BaseSkills skill){
        playerUnits[unitId].AddSkill(skill);
    }

}
