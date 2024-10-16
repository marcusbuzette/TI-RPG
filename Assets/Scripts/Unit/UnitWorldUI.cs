using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour {

    [SerializeField] private Image healthBar;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Unit unit;

    private void Start() {
        healthSystem.OnDamage += HealthSystem_OnDamage;
    }


    public void UpdateHealthBar() {
        healthBar.fillAmount = healthSystem.GetHealthPointsNormalized();
    }

    private void HealthSystem_OnDamage(object sender, EventArgs e) {
        UpdateHealthBar();
    } 

}
