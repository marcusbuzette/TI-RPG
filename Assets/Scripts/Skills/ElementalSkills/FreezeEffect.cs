using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    private Unit unit;
    private int coolDown;

    private MoveAction moveAction;
    private int maxMoveDistance;
    private float moveSpeed;

    private void Start() {
        LevelGrid.Instance.OnGameModeChanged += CureFreeze;
    }

    public void SetFreezeEffect(Unit unit, int coolDown) {
        this.unit = unit;
        this.coolDown = coolDown;

        moveAction = unit.GetAction<MoveAction>();
        maxMoveDistance = moveAction.GetMaxDistanceMovement();
        moveSpeed = moveAction.GetMovementSpeed();

        Freeze();
    }

    public void Freeze() {
        moveAction.SetMaxDistanceMovement(Mathf.RoundToInt(maxMoveDistance / 1.5f));
        moveAction.SetMovementSpeed(Mathf.RoundToInt(moveSpeed / 1.5f));
        
        coolDown--;
        if (coolDown <= 0) CureFreeze();
    }

    public void CureFreeze() {
        moveAction.SetMaxDistanceMovement(maxMoveDistance);
        moveAction.SetMovementSpeed(moveSpeed);

        Destroy(this);
    }
    
    public void CureFreeze(object sender, EventArgs e) {
        LevelGrid.Instance.OnGameModeChanged -= CureFreeze;
        moveAction.SetMaxDistanceMovement(maxMoveDistance);
        moveAction.SetMovementSpeed(moveSpeed);

        Destroy(this);
    }
}
