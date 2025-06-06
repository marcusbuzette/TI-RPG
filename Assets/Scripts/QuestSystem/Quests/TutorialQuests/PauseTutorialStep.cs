using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTutorialStep : TutorialStep {

    public bool isPaused;

    public void ForceFinishStep() {
        FinishQuestStep();
    }
}
