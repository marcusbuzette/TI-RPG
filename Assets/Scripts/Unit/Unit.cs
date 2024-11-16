using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Unit : MonoBehaviour {

    public static event EventHandler OnAnyActionPerformed;
    public static event EventHandler OnAnyUnitSpawn;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    [SerializeField] private XpSystem xpSystem;
    [SerializeField] private string unitId = "";
    [SerializeField] private UnitStats unitStats;
    [SerializeField] private BaseAction[] actionsArray;
    [SerializeField] private bool hasMoved = false;
    [SerializeField] private bool hasPerformedAction = false;
    [SerializeField] private bool hasPerformedSkill = false;
    public bool isUnitTurn = false;

    private int intimidateCoolDown = 0;
    [SerializeField] private int enemyFocus = 0;

    private void Awake() {
        actionsArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
        xpSystem = GetComponent<XpSystem>();
    }

    private void Start() {
        if(!isEnemy && GameController.controller.HasUnitRecords(unitId)) {
            UnitRecords unitRecords = GameController.controller.GetUnitRecords(unitId);
            this.xpSystem.AddXp(unitRecords.xp);
            this.unitStats = unitRecords.unitStats;
        }
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
        healthSystem.OnDead += HealthSystem_OnDie;
        OnAnyUnitSpawn?.Invoke(this, EventArgs.Empty);
    }

    private void Update() {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        if (newGridPosition != gridPosition) {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction {
        foreach(BaseAction baseAction in actionsArray) {
            if (baseAction is T) {
                return (T)baseAction;
            }
        }
        return null;
    }

    public BaseAction[] GetActionsArray() {
        return actionsArray;
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    public bool TryToPerformAction(BaseAction action) {
        if (CanTriggerAction(action)) {
            PerformAction(action);
            return true;
        }
        else {
            return false;
        }
    }

    public bool CanTriggerAction(BaseAction action) {
        Debug.Log(action.ToString());
        switch (action.GetActionType()) {
            case ActionType.MOVE:
                return !hasMoved;
            case ActionType.ACTION:
                return !hasPerformedAction;
            case ActionType.SKILL:
                return !hasPerformedSkill;
        }
        return false;
    }

    private void PerformAction(BaseAction action) {
        switch (action.GetActionType()) {
            case ActionType.MOVE:
                hasMoved = true;
                break;
            case ActionType.ACTION:
                hasPerformedAction = true;
                hasPerformedSkill = true;
                hasMoved = true;
                break;
            case ActionType.SKILL:
                hasPerformedSkill = true;
                break;
        }
        OnAnyActionPerformed.Invoke(this, EventArgs.Empty);
    }

    public bool GetHasMoved() { return hasMoved; }
    public bool GetHasPerformedAction() { return hasPerformedAction; }

    public bool GetHasPerformedSkill() { return hasPerformedSkill; }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        if ((IsEnemy() && isUnitTurn) || (!IsEnemy() && isUnitTurn)) {
            hasMoved = false;
            hasPerformedAction = false;
            hasPerformedSkill = false;
            isUnitTurn = false;
            // OnAnyActionPerformed.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy() {
        return isEnemy;
    }

    public void Damage(int damage) {
        healthSystem.Damage(damage);
    }

    public void AddXp(int xpAmount) {
        xpSystem.AddXp(xpAmount);
    }

    private void HealthSystem_OnDie(object sender, EventArgs e) {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.RemoveUnitFromList(this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() {
        return healthSystem.GetHealthPointsNormalized();
    }


    public Vector3 GetWorldPosition() { return transform.position;}

    public int GetUnitSpeed() { return unitStats.GetSpeed(); }

    public bool IsUnityTurn() { return isUnitTurn; }

    public void StartUnitTurn() {
        this.isUnitTurn = true;

        if(intimidateCoolDown != 0) {
            hasMoved = true;
            hasPerformedAction = true;
            hasPerformedSkill = true;
            intimidateCoolDown--;
        }

        if(enemyFocus != 0) {
            enemyFocus--;
        }

        for (int i = 0; i < actionsArray.Length; i++) {
            if (actionsArray[i].GetActionType() == ActionType.SKILL) {
                actionsArray[i].IsAnotherRound();
            }
        }

        UnitActionSystem.Instance.ChangeSelectedUnit(this);
        OnAnyActionPerformed.Invoke(this, EventArgs.Empty);
    }

    public string GetUnitId() { return this.unitId; }
    public XpSystem GetUnitXpSystem() { return this.xpSystem; }
    public UnitStats GetUnitXpStats() { return this.unitStats; }

    private void OnDestroy() {
        if (!isEnemy && GameController.controller.HasUnitRecords(unitId)) {
            GameController.controller.UpdateUnitRecords(this);
        }
    }

    public void BeIntimidate() {
        intimidateCoolDown = 1;
    }

    public void FocusOnMe(int focusTime) {
        enemyFocus = focusTime;
    }

    public bool GetEnemyFocus() {
        if (enemyFocus == 0) return false;
        else return true;
    }
}
