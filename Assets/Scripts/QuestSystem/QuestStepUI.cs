using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestStepUI : MonoBehaviour {
    [SerializeField] private TMP_Text stepText;

    [SerializeField] private Color stepColor;
    [SerializeField] private Color stepCompletedColor;

    public void SetQuesSteptInfo(Quest currentQuest) {

        this.stepText.text = currentQuest.CurrentQuestStepExists() ? 
        currentQuest.info.questStepPrefabs[currentQuest.GetCurrentStepIndex()]
        .GetComponent<QuestStep>().GetStepInstruction() : "error";
        this.stepText.color = stepColor;
    }

    public void CompleteStep() {
        this.stepText.color = stepCompletedColor;
    }

}
