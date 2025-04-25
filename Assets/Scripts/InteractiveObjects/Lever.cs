using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractiveObjects
{
    private bool goingTo = false;
    private GridPosition targetToUnit;
    private Unit currentUnit;

    public GameObject m_ObjectToCall;

    bool used = false;

    private void Start() {
        GetFowardGridObject();
    }

    private void Update() {
        if (goingTo) {
            if (currentUnit.GetGridPosition() == targetToUnit) {
                LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
                PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

                m_ObjectToCall.GetComponent<ICalledObject>().Action();

                used = true;
                goingTo = false;
            }
        }
    }

    public void GetFowardGridObject() {
        Vector3 pos = (transform.forward * 2) + transform.position;
        targetToUnit = LevelGrid.Instance.GetGridPosition(pos);
        Debug.Log(targetToUnit);
    }

    public void MoveUnitToGridPostion(Unit unit) {
        if (used) return;

        goingTo = true;
        currentUnit = unit;

        UnitActionSystem.Instance.MoveUnitToGridPosition(currentUnit, targetToUnit);
        LevelGrid.Instance.OnGameModeChanged += UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath += UnitStopGoingTo;
    }

    public void UnitStopGoingTo(object sender, EventArgs e) {
        LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

        Debug.Log("PAROU DE IR");

        goingTo = false;
        currentUnit = null;
    }

    public void UnitStopGoingTo() {
        LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

        goingTo = false;
        currentUnit = null;
    }
}
