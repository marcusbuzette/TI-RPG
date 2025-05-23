using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogStep : QuestStep {

    [SerializeField] private Dialogue[] dialogues;

    private void Start() {
        DialogueController.dialogueController.StartDialogue(dialogues[0]);
        DialogueController.dialogueController.onEndDialogue += DialogueController_OnEndDialogue;
    }

    private void OnEnable() {

    }

    private void OnDisable() {

    }

    private void OnDestroy() {
        DialogueController.dialogueController.onEndDialogue -= DialogueController_OnEndDialogue;
    }

    private void DialogueController_OnEndDialogue(object enemy, EventArgs e) {
        this.FinishQuestStep();
    }

}
