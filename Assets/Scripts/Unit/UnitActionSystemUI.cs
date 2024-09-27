using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour {

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonsContainer;

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnicActionSystem_OnSelectedUnitChanged;

        CreateUnitActionButtons();
    }


    private void CreateUnitActionButtons() {
        foreach(Transform buttonTransform in actionButtonsContainer) {
            Destroy(buttonTransform.gameObject);
        }

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction action in selectedUnit.GetActionsArray()) {

            Transform actioonButtonTransform = Instantiate(actionButtonPrefab, actionButtonsContainer);
            actioonButtonTransform.GetComponent<ActionButtonUI>().SetBaseAction(action);
        }
    }

    private void UnicActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) {
        CreateUnitActionButtons();
    }
}
