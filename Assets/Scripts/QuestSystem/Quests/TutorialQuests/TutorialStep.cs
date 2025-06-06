using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStep : QuestStep {
    override protected void FinishQuestStep() {
        if (!this.isFinished) isFinished = true;
        TutorialManager.Instance.AdvanceTutorial();
        Destroy(this.gameObject);
    }
}
