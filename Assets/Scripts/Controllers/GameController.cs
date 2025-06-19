using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class GameController : MonoBehaviour, IDataPersistence {

    public static GameController controller;
    public UIController uicontroller;
    public int dinheiro;
    [SerializeField] private int currentLevel = 0;
    private SerializableDictionary<string, UnitRecords> playerUnits = new SerializableDictionary<string, UnitRecords>();

    [SerializeField] private bool debugMode = false;
    [SerializeField] private bool debugPathFindingMode = false;

    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;


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

    void Start() {
        // UnitStats statsAux = new UnitStats(0,0,0,0,0);
        // playerUnits.Add("monkey", new UnitRecords(0,statsAux));
        // playerUnits.Add("archer", new UnitRecords(0,statsAux));
        // TalentManager.Instance.OnSelectedUnitChanged("monkey");
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    public bool GetDebugMode() { return this.debugMode; }
    public bool GetPathFindingDebugMode() { return this.debugPathFindingMode; }
    public bool HasUnitRecords(string unitId) {
        return playerUnits.ContainsKey(unitId);
    }
    public void AddUnitToRecords(Unit unit) {
        UnitRecords unitRecordsAux = new UnitRecords(unit.GetUnitXpSystem().getXpAmount(), unit.GetBaseUnitStats());
        playerUnits.Add(unit.GetUnitId(), unitRecordsAux);
    }

    public UnitRecords GetUnitRecords(string unitId) {
        return playerUnits[unitId];
    }
    public void UpdateUnitRecords(Unit unit) {
        List<BaseSkills> skillsAux = playerUnits[unit.GetUnitId()].GetUnitSKills().Count > 0 ? playerUnits[unit.GetUnitId()].GetUnitSKills() : null;
        UnitRecords unitRecordsAux = new UnitRecords(unit.GetUnitXpSystem().getXpAmount(), unit.GetUnitStats(),
                                                        skillsAux);
        playerUnits[unit.GetUnitId()] = unitRecordsAux;
    }

    public void AddSkillToRecordById(string unitId, BaseSkills skill) {
        playerUnits[unitId].AddSkill(skill);
    }

    public void AddUpgradeToRecordsById(string unitId, PossibleUpgrade upgrade, int index) {
        playerUnits[unitId].AddLevelUpgrade(upgrade.level, index, upgrade.upgrade[index]);
    }

    public Dictionary<string, UnitRecords>.KeyCollection playerUnitsIds() {
        return playerUnits.Keys;
    }

    public void GameOver() {
        Debug.Log("Derrota");
        uicontroller.ChangeScene("GameOver");
    }

    public int GetCurrentLevel() { return this.currentLevel; }

    public void NextLevel() {
        this.currentLevel++;
        DataPersistenseManager.instace?.SaveGame();
    }

    public void AddMoney(int money) {
        this.dinheiro += money;
    }

    public void LoadData(GameData data) {
        this.currentLevel = data.currentLevel;
        this.dinheiro = data.money;
        if (data.playerUnits.Count > 0) {
            this.playerUnits = data.playerUnits;
            //foreach (KeyValuePair<string, UnitRecords> item in this.playerUnits) {
            //TalentManager.Instance.UpdateLocalUnitValues(item.Key, item.Value);
            //}
        }
    }

    public void SaveData(ref GameData data) {
        data.currentLevel = this.currentLevel;
        data.money = this.dinheiro;
        data.playerUnits = this.playerUnits;

    }

    public void TogglePause() {
        if (isPaused) {
            ResumeGame();
        }
        else {
            PauseGame();
        }
    }

    public void PauseGame() {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenuUI == null) {
            GameObject pausePrefab = Resources.Load<GameObject>("UIPrefabs_R/PauseMenu");
            GameObject canvasAux = GameObject.FindGameObjectWithTag("UICanvas"); 
            if (pausePrefab != null && canvasAux != null) {
                pauseMenuUI = Instantiate(pausePrefab, canvasAux.transform);
            } else {
                Debug.Log("Erro ao instanciar o menu de pausa");
            }
        } else {
            pauseMenuUI?.SetActive(true);
        }
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI?.SetActive(false);
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    public bool IsPaused() => isPaused;

}
