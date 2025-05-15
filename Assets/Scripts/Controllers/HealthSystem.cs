using System;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour {

    public event EventHandler OnDead;
    public event EventHandler OnRevive;
    public event EventHandler OnDamage;

    public enum HealthState { ALIVE, FAINT }

    [SerializeField] private HealthState healthState = HealthState.ALIVE;

    private UnitWorldUI worldUI;
    public int healthPoints = 100;
    public int maxHealthPoints = 100;
    public Animator animator;
    public string damageSFX;
    private bool isDefending = false;

    public Transform faintText; //TEMPORARIO <----- (ATENÇÃO)

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        OnDamage?.Invoke(this, EventArgs.Empty);
    }

    public void TestDamage(int damage, Unit attackedBy, bool haveProjectile) {
        //Verifica se alguma unidade o atacou, se não, foi algum efeito que não tem chance de errar
        if (attackedBy != null) {
            int dice = Random.Range(0, 10);

            if (dice <= 1) {
                attackedBy.GetHealthSystem().GetUnitWorldUI().ShowUIValue(0, "Miss");
                if(haveProjectile) attackedBy.SpawnProjectile(this, 0, true);
                return;
            }
        }

        Debug.Log(haveProjectile);
        if (haveProjectile) attackedBy.SpawnProjectile(this, damage);
        else Damage(damage, attackedBy);
    }

    public void Damage(int damage, Unit attackedBy) {
        if (isDefending) {
            worldUI.ShowUIValue(0, "Defending");
            return;
        }

        // animator?.SetTrigger("TookDamage");
        GetComponent<Unit>().PlayAnimation("TookDamage");

        healthPoints -= damage;

        worldUI.ShowUIValue(damage, "Damage");

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
        healthState = HealthState.FAINT;

        if (!GetComponent<Unit>().IsEnemy()) {
            worldUI.GetHealthBarPrefab().SetActive(false);
            faintText?.gameObject.SetActive(true);
        }

        OnDead.Invoke(this, EventArgs.Empty);
    }

    public bool Revive(int amount = 20) {
        if(LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE) { return false; }

        healthState = HealthState.ALIVE;
        healthPoints += amount;
        OnRevive.Invoke(this, EventArgs.Empty);
        OnDamage?.Invoke(this, EventArgs.Empty);

        faintText?.gameObject.SetActive(false);
        worldUI.GetHealthBarPrefab().SetActive(true);

        return true;
    }

    public float GetHealthPointsNormalized() {
        return (float)healthPoints / maxHealthPoints;
    }

    public void Heal(int amount) {
        healthPoints += amount;
        if (healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
        OnDamage?.Invoke(this, EventArgs.Empty);

        worldUI.ShowUIValue(amount, "Heal");
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

    public void SetUnitWorldUI(UnitWorldUI worldUI) { this.worldUI = worldUI; }
    public UnitWorldUI GetUnitWorldUI() { return worldUI; }
    public HealthState GetHealthState() { return healthState; }
}
