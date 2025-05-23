using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController dialogueController;
    private Queue<string> sentences;
    public Text NameTXT;
    public Text DialogueTXT;
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
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue){
        isDialogueOpened = true;
        animator?.SetBool("IsOpen", isDialogueOpened);
        NameTXT.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences){
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence(){
        if(sentences.Count == 0){
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StopCoroutine(TypeSentence(sentence));
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence){
        DialogueTXT.text="";
        foreach(char letter in sentence.ToCharArray ()){
            DialogueTXT.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void EndDialogue(){
        isDialogueOpened = false;
        animator?.SetBool("IsOpen", isDialogueOpened);
        onEndDialogue?.Invoke(this, EventArgs.Empty);
    }
}
