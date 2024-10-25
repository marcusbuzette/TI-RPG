using System;
using Unity.VisualScripting;
using UnityEngine;


public class Unit : MonoBehaviour {

    public static event EventHandler OnAnyActionPerformed;
    public static event EventHandler OnAnyUnitSpawn;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private ShootAction shootAction;
    private BaseAction[] actionsArray;
    private bool hasMoved = false;
    private bool hasPerformedAction = false;

    private void Awake() {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        shootAction = GetComponent<ShootAction>();
        actionsArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
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

    public MoveAction GetMoveAction() {
        return moveAction;
    }
    public SpinAction GetSpinAction() {
        return spinAction;
    }
    public ShootAction GetShootAction() {
        return shootAction;
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
        } else {
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

    public bool GetHasMoved() {return hasMoved;}
    public bool GetHasPerformedAction() {return hasPerformedAction;}

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        if  (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()) ){
        hasMoved = false;
        hasPerformedAction = false;
        OnAnyActionPerformed.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damage) {
        healthSystem.Damage(damage);
    }

    private void HealthSystem_OnDie(object sender, EventArgs e) {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() {
        return healthSystem.GetHealthPointsNormalized();
    }
}
