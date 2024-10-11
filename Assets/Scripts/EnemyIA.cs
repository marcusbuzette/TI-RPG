using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyIA : MonoBehaviour
{
    private float timer;

    private void Start()
    {
        TurnSystem.Instance.onTurnChange += TurnSystem_onTurnChange;
    }
    private void Update()
    {

    if(TurnSystem.Instance.IsPlayerTurn())
    {
        return;
    }

    timer-= Time.deltaTime;
    if(timer<= 0f)
    {
        TurnSystem.Instance.NextTurn();
    }
    }

private void TurnSystem_onTurnChange(object sender, EventArgs e)
{
    timer = 2f;
}
}
