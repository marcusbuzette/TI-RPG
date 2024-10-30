using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats {

    [SerializeField] private int speed;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int accuracy;


    public int GetSpeed() { return this.speed; }
    public int GetAttack() { return this.attack; }
    public int GeDefence() { return this.defence; }

}
