using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemButtonUI : ActionButtonUI {

    [SerializeField] protected TextMeshProUGUI quantityText;

    public void SetBaseAction(ItemAction itemAction, int quantity) {
        base.SetBaseAction(itemAction);
        quantityText.text = quantity.ToString();
    }
    
}
