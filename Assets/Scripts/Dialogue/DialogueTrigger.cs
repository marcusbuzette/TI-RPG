using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public List<Dialogue> historia_dialogues = new List<Dialogue>();

    public void TriggerDialogue(){

        Dialogue dialogue;
        if (GameController.controller.GetCurrentLevel() - 1 < historia_dialogues.Count) {
            dialogue = historia_dialogues[GameController.controller.GetCurrentLevel() - 1];
        } else {
            dialogue = historia_dialogues[historia_dialogues.Count -1];
        }
        DialogueController.dialogueController.StartDialogue(dialogue);
    }
}
