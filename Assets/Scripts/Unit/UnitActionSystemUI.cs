using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour {

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonsContainer;

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
        Unit.OnAnyActionPerformed += Unit_OnAnyActionPerformed;

        CreateUnitActionButtons();
    }


    private void CreateUnitActionButtons() {
        Debug.Log(actionButtonsContainer);
        foreach (Transform buttonTransform in actionButtonsContainer) {
            Destroy(buttonTransform.gameObject);
        }

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit == null) return;

        foreach (BaseAction action in selectedUnit.GetActionsArray()) {

            Transform actioonButtonTransform = Instantiate(actionButtonPrefab, actionButtonsContainer);
            actioonButtonTransform.GetComponent<ActionButtonUI>().SetBaseAction(action);
            if (((selectedUnit.GetHasMoved() && action.GetActionType() == ActionType.MOVE) || !selectedUnit.IsUnityTurn()) 
            || (selectedUnit.GetHasPerformedAction() && action.GetActionType() == ActionType.ACTION) || !selectedUnit.IsUnityTurn()) {
                actioonButtonTransform.GetComponent<ActionButtonUI>().DisableActionButton();
            }
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) {
        CreateUnitActionButtons();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e) {
        CreateUnitActionButtons();
    }

    private void TurnSystem_OnTurnChange (object sender, EventArgs e) {
        CreateUnitActionButtons();
    }
    private void Unit_OnAnyActionPerformed (object sender, EventArgs e) {
        CreateUnitActionButtons();
    }

    private void OnDestroy() {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnActionStarted -= UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.onTurnChange -= TurnSystem_OnTurnChange;
        Unit.OnAnyActionPerformed -= Unit_OnAnyActionPerformed;
    }
}
