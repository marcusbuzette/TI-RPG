using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    private Unit unit;
    private int damage;
    private int coolDown;
    private PoisonAttack poisonAttack;

    public void SetPoisonEffect(Unit unit, int damage, int coolDown) {
        this.unit = unit;
        this.damage = damage;
        this.coolDown = coolDown;

        TurnSystem.Instance.onTurnChange += TurnSystem_onTurnChange;
    }

    public void TurnSystem_onTurnChange(object sender, EventArgs e) {
        unit.Damage(damage);
        coolDown--;
        if(coolDown <= 0 ) CurePoison();
    }

    private void OnDestroy() {
        TurnSystem.Instance.onTurnChange -= TurnSystem_onTurnChange;
    }

    public void CurePoison() {
        Destroy(this);
    }
}
