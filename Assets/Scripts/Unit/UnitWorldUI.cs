using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour {

    [SerializeField] private Image healthBar;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Unit unit;
    [SerializeField] private GameObject healthValuePrefab;

    private void Start() {
        healthSystem.OnDamage += HealthSystem_OnDamage;
        transform.parent.GetComponent<HealthSystem>().SetUnitWorldUI(this);

        if(!unit.IsEnemy()) {
            healthBar.GetComponent<Image>().color = Color.blue;
        }
        else healthBar.GetComponent<Image>().color = Color.red;
    }

    public void UpdateHealthBar() {
        float healthNormalized = healthSystem.GetHealthPointsNormalized();
        healthBar.fillAmount = healthNormalized;

    }

    private void HealthSystem_OnDamage(object sender, EventArgs e) {
        UpdateHealthBar();
    }

    public void ShowUIValue(int value, string type) {
        if (healthValuePrefab == null) {
            Debug.Log("HELTH VALUE PREFAB DO NOT ASSING IN UNIT WORLD UI OF " + unit);
            return;
        }

        var obj = Instantiate(healthValuePrefab, transform);
        switch (type) {
            case "Damage":
                obj.GetComponent<TMP_Text>().color = Color.red;
                obj.GetComponent<TMP_Text>().text = ("-" + value);
                break;
            case "Heal":
                obj.GetComponent<TMP_Text>().color = Color.green;
                obj.GetComponent<TMP_Text>().text = ("+" + value);
                break;
            case "Defending":
                obj.GetComponent<TMP_Text>().color = Color.gray;
                obj.GetComponent<TMP_Text>().text = ("DEFENDED");
                break;
            case "Miss":
                obj.GetComponent<TMP_Text>().color = Color.red;
                obj.GetComponent<TMP_Text>().text = ("MISS");
                break;
        }

        Destroy(obj, 1f);
    }

    public GameObject GetHealthBarPrefab() { return healthBar.gameObject; }
}
