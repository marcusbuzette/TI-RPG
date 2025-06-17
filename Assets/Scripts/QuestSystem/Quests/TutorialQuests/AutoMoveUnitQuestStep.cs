using System;
using System.Linq;
using UnityEngine;

public class AutoMoveUnitQuestStep : TutorialStep {

    [Serializable]
    public struct GridValuesAux {
        public int x;
        public int z;
        public int floor;
    }

    [Serializable]
    public struct WhereUnitGo {
        public string unitId;
        public Unit unit;
        public GridValuesAux position;
    }

    public WhereUnitGo[] unitsToMove;
    private int finishedWalk = 0;

    private void Start() {
        Unit[] units = FindObjectsOfType<Unit>();
        for (int i = 0; i < unitsToMove.Length; i++) {
            unitsToMove[i].unit = units.FirstOrDefault(u => u.unitId == unitsToMove[i].unitId);
            unitsToMove[i].unit.GetComponent<MoveAction>().OnFinishedWalking += MoveAction_OnFinishedWalking;
            GridPosition gp = LevelGrid.Instance.GetGridPositionFromXZValues(unitsToMove[i].position.x,unitsToMove[i].position.z,unitsToMove[i].position.floor);
            UnitActionSystem.Instance.MoveUnitToGridPosition(unitsToMove[i].unit, gp);
        }
    }

    private void MoveAction_OnFinishedWalking(object sender, EventArgs e) {
        Debug.Log("Finish walking");
        Debug.Log(sender as MoveAction);
        finishedWalk++;
        if (finishedWalk >= unitsToMove.Length) {
            this.FinishQuestStep();
        }
    }

    void OnDestroy() {
        for (int i = 0; i < unitsToMove.Length; i++) {
            unitsToMove[i].unit.GetComponent<MoveAction>().OnFinishedWalking -= MoveAction_OnFinishedWalking;
        }
    }


}
