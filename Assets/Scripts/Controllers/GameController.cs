using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController controller;
    public UIController uicontroller;
    public int dinheiro;
    private Dictionary<string, UnitRecords> playerUnits = new Dictionary<string, UnitRecords>();

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
        // UnitStats statsAux = new UnitStats(0,0,0,0,0);
        // playerUnits.Add("monkey", new UnitRecords(0,statsAux));
        // playerUnits.Add("archer", new UnitRecords(0,statsAux));
        // TalentManager.Instance.OnSelectedUnitChanged("monkey");
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
        UnitRecords unitRecordsAux = new UnitRecords(unit.GetUnitXpSystem().getXpAmount(), unit.GetUnitStats());
        playerUnits.Add(unit.GetUnitId(), unitRecordsAux);
    }

    public UnitRecords GetUnitRecords(string unitId) { return playerUnits[unitId]; }
    public void UpdateUnitRecords(Unit unit) {
        Debug.Log("asd");
        List<BaseSkills> skillsAux = playerUnits[unit.GetUnitId()].GetUnitSKills().Count > 0 ? playerUnits[unit.GetUnitId()].GetUnitSKills() : null;
        UnitRecords unitRecordsAux = new UnitRecords(unit.GetUnitXpSystem().getXpAmount(), unit.GetUnitStats(),
                                                        skillsAux);
        playerUnits[unit.GetUnitId()] = unitRecordsAux;
    }

    public void AddSkillToRecordById(string unitId,BaseSkills skill){
        playerUnits[unitId].AddSkill(skill);
    }

    public Dictionary<string, UnitRecords>.KeyCollection playerUnitsIds() {
        return playerUnits.Keys;
    }

}
