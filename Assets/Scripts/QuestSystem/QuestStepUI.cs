using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class QuestStepUI : MonoBehaviour {
    [SerializeField] private TMP_Text stepText;

    [SerializeField] private Color stepColor;
    [SerializeField] private Color stepCompletedColor;

    private bool hasDynamicText = false;
    private string originalText;
    private QuestStep step;

    public void SetQuesSteptInfo(Quest currentQuest, GameObject stepGO) {
        this.step = stepGO.GetComponent<QuestStep>();
        this.hasDynamicText = step.HasDynamicText();
        this.originalText = step.GetStepInstruction();
        this.stepText.text = originalText;
        this.stepText.color = stepColor;
        if(hasDynamicText) {
            UpdateText();
            QuestManager.Instance.onQuestStepUpdate += QuestManager_onStepUpdate;
            Debug.Log("inscreveu");
        };
    }

    public void CompleteStep() {
        QuestManager.Instance.onQuestStepUpdate -= QuestManager_onStepUpdate;
        this.stepText.color = stepCompletedColor;
    }

    private void UpdateText() {
        Debug.Log("Updating text");
        this.stepText.text = originalText + this.step.GetComponent<QuestStep>().GetDynamicText();
    }

    private void QuestManager_onStepUpdate(object sender, EventArgs e) {
        Debug.Log("chamou utade text");
        UpdateText();
    }

}
