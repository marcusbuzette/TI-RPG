using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button  button;
    [SerializeField] private Color  NORMAL_COLOR;
    [SerializeField] private Color  SELECTED_COLOR;
    [SerializeField] private ActionType actionType;

    public void SetBaseAction(BaseAction baseAction) {
        textMeshPro.text = baseAction.GetActionName().ToUpper();

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






}
