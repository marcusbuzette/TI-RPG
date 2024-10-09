using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour {
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;

    private void Start() {
        endTurnButton.onClick.AddListener(() => {
            TurnSystem.Instance.NextTurn();
        });
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;

        UpdatedTurnText();
    }

    private void UpdatedTurnText() {
        turnNumberText.text = "TURNO: " + TurnSystem.Instance.GetTurnNumber();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        UpdatedTurnText();
    }
}
