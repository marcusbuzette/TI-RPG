using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStep : MonoBehaviour {

    private bool isFinished = false;
    [SerializeField] protected string stepInstruction;

    protected void FinishQuestStep() {
        if (!this.isFinished) isFinished = true;
        QuestManager.Instance.AdvanceQuest();
        Destroy(this.gameObject);
    }

    public string GetStepInstruction() {return this.stepInstruction;}
}
