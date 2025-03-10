using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitOrderUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private Image image;
    [SerializeField] private Image healthBar;
    [SerializeField] private Color NORMAL_COLOR;
    [SerializeField] private Color TURN_COLOR;
    [SerializeField] private HealthSystem unitHealthSystem;


    public void SetUnitOrderUI(Unit unit, bool currentTurn) {
        this.name.text = unit.GetUnitName();
        this.unitHealthSystem = unit.GetComponent<HealthSystem>();
        this.healthBar.fillAmount = unitHealthSystem.GetHealthPointsNormalized();
        unitHealthSystem.OnDamage += HealthSystem_OnDamage;
        unitHealthSystem.OnDead += HealthSystem_OnDead;
        GetComponent<Image>().color = currentTurn ? TURN_COLOR : NORMAL_COLOR;
    }

    private void HealthSystem_OnDamage(object sender, EventArgs e) {
        // Debug.Log(this.healthBar);
        if (unitHealthSystem != null) {
            this.healthBar.fillAmount = unitHealthSystem.GetHealthPointsNormalized();
        }
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) {
        Destroy(gameObject);
    }

    private void OnDestroy() {
        unitHealthSystem.OnDamage -= HealthSystem_OnDamage;
        unitHealthSystem.OnDead -= HealthSystem_OnDead;
    }

}
