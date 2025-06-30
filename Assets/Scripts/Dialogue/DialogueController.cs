using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour {
    public static DialogueController dialogueController;
    private Queue<Dialogue.DialogueStruct> dialogueQueue;
    public TMP_Text NameTXT;
    public TMP_Text DialogueTXT;
    public Image imageChar;
    public Animator animator;
    public bool isDialogueOpened = false;


    public EventHandler onEndDialogue;

    private void Awake() {
        if (dialogueController == null) {
            dialogueController = this;
            //DontDestroyOnLoad(this);
        }
        else {
            DestroyImmediate(gameObject);
        }

    }
    void Start() {
        dialogueQueue = new Queue<Dialogue.DialogueStruct>();
    }


    public void StartDialogue(Dialogue dialogue) {
        CameraHUB.isMenuOpen = true;
        isDialogueOpened = true;
        animator?.SetBool("IsOpen", isDialogueOpened);
        dialogueQueue.Clear();

        foreach (Dialogue.DialogueStruct d in dialogue.dialogue) {
            dialogueQueue.Enqueue(d);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (dialogueQueue.Count == 0) {

            EndDialogue();
            return;
        }
        Dialogue.DialogueStruct d = dialogueQueue.Dequeue();
        StopAllCoroutines();
        StopCoroutine(TypeSentence(d.sentences));
        if (d.image != null) {
            imageChar.sprite = d.image;
        }
        NameTXT.text = d.name;
        StartCoroutine(TypeSentence(d.sentences));
    }

    IEnumerator TypeSentence(string sentence) {
        DialogueTXT.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            DialogueTXT.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void EndDialogue() {
        isDialogueOpened = false;
        CameraHUB.isMenuOpen = false;
        animator?.SetBool("IsOpen", isDialogueOpened);
        onEndDialogue?.Invoke(this, EventArgs.Empty);


    }
}
