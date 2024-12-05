using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats {

    [SerializeField] private int speed;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int accuracy;
    [SerializeField] private int xpSpoil;

    public UnitStats(int speed, int attack, int defence, int accuracy, int xpSpoil) {
        this.speed = speed;
        this.attack = attack;
        this.defence = defence;
        this.accuracy = accuracy;
        this.xpSpoil = xpSpoil;
    }


    public int GetSpeed() { return this.speed; }
    public int GetAttack() { return this.attack; }
    public int GetDefence() { return this.defence; }
    public int GetXpSpoil() { return this.xpSpoil; }

}
