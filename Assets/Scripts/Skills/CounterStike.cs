using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CounterStike : BaseSkills {
    private float totalSpinAmmount = 0;
    [SerializeField] private float MAX_SPIN = 360f;
    public string counterStrikeSFX;

    [SerializeField] private BaseAction attackToPerform;

    private bool isCountering = false;
    private int Attack = 1;

    private HealthSystem hs;

    void Start() {
        hs = GetComponent<Unit>().GetHealthSystem();
        hs.OnDamage += HealthSystem_OnDamage;
        if (attackToPerform == null) attackToPerform = GetComponent<HitAction>();
    }

    public override void Action() {
        if (Attack == 1) {
            AudioManager.instance?.PlaySFX("Melee");
            Attack = 0;
        }
        StartCoroutine(DelayActionFinish());
    }

    public override string GetActionName() {
        return "Contra Ataque";
    }

    private IEnumerator DelayActionFinish() {
        yield return new WaitForSeconds(0.5f); // Ajuste o tempo conforme necessário
        ActionFinish();
        Attack = 1;
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> {
            unitGridPosition
        };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        this.isCountering = true;
        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override void IsAnotherRound() {
        if (this.isCountering) this.StopCounter();
        if (currentCoolDown > 0) {
            currentCoolDown--;
        }
        if (currentCoolDown <= 0) {
            onCoolDown = false;
        }
    }

    private void StopCounter() {
        this.isCountering = false;
        //voltar animação de idle aqui
    }

    private void HealthSystem_OnDamage(object sender, EventArgs e) {
        if (this.isCountering) this.PerformCounterStrike((e as HealthSystemEvent).attacker);
    }

    private void PerformCounterStrike(Unit attacker) {
        this.StopCounter();
        attackToPerform.TriggerAction(
            attacker.GetGridPosition(),
            AfterCounterAttack
        );
    }

    private void AfterCounterAttack() {return;}

    public override bool GetOnCooldown() { return onCoolDown; }

    private void OnDestroy() {
       hs.OnDamage -= HealthSystem_OnDamage;
    }
}
