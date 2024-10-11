using System;
using Unity.VisualScripting;
using UnityEngine;


public class Unit : MonoBehaviour {

    public static event EventHandler OnAnyActionPerformed;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] actionsArray;
    private bool hasMoved = false;
    private bool hasPerformedAction = false;

    private void Awake() {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        actionsArray = GetComponents<BaseAction>();
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
    }

    private void Update() {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        if (newGridPosition != gridPosition) {
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction() {
        return moveAction;
    }
    public SpinAction GetSpinAction() {
        return spinAction;
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
}
