using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour {
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private Transform unitsOrderContainer;
    [SerializeField] private Transform unitOrderUIPrefab;

    private void Start() {
        endTurnButton.onClick.AddListener(() => {
            TurnSystem.Instance.NextTurn();
        });
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
        TurnSystem.Instance.onOrderChange += TurnSystem_OnOrderChange;
        UpdatedTurnText();
        CreateUnitActionButtons();
        UpdateEndTurnButton();
    }

    private void UpdatedTurnText() {
        turnNumberText.text = "TURNO: " + TurnSystem.Instance.GetTurnNumber();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        UpdatedTurnText();
        CreateUnitActionButtons();
        UpdateEndTurnButton();
    }
    private void TurnSystem_OnOrderChange(object sender, EventArgs e) {
        CreateUnitActionButtons();
    }

    private void CreateUnitActionButtons() {
        foreach (Transform uinitOrderTransform in unitsOrderContainer) {
            Destroy(uinitOrderTransform.gameObject);
        }

        for (int i = 0; i < TurnSystem.Instance.GetTurnOrder().Count; i++) {
            Transform unitOrderTransform = Instantiate(unitOrderUIPrefab, unitsOrderContainer);
            unitOrderTransform.GetComponent<UnitOrderUI>().SetUnitOrderUI(TurnSystem.Instance.GetTurnOrder()[i], i ==0);
        }

 
    }

    private void UpdateEndTurnButton() {
        endTurnButton.interactable = TurnSystem.Instance.IsPlayerTurn();
    }
}
