using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour {
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button turnSpeedButton;
    [SerializeField] private TextMeshProUGUI turnSpeedText;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private Transform unitsOrderContainer;
    [SerializeField] private Transform unitOrderUIPrefab;

    private void Start() {
        endTurnButton.onClick.AddListener(() => {
            TurnSystem.Instance.NextTurn();
        });
        turnSpeedButton.onClick.AddListener(() => {
            TurnSystem.Instance.ChengeTurnSpeed();
            UpdateTurnSpeedText();

        });
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;
        TurnSystem.Instance.onOrderChange += TurnSystem_OnOrderChange;
        LevelGrid.Instance.OnGameModeChanged += LevelGrid_OnGameModeChanged;
        UpdatedTurnText();
        CreateUnitActionButtons();
        UpdateEndTurnButton();
        this.UpdateStatus();
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
            unitOrderTransform.GetComponent<UnitOrderUI>().SetUnitOrderUI(TurnSystem.Instance.GetTurnOrder()[i], i == 0);
        }
    }

    private void UpdateEndTurnButton() {
        endTurnButton.interactable = TurnSystem.Instance.IsPlayerTurn();
    }

    public void UpdateTurnSpeedText() {
        turnSpeedText.text = Time.timeScale.ToString() + "x";
    }

    private void LevelGrid_OnGameModeChanged(object sender, EventArgs e) {
        this.UpdateStatus();
    }

    void OnDestroy() {
        LevelGrid.Instance.OnGameModeChanged -= LevelGrid_OnGameModeChanged;
        TurnSystem.Instance.onTurnChange -= TurnSystem_OnTurnChange;
        TurnSystem.Instance.onOrderChange -= TurnSystem_OnOrderChange;
    }

    private void UpdateStatus() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE ? true : false);
        }
    }
}
