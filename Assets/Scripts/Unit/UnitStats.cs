using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats {

    [SerializeField] private BaseUnitStats baseUnitStats;
    [SerializeField] private int speed;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int accuracy;
    [SerializeField] private int maxHealthPoints;
    [SerializeField] private int maxMove;
    [SerializeField] private int xpSpoil;
    [SerializeField] private int range;

    public UnitStats(int speed, int attack, int defence, int accuracy,
    int xpSpoil, int maxHealthPoints, int maxMove, int range, BaseUnitStats baseUnitStats) {
        this.speed = speed;
        this.attack = attack;
        this.defence = defence;
        this.accuracy = accuracy;
        this.xpSpoil = xpSpoil;
        this.maxHealthPoints = maxHealthPoints;
        this.maxMove = maxMove;
        this.range = range;
        this.baseUnitStats = baseUnitStats;
    }


    public int GetSpeed() { return this.speed; }
    public int GetAttack() { return this.attack; }
    public int GetDefence() { return this.defence; }
    public int GetMaxHP() { return this.maxHealthPoints; }
    public int GetMaxMove() { return this.maxMove; }
    public int GetXpSpoil() { return this.xpSpoil; }
    public int GetRange() { return this.range; }
    public BaseUnitStats GetBaseUnitStats() {return this.baseUnitStats;}

    public void UpgradeSpeed(int speed) { this.speed += speed; }
    public void UpgradeAttack(int attack) { this.attack += attack; }
    public void UpgradeDefence(int defence) { this.defence += defence; }
    public void UpgradeAccuracy(int accuracy) { this.accuracy += accuracy; }
    public void UpgradeHP(int maxHealthPoints) { this.maxHealthPoints += maxHealthPoints; }
    public void UpgradeMove(int maxMove) { this.maxMove += maxMove; }
    public void UpgradeRange(int range) { this.range += range; }
    public void SetMaxHP(int maxHp) { this.maxHealthPoints = maxHp; }


}


public struct BaseUnitStats {
    public int speed;
    public int attack;
    public int defence;
    public int accuracy;
    public int maxHealthPoints;
    public int maxMove;
    public int xpSpoil;
    public int range;
}
