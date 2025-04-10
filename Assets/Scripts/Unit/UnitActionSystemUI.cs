using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour {

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform itemButtonPrefab;
    [SerializeField] private Transform actionButtonsContainer;
    [SerializeField] private Transform attackButtonsContainer;
    [SerializeField] private Transform skillsButtonsContainer;
    [SerializeField] private Transform inventoryButtonsContainer;

    private Transform inventoyButton;

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
        Unit.OnAnyActionPerformed += Unit_OnAnyActionPerformed;
        UnitActionSystem.Instance.OnInventoryClicked += UnitActionSystem_OnInventoryClicked;
        LevelGrid.Instance.OnGameModeChanged += LevelGrid_OnGameModeChanged;

        CreateUnitActionButtons();
        this.UpdateStatus();
    }


    private void CreateUnitActionButtons() {
        foreach (Transform buttonTransform in actionButtonsContainer) {
            Destroy(buttonTransform.gameObject);
        }

        foreach (Transform buttonTransform in attackButtonsContainer) {
            Destroy(buttonTransform.gameObject);
        }

        foreach (Transform buttonTransform in skillsButtonsContainer) {
            Destroy(buttonTransform.gameObject);
        }

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit == null) return;
        foreach (BaseAction action in selectedUnit.GetActionsArray()) {

            Transform parentAux;

            switch (action.GetActionType()) {
                case ActionType.ACTION:
                    parentAux = attackButtonsContainer;
                    break;
                case ActionType.MOVE:
                    parentAux = actionButtonsContainer;
                    break;
                case ActionType.INVENTORY:
                    parentAux = actionButtonsContainer;
                    break;
                case ActionType.SKILL:
                    parentAux = skillsButtonsContainer;
                    break;
                default:
                    parentAux = actionButtonsContainer;
                    break;
            }

            Transform actioonButtonTransform = Instantiate(actionButtonPrefab, parentAux);
            actioonButtonTransform.GetComponent<ActionButtonUI>().SetBaseAction(action);
            if (((selectedUnit.GetHasMoved() && action.GetActionType() == ActionType.MOVE) || !selectedUnit.IsUnityTurn())
            || (selectedUnit.GetHasPerformedAction() && action.GetActionType() == ActionType.ACTION) || !selectedUnit.IsUnityTurn()
            || (selectedUnit.GetHasPerformedSkill() && action.GetActionType() == ActionType.SKILL)
            || (action.GetOnCooldown() && action.GetActionType() == ActionType.SKILL) || !selectedUnit.IsUnityTurn()) {
                actioonButtonTransform.GetComponent<ActionButtonUI>().DisableActionButton();
            }

            if (action.GetActionType() == ActionType.INVENTORY) {
                this.inventoyButton = actioonButtonTransform;
                if (InventorySystem.inventorySystem.IsEmpty()) {
                    actioonButtonTransform.GetComponent<ActionButtonUI>().DisableActionButton();
                }
            }
        }
    }

    public void InventoryClick() {
        if (inventoryButtonsContainer.gameObject.activeSelf) {
            CloseInventory();
        }
        else {
            OpenInventory();
        }

    }

    private void CloseInventory() {
        foreach (Transform itemTransform in inventoryButtonsContainer) {
            Destroy(itemTransform.gameObject);
        }
        inventoryButtonsContainer.gameObject.SetActive(false);
    }

    private void OpenInventory() {
        Vector3 posAux = inventoryButtonsContainer.transform.position;
        posAux.x = inventoyButton.transform.position.x;
        inventoryButtonsContainer.transform.position = posAux;
        inventoryButtonsContainer.gameObject.SetActive(true);

        foreach (KeyValuePair<InventoryItemData, SerializableInventoryItem> item in InventorySystem.inventorySystem.GetInventoryContent()) {
            Transform itemButtonTransform = Instantiate(itemButtonPrefab, inventoryButtonsContainer);
            itemButtonTransform.GetComponent<ItemButtonUI>().SetBaseAction(item.Value.data.prefab.GetComponent<ItemAction>(), item.Value.stackSize);
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) {
        CreateUnitActionButtons();
        CloseInventory();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e) {
        CreateUnitActionButtons();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        CreateUnitActionButtons();
        CloseInventory();
    }
    private void Unit_OnAnyActionPerformed(object sender, EventArgs e) {
        CreateUnitActionButtons();
        CloseInventory();
    }

    private void UnitActionSystem_OnInventoryClicked(object sender, EventArgs e) {
        InventoryClick();
    }

    private void OnDestroy() {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnActionStarted -= UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.onTurnChange -= TurnSystem_OnTurnChange;
        Unit.OnAnyActionPerformed -= Unit_OnAnyActionPerformed;
        UnitActionSystem.Instance.OnInventoryClicked -= UnitActionSystem_OnInventoryClicked;
        LevelGrid.Instance.OnGameModeChanged -= LevelGrid_OnGameModeChanged;
    }

    private void LevelGrid_OnGameModeChanged(object sender, EventArgs e) {
        this.UpdateStatus();
    }

    private void UpdateStatus() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE ? true : false);
        }
    }
}
