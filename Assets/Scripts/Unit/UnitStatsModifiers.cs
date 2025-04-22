using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum BuffType {
    ATTACK,DEFENCE,MOVE,SPEED,ACCURACY
}
[System.Serializable]
public class UnitStatsModifiers {
    [SerializeField] private int attack;
    [SerializeField] private int deffence;
    [SerializeField] private int move;
    [SerializeField] private int accuracy;
    [SerializeField] private int speed;

    public UnitStatsModifiers(int attack = 0, int deffence = 0, int move = 0, int accuracy = 0, int speed = 0) {
        this.accuracy = accuracy;
        this.attack = attack;
        this.move = move;
        this.accuracy = accuracy;
        this.speed = speed;
    }

    public int GetAttack() {return this.attack;}
    public int GetAccuracy() {return this.accuracy;}
    public int GetDefence() {return this.deffence;}
    public int GetMove() {return this.move;}
    public int GetSpeed() {return this.speed;}

    public void Buff(BuffType buffType, int buff, Unit target, BaseSkills bs) {
        switch(buffType) {
            case BuffType.ATTACK:
                this.attack += buff;
                break;
            case BuffType.DEFENCE:
                this.deffence += buff;
                break;
            case BuffType.SPEED:
                this.speed += buff;
                break;
            case BuffType.MOVE:
                this.move += buff;
                break;
            case BuffType.ACCURACY:
                this.accuracy += buff;
                break;
        }
        target.SubscribeToModifiedEvent(bs);
    }

    public void Debuff(BuffType buffType, int buff) {
        switch(buffType) {
            case BuffType.ATTACK:
                this.attack -= buff;
                break;
            case BuffType.DEFENCE:
                this.deffence -= buff;
                break;
            case BuffType.SPEED:
                this.speed -= buff;
                break;
            case BuffType.MOVE:
                this.move -= buff;
                break;
            case BuffType.ACCURACY:
                this.accuracy -= buff;
                break;
        }
    }

    public void ResetModifiers() {
        this.accuracy = 0;
        this.speed = 0;
        this.move = 0;
        this.attack = 0;
        this.deffence = 0;
    }

    public void ResetModifier(BuffType buffType) {
         switch(buffType) {
            case BuffType.ATTACK:
                this.attack =0;
                break;
            case BuffType.DEFENCE:
                this.deffence = 0;
                break;
            case BuffType.SPEED:
                this.speed = 0;
                break;
            case BuffType.MOVE:
                this.move = 0;
                break;
            case BuffType.ACCURACY:
                this.accuracy = 0;
                break;
        }   
    }
}
