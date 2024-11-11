using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Unit : MonoBehaviour {

    public static event EventHandler OnAnyActionPerformed;
    public static event EventHandler OnAnyUnitSpawn;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    [SerializeField] private UnitStats unitStats;
    private BaseAction[] actionsArray;
    private bool hasMoved = false;
    private bool hasPerformedAction = false;
    public bool isUnitTurn = false;

    private void Awake() {
        actionsArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
        // unitStats = GetComponent<UnitStats>();
    }

    private void Start() {
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
        switch (action.GetActionType()) {
            case ActionType.MOVE:
                return !hasMoved;
            case ActionType.ACTION:
                return !hasPerformedAction;
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
                hasMoved = true;
                break;
        }
        OnAnyActionPerformed.Invoke(this, EventArgs.Empty);
    }

    public bool GetHasMoved() { return hasMoved; }
    public bool GetHasPerformedAction() { return hasPerformedAction; }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        if ((IsEnemy() && isUnitTurn) || (!IsEnemy() && isUnitTurn)) {
            hasMoved = false;
            hasPerformedAction = false;
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
        UnitActionSystem.Instance.ChangeSelectedUnit(this);
        OnAnyActionPerformed.Invoke(this, EventArgs.Empty);

    }
}
