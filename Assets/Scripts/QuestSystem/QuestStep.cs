using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStep : MonoBehaviour {

    private bool isFinished = false;
    [SerializeField] protected bool hasDinamicText = false;
    [SerializeField] protected string stepInstruction;

    protected void FinishQuestStep() {
        if (!this.isFinished) isFinished = true;
        QuestManager.Instance.AdvanceQuest();
        Destroy(this.gameObject);
    }

    public string GetStepInstruction() {return this.stepInstruction;}

    public bool HasDynamicText() {return this.hasDinamicText;}

    virtual public string GetDynamicText() {
        return "";}
}
