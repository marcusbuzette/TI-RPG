using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour {

    public event EventHandler OnDead;
    public event EventHandler OnDamage;
    [SerializeField] private int healthPoints = 100;
    [SerializeField] private int maxHealthPoints = 100;

    public void Damage(int damage) {

        healthPoints -= damage;

        if (healthPoints < 0) healthPoints = 0;
        OnDamage?.Invoke(this, EventArgs.Empty);
        if (healthPoints == 0) Die();
    }

    private void Die() {
        OnDead.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPointsNormalized() {
        return (float)healthPoints / maxHealthPoints;
    }
   
}
