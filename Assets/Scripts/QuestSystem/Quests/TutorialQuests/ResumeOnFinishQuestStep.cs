using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeOnFinishQuestStep : MonoBehaviour {
    void OnDestroy() {
        if (!TutorialManager.Instance.IsTutorialFinished() && TutorialManager.Instance.IsWaitingStep()) {
            TutorialManager.Instance.ResumeTutorial();
        }
    }
}
