using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTutorialStep : TutorialStep {

    private void Start() {
        TutorialManager.Instance.PauseTutorial();
    }

    public void ForceFinishStep() {
        FinishQuestStep();
    }
}
