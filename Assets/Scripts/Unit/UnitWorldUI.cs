using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour {

    [SerializeField] private List<Image> healthBar;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Unit unit;

    private void Start() {
        healthSystem.OnDamage += HealthSystem_OnDamage;
    }


    public void UpdateHealthBar() {
        float healthNormalized = healthSystem.GetHealthPointsNormalized();
        foreach (Image healthBar in healthBar) {
            healthBar.fillAmount = healthNormalized;
        }
    }

    private void HealthSystem_OnDamage(object sender, EventArgs e) {
        UpdateHealthBar();
    } 

}
