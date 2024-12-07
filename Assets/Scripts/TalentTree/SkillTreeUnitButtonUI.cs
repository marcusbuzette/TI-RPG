using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTreeUnitButtonUI : MonoBehaviour {

    private string unitId;
    private string unitName;
    [SerializeField] private TMP_Text buttonText;


    public void SetUnitData(string unitId, string unitName) {
        this.unitId = unitId;
        this.unitName = unitName;
        this.buttonText.text = unitName;
    }

    public string GetUnitId() {return unitId;}
    public string GetUnitName() {return unitName;}
}
