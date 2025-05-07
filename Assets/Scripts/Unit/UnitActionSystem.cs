using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class UnitActionSystem : MonoBehaviour {
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler OnUnitMovedInExploreMode;

    public event EventHandler OnInventoryClicked;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private LayerMask interactiveLayerMask;

    private BaseAction selectedAction;

    private bool isBusy = false;

    private void Awake() {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Update() {
        if (isBusy) return;

        if (!TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (TryHandleUnitSelection()) return;
            HandleSelectedAction();
        }
    }

    private void HandleSelectedAction() {
        if (Input.GetMouseButtonDown(0)) {

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE) {
                if (selectedAction == null) return;
                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;

                if (selectedUnit.TryToPerformAction(selectedAction)) {
                    SetBusy();
                    selectedAction.TriggerAction(mouseGridPosition, ClearBusy);
                    OnActionStarted.Invoke(this, EventArgs.Empty);
                }
            }
            else {
                this.selectedAction = null;
                OnUnitMovedInExploreMode?.Invoke(this, EventArgs.Empty);
                if (this.selectedUnit == null || this.selectedUnit.unitId != "hero") {
                    this.selectedUnit = TurnSystem.Instance.GetPlayerUnitToExplore();
                    OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
                    OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, interactiveLayerMask)) {
                    IInteractiveObjects interactiveObject = hit.collider.GetComponent<IInteractiveObjects>();
                    interactiveObject.MoveUnitToGridPostion(selectedUnit);
                    return;
                }

                List<Unit> unitList = TurnSystem.Instance.GetUnitsOrderList();
                unitList.Remove(selectedUnit);
                selectedUnit?.GetComponent<MoveAction>().TriggerAction(mouseGridPosition, ClearBusy);

                List<Vector3> possiblePositions = LevelGrid.Instance.GetPositionsBehindUnit(selectedUnit, unitList.Count);
                for (int i = 0; i < possiblePositions.Count; i++) {
                    unitList[i].GetComponent<MoveAction>().TriggerAction(
                                    LevelGrid.Instance.GetGridPosition(possiblePositions[i]),
                                    ClearBusy
                                );

                }
            }
        }
    }

    private bool TryHandleUnitSelection() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayerMask)) {
            if (hit.transform.TryGetComponent<Unit>(out Unit unit)) {
                if (unit == selectedUnit) return false;
                if (unit.IsEnemy()) {
                    return false;
                }
                SetSelectedUnit(unit);
                return true;
            }
        }

        return false;
    }

    public void ChangeSelectedUnit(Unit unit) {
        if (unit != null && !unit.IsEnemy()) {
            SetSelectedUnit(unit);
        }
        else {
            SetSelectedUnit(null);
        }
    }

    private void SetSelectedUnit(Unit unit) {
        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE) {
            selectedUnit = unit;
            selectedAction = null;
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetSelectedAction(BaseAction action) {
        switch (selectedAction) {
            case HealArea fireAttack:
                fireAttack.isAiming = false;
                break;
        }
        selectedAction = action;
        if (action.GetActionType() != ActionType.INVENTORY) {
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }
        else {
            OnInventoryClicked.Invoke(action, EventArgs.Empty);
        }
    }

    public Unit GetSelectedUnit() { return selectedUnit; }

    public BaseAction GetSelectedAction() { return selectedAction; }

    private void SetBusy() {
        isBusy = true;
    }

    private void ClearBusy() {
        isBusy = false;
    }

    public void MoveUnitToGridPosition(Unit unit, GridPosition pos) {
        selectedUnit = unit;
        selectedAction = null;
        selectedUnit.GetComponent<MoveAction>().TriggerAction(pos, ClearBusy);
    }
}
