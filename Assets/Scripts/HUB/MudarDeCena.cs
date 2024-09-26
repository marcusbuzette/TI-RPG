using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarDeCena : MonoBehaviour
{
    public string sceneToLoad;
    public DialogueTrigger dialogueTrigger;

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
        SceneManager.LoadScene(sceneToLoad);
    }
}