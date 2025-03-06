using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarDeCena : MonoBehaviour
{
    public string sceneToLoad;
    public DialogueTrigger dialogueTrigger;

    private UIController uiController; // PEU
    private void Awake() { // PEU
        // Encontrar o UIController na cena
        uiController = FindObjectOfType<UIController>();
        if (uiController == null) {
            Debug.LogError("UIController n�o encontrado na cena!");
        }
    }
    private void OnMouseDown()
    {
        if (gameObject.CompareTag("Campfire")) 
        {
            if (dialogueTrigger != null)
            {
                dialogueTrigger.TriggerDialogue();
            }

            StartCoroutine(LoadSceneAfterDialogue());
        }
    }

    private IEnumerator LoadSceneAfterDialogue()
    {
        while (DialogueController.dialogueController.animator.GetBool("IsOpen"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        //SceneManager.LoadScene(sceneToLoad);

        if (uiController != null) { //peu
            uiController.ChangeScene(sceneToLoad);
        }
        else {
            // Fallback, caso o UIController n�o seja encontrado
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}