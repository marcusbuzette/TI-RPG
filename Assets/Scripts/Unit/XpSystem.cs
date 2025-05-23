using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XpSystem : MonoBehaviour {

    [SerializeField] private int xp = 0;
    [SerializeField] private int levelXp = 0;

    public int getXpAmount() { return this.xp; }
    public int getLevelXpAmount() { return this.levelXp; }

    public void AddXp(int xpAmount) { 
        this.levelXp += xpAmount; }

    public void SetXp(int xp) {this.xp = xp;}

    public void NextLevelXp() {
        this.xp += this.levelXp;
    }

    public void UseXp(int xpAmount) {
        if (this.xp > 0) {
            this.xp -= xpAmount;
            if (this.xp <= 0) this.xp = 0;
        }
    }

    public void ResetXP() {
        this.xp = 0;
        this.levelXp = 0;
    }

    public bool CanBuyUpgrade(int xpAmount) { return this.xp - xpAmount > 0;}
}
