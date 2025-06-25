using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour {
    [SerializeField] private Button turnSpeedButton;
    [SerializeField] private TextMeshProUGUI turnSpeedText;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private Transform unitsOrderContainer;
    [SerializeField] private Transform unitOrderUIPrefab;

    [Header("Finalizar turno")]
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Vector2 originalPosition;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private float moveDuration = 0.5f;

    private Coroutine moveCoroutine;

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
        if(LevelGrid.Instance.IsInBattleMode() && TurnSystem.Instance.GetTurnUnit() != null) {
            if(TurnSystem.Instance.GetTurnUnit().IsEnemy()) {
                HideEndTurnButton();
            }
            else {
                ShowEndTurnButton();
            }
        }
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

    private void ShowEndTurnButton() {
        StartMovingTo(originalPosition);
    }

    private void HideEndTurnButton() {
        StartMovingTo(hiddenPosition);
    }

    private void StartMovingTo(Vector2 targetPosition) {
        // Para qualquer movimento anterior
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }

        // Começa a nova animação
        moveCoroutine = StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector2 target) {
        Vector2 start = endTurnButton.GetComponent<RectTransform>().anchoredPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            endTurnButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        endTurnButton.GetComponent<RectTransform>().anchoredPosition = target;
        moveCoroutine = null;
    }
}