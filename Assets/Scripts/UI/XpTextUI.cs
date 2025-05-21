using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XpTextUI : MonoBehaviour {

    [SerializeField] private string unitId;
    private TMP_Text xpTextField;

    void Start() {
        xpTextField = GetComponent<TMP_Text>();
        foreach (Unit unit in TurnSystem.Instance.GetUnitsOrderList()) {
            if (unit.unitId == this.unitId) {
                xpTextField.text = 
                "+" + unit.GetUnitXpSystem().getLevelXpAmount().ToString() + " XP";
            }
        }
        
    }

}
