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

    public Animator m_Animator;

    bool used = false;

    public GameObject m_Camera;

    private void Start() {
        GetFowardGridObject();
    }

    private void Update() {
        if (goingTo) {
            if (currentUnit.GetGridPosition() == targetToUnit) {
                LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
                PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

                m_ObjectToCall.GetComponent<ICalledObject>().Action(m_Camera);

                TriggerAnim();
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

    public void TriggerAnim() {
        m_Animator.SetBool("Used", true);
    }
}
