using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour {

    [SerializeField] private int turnNumber = 0;
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private List<Unit> unitiesOrderList = new List<Unit>();
    private List<Unit> allEnemies = new List<Unit>();

    public static TurnSystem Instance { get; private set; }
    public event EventHandler onTurnChange;
    public event EventHandler onOrderChange;
    public event EventHandler onEnemyKilled;
    private CameraController cameraController;

    [SerializeField] private int[] turnSpeeds;
    private int turnSpeedIndex;
    private bool isOnCombo = false;

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
        BaseAction.OnAnyActionCompleted += FinishTurnAuto;
        unitiesOrderList = FindObjectsOfType<Unit>(false)
            .Where(unit => unit.GetGridPosition().zone == LevelGrid.Instance.GetCurrentBattleZone()).
            Where(unit => unit.GetHealthSystem().GetHealthState() == HealthSystem.HealthState.ALIVE).ToList<Unit>();
        allEnemies = FindObjectsOfType<Unit>(false).Where(unit => unit.IsEnemy()).ToList<Unit>();
        if (LevelGrid.Instance.IsInBattleMode()) unitiesOrderList.Sort((x, y) => y.GetUnitSpeed().CompareTo(x.GetUnitSpeed()));
        isPlayerTurn = !unitiesOrderList[turnNumber].IsEnemy();
        if (LevelGrid.Instance.IsInBattleMode()) unitiesOrderList[turnNumber].StartUnitTurn();
        onOrderChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetUpBattleNewZone() {
        List<Unit> playerUnits = FindObjectsOfType<Unit>(false).
            Where(unit => unit.IsEnemy() == false).
            Where(unit => unit.GetHealthSystem().GetHealthState() == HealthSystem.HealthState.ALIVE).ToList<Unit>();
        for (int i = 0; i < playerUnits.Count; i++) {
            UnitActionSystem.Instance.MoveUnitToGridPosition(playerUnits[i],
            LevelGrid.Instance.GetZoneSpawnList(LevelGrid.Instance.GetCurrentBattleZone())[i]);
        }

    }

    public void StartBattleNewZone() {
        turnNumber = 0;
        unitiesOrderList = FindObjectsOfType<Unit>(false)
            .Where(unit => (unit.GetGridPosition().zone == LevelGrid.Instance.GetCurrentBattleZone() || !unit.IsEnemy())).
            Where(unit => unit.GetHealthSystem().GetHealthState() == HealthSystem.HealthState.ALIVE).ToList<Unit>();
        unitiesOrderList.Sort((x, y) => y.GetUnitSpeed().CompareTo(x.GetUnitSpeed()));
        isPlayerTurn = !unitiesOrderList[turnNumber].IsEnemy();
        unitiesOrderList[turnNumber].StartUnitTurn();
        onOrderChange.Invoke(this, EventArgs.Empty);
    }

    public void FinishTurnAuto(object sender, EventArgs e) {

        var unitAction = (sender as BaseAction)?.GetUnit();

        if (unitAction != null) {
            if (unitAction.isUnitTurn) {
                if (!unitAction.IsEnemy() && unitAction.CanFinishRound()) {
                    NextTurn();
                }
            }
        }
    }

    public void NextTurn() {
        Unit currentUnit = unitiesOrderList[turnNumber];
        // Se usou ataque rápido, avança na fila antes de prosseguir
        if (currentUnit.HasUsedQuickAttack()) {
            AdvanceTurnToMiddleCircular(currentUnit);  // avanca o turno do presonagem para o meio da fila
            currentUnit.ClearQuickAttackFlag();
        }
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

    IEnumerator ComboKill() {

        yield return new WaitForSeconds(0.5f);

        if (turnNumber == 0) turnNumber = unitiesOrderList.Count - 1;
        else turnNumber--;

        Debug.Log(turnNumber);
        
        isPlayerTurn = !unitiesOrderList[turnNumber].IsEnemy();
        onTurnChange.Invoke(this, EventArgs.Empty);
        unitiesOrderList[turnNumber].StartUnitTurn();

        Vector3 unitTurnTransform = unitiesOrderList[turnNumber].transform.position;
        if (isPlayerTurn) { cameraController.LockCameraOnSelectedUnit(unitiesOrderList[turnNumber]); }
        else cameraController.GoToPosition(unitTurnTransform);

        yield return null;
    }

    public int GetTurnNumber() { return turnNumber; }

    public void RemoveUnitFromList(Unit unitDead) {

        int unitDeadIndex = unitiesOrderList.FindIndex((u) => u.transform == unitDead.transform);
        if (unitDead.IsEnemy()) {
            // this.unitiesOrderList[this.turnNumber]
            //     .AddXp(this.unitiesOrderList[unitDeadIndex].GetUnitStats().GetXpSpoil());
            onEnemyKilled.Invoke(unitDead, EventArgs.Empty);
            allEnemies.Remove(unitDead);
        }
        unitiesOrderList.Remove(unitDead);
        if (turnNumber > unitDeadIndex) { turnNumber--; }
        if (isPlayerTurn && CheckEnemiesLeftInTheBattleZone()) {
            StartCoroutine(ComboKill());
        }
        else if (isPlayerTurn && !CheckEnemiesLeftInTheBattleZone() && CheckEnemiesLeft()) {
            LevelGrid.Instance.RemoveZoneFromGrid(LevelGrid.Instance.GetCurrentBattleZone());
            List<Unit> playerUnits = FindObjectsOfType<Unit>(false).Where(unit => unit.IsEnemy() == false).
                Where(unit => unit.GetHealthSystem().GetHealthState() == HealthSystem.HealthState.ALIVE).ToList<Unit>();
            foreach (Unit unit in playerUnits) {
                unit.UpdateGridPositionZone(0);
            }

            InstantiateRewardChest(unitDead.transform);

            LevelGrid.Instance.ExploreMode();
        }
        else if (!isPlayerTurn && !CheckPlayerCharsLeft()) {
            ResetTurnSpeed();
            GameController.controller.GameOver();
        }
        else if (isPlayerTurn && !CheckEnemiesLeft()) {
            ResetTurnSpeed();
            LevelGrid.Instance.ExploreMode();
            // foreach (Unit u in unitiesOrderList) {
            //     if (!u.IsEnemy()) u.AddXp(2);
            // }

            // GameController.controller.NextLevel();
            // SceneManager.LoadScene("HUB");
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

    public void AdvanceTurnToMiddleCircular(Unit unitToAdvance) {
        int currentIndex = unitiesOrderList.IndexOf(unitToAdvance);
        if (currentIndex == -1) return;

        // Remove a unidade da lista
        unitiesOrderList.RemoveAt(currentIndex);

        // Cria uma lista circular dos futuros turnos
        List<Unit> futureUnits = new List<Unit>();

        // Adiciona de turnNumber + 1 até o final
        for (int i = turnNumber + 1; i < unitiesOrderList.Count; i++) {
            futureUnits.Add(unitiesOrderList[i]);
        }
        // Adiciona do começo até turnNumber (sem incluir quem está jogando agora)
        for (int i = 0; i < turnNumber; i++) {
            futureUnits.Add(unitiesOrderList[i]);
        }

        // Calcula o ponto central mais próximo do começo
        int middleOffset = Mathf.FloorToInt(futureUnits.Count / 2f);

        Unit insertAfter = futureUnits[middleOffset % futureUnits.Count];
        int insertIndex = unitiesOrderList.IndexOf(insertAfter);

        if (insertIndex == -1) {
            // Fallback se algo estranho acontecer
            insertIndex = (turnNumber + 1) % unitiesOrderList.Count;
        }
        else {
            insertIndex = (insertIndex + 1) % (unitiesOrderList.Count + 1); // inserir após
        }

        unitiesOrderList.Insert(insertIndex, unitToAdvance);

        // Ajusta turnNumber se necessário
        if (currentIndex < turnNumber) {
            turnNumber--;
            if (turnNumber < 0) turnNumber += unitiesOrderList.Count;
        }

        onOrderChange?.Invoke(this, EventArgs.Empty);
    }

    //test
    public Unit GetPlayerUnitToExplore() {
        Unit tryTofindHero = unitiesOrderList.Find((u) => u.unitId == "hero");
        if (tryTofindHero != null) return tryTofindHero;
        foreach (Unit unit in unitiesOrderList) {
            if (!unit.IsEnemy()) {
                return unit;
            }
        }
        return null;
    }

    private void InstantiateRewardChest(Transform chestTransform) {
        var chest = Instantiate(Resources.Load<GameObject>("Prefabs_R/Chest"), chestTransform.position, chestTransform.rotation);
        chest.GetComponent<Chest>().AddItens(GetRewardChestItems(), (100 * unitiesOrderList.Count));
        PathFinding.Instance.SetNodeIsWalkable(chest.transform.position, false);
    }

    private List<InventoryItemData> GetRewardChestItems() {
        List<InventoryItemData> listItems = new List<InventoryItemData>();
        int quality = unitiesOrderList.Count;
        int dice = UnityEngine.Random.Range(0, 100);

        switch (quality) {
            case 1:
                //Revive
                if(dice <= 30) {
                    if (dice <= 10) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));

                //Great Potion
                if (dice <= 40) {
                    if (dice <= 20) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                    }
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                }

                //Medium Potion
                if (dice <= 60) {
                    if (dice <= 50) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                    if(dice <= 20) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));

                //Basic Postion
                if (dice <= 70) {
                    if (dice <= 40) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));

                break;

            case 2:
                //Revive
                if (dice <= 40) {
                    if (dice <= 20) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));

                //Great Potion
                if (dice <= 50) {
                    if (dice <= 30) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                    }
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                }

                //Medium Potion
                if (dice <= 70) {
                    if (dice <= 60) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                    if (dice <= 30) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));

                //Basic Postion
                if (dice <= 80) {
                    if (dice <= 50) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));

                break;

            case 3:
                //Revive
                if (dice <= 50) {
                    if (dice <= 30) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));

                //Great Potion
                if (dice <= 60) {
                    if (dice <= 40) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                    }
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                }

                //Medium Potion
                if (dice <= 80) {
                    if (dice <= 70) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                    if (dice <= 40) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));

                //Basic Postion
                if (dice <= 85) {
                    if (dice <= 65) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));

                break;

            case 4:
                //Revive
                if (dice <= 50) {
                    if (dice <= 30) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_Revive"));

                //Great Potion
                if (dice <= 80) {
                    if (dice <= 40) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                    }
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_GreatPotion"));
                }

                //Medium Potion
                if (dice <= 85) {
                    if (dice <= 60) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                    if (dice <= 40) {
                        listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                    }
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_MediumPotion"));

                //Basic Postion
                if (dice <= 85) {
                    if (dice <= 65) listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                    listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                }
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));
                listItems.Add(Resources.Load<InventoryItemData>("InventoryItemData_R/InventoryItem_BasicPotion"));

                break;
        }
        return new List<InventoryItemData>();
    }

    public void SetCameraController(CameraController controller) { cameraController = controller; }
    public CameraController GetCameraController() { return cameraController; }

    public List<Unit> GetUnitsOrderList() { return this.unitiesOrderList; }
}
