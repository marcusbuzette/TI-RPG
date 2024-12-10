using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XpSystem : MonoBehaviour {

    [SerializeField] private int xp = 0;

    public int getXpAmount() { return this.xp; }

    public void AddXp(int xpAmount) { 
        this.xp += xpAmount; }

    public void SetXp(int xp) {this.xp = xp;}

    public void UseXp(int xpAmount) {
        if (this.xp > 0) {
            this.xp -= xpAmount;
            if (this.xp <= 0) this.xp = 0;
        }
    }

    public void ResetXP() {
        this.xp = 0;
    }

    public bool CanBuyUpgrade(int xpAmount) { return this.xp - xpAmount > 0;}
}
