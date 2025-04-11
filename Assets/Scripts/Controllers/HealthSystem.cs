using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour {

    public event EventHandler OnDead;
    public event EventHandler OnDamage;
    public int healthPoints = 100;
    public int maxHealthPoints = 100;
    public Animator animator;
    public string damageSFX;
    private bool isDefending = false;
    private void Awake() {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        OnDamage?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(int damage, Unit attackedBy) {
        if (isDefending) return;
        // animator?.SetTrigger("TookDamage");
        GetComponent<Unit>().PlayAnimation("TookDamage");

        healthPoints -= damage;

        if (!string.IsNullOrEmpty(damageSFX)) {
            AudioManager.instance?.PlaySFX(damageSFX);  // vai tocar o sfx q ta no inspector do healthSystem do cada boneco
        }
        if (healthPoints < 0) healthPoints = 0;
        if (healthPoints == 0) {
            Die();
        }
        else {
            OnDamage?.Invoke(this, new HealthSystemEvent(attackedBy));
        }
    }

    private void Die() {
        OnDead.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPointsNormalized() {
        return (float)healthPoints / maxHealthPoints;
    }

    public void Heal(int amount) {
        healthPoints += amount;
        if (healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
        OnDamage?.Invoke(this, EventArgs.Empty);
    }

    public HealthSystem GetHealthSystem() {
        return this;
    }

    public int GetHealthPoints() {
        return this.healthPoints;
    }

    public void SetMaxHP(int hp) {
        this.maxHealthPoints = hp;
        this.healthPoints = this.maxHealthPoints;
    }

    public void SetDefenceMode(bool isDefending) { this.isDefending = isDefending; }
    public bool GetDefenceMode() { return this.isDefending; }

}
