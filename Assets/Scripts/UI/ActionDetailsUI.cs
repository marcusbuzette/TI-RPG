using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ActionDetailsUI : MonoBehaviour {

    [SerializeField] private Image actionImage;
    [SerializeField] private TMP_Text actionTile;
    [SerializeField] private TMP_Text actionDescription;


    public void SetActionDetails(BaseAction baseAction) {
        actionImage.sprite = baseAction.GetActionImage();
        actionTile.text = baseAction.GetActionName();
        actionDescription.text = baseAction.descricao;
    }
   
}
