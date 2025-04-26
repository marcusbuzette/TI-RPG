using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] protected TextMeshProUGUI textMeshPro;
    private Sprite actionImage;
    [SerializeField] protected Button button;
    [SerializeField] private Color NORMAL_COLOR;
    [SerializeField] private Color SELECTED_COLOR;
    [SerializeField] private ActionType actionType;

    private string name;
    private string description;

    private Vector3 originalScale;
    private Vector3 targetScale;
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float speed = 10f;
    private Coroutine scaleCoroutine;

    public void SetBaseAction(BaseAction baseAction) {
        originalScale = transform.localScale;
        targetScale = originalScale;
        textMeshPro.text = baseAction.GetActionName().ToUpper();
        this.name = baseAction.GetActionName();
        this.actionImage = baseAction.GetActionImage();

        if (actionImage != null) {
            textMeshPro.enabled = false;
            this.button.GetComponent<Image>().sprite = this.actionImage;
            this.button.GetComponent<Image>().color = Color.white;
        }

        button.onClick.AddListener(() => {
            SelectAction();
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }


    public void SelectAction() {
        GetComponent<Outline>().effectColor = SELECTED_COLOR;
    }
    public void UnselectAction() {
        GetComponent<Outline>().effectColor = NORMAL_COLOR;
    }
    public void EnableActionButton() {
        button.interactable = true;
    }
    public void DisableActionButton() {
        button.interactable = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Tooltip.Instance.ShowTooltip(textMeshPro.text, transform, TooltipPosition.TOP);
        StartScaling(originalScale * scaleMultiplier);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Tooltip.Instance.HideTooltip();
        StartScaling(originalScale);
    }

    private void StartScaling(Vector3 newTargetScale) {
        targetScale = newTargetScale;

        // Se já tiver uma coroutine rodando, para ela antes
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleButton());
    }

    private IEnumerator ScaleButton() {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f) {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
            yield return null; // Espera o próximo frame
        }

        transform.localScale = targetScale; // Garante que termina exatamente no tamanho certo
    }
}
