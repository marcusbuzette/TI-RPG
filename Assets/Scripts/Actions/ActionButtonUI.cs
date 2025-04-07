using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] protected TextMeshProUGUI textMeshPro;
    private Sprite actionImage;
    [SerializeField] protected Button  button;
    [SerializeField] private Color  NORMAL_COLOR;
    [SerializeField] private Color  SELECTED_COLOR;
    [SerializeField] private ActionType actionType;

    private string name;
    private string description;

    public void SetBaseAction(BaseAction baseAction) {
        textMeshPro.text = baseAction.GetActionName().ToUpper();
        this.name = baseAction.GetActionName();
        this.actionImage = baseAction.GetActionImage();

        if (actionImage != null) {
            textMeshPro.enabled = false;
            this.button.GetComponent<Image>().sprite = this.actionImage;
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
    }

    public void OnPointerExit(PointerEventData eventData) {
        Tooltip.Instance.HideTooltip();
    }
}
