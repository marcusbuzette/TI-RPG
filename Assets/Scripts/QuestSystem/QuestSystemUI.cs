using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystemUI : MonoBehaviour {

    [SerializeField] private Transform questsContainer;
    [SerializeField] private GameObject questStepPrefab;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private Transform victoryPanel;
    [SerializeField] private Transform victoryDisplayRoot;
    private GameObject currentStep;

    private void Start() {
        QuestManager.Instance.onQuestStarted += QuestManager_OnQuestStarted;
        QuestManager.Instance.onQuestAdvanced += QuestManager_OnQuestAdvanced;
        QuestManager.Instance.onQuestFinished += QuestManager_OnQuestFinished;

        victoryCanvasGroup = victoryPanel.GetComponent<CanvasGroup>();
        if (victoryCanvasGroup == null) {
            victoryCanvasGroup = victoryPanel.gameObject.AddComponent<CanvasGroup>();
        }
        victoryPanel.gameObject.SetActive(false);

        this.QuestManager_OnQuestStarted(this, EventArgs.Empty);
    }

    private void QuestManager_OnQuestStarted(object sender, EventArgs e) {
        currentStep = Instantiate(questStepPrefab, questsContainer);
        currentStep.GetComponent<QuestStepUI>().SetQuesSteptInfo(QuestManager.Instance.GetLevelQuest());
    }

    private void QuestManager_OnQuestAdvanced(object sender, EventArgs e) {
        currentStep.GetComponent<QuestStepUI>().CompleteStep();
        //verifica se o proximo passo da quest tem uma instrucao. So se houver instrucao, o passo aparecera na lista
        if (QuestManager.Instance.GetLevelQuest().info.questStepPrefabs[QuestManager.Instance.GetLevelQuest()
        .GetCurrentStepIndex()].GetComponent<QuestStep>().GetStepInstruction() != "") {
            currentStep = Instantiate(questStepPrefab, questsContainer);
            currentStep.GetComponent<QuestStepUI>().SetQuesSteptInfo(QuestManager.Instance.GetLevelQuest());
        }
    }

    private void QuestManager_OnQuestFinished(object sender, EventArgs e) {
        victoryDisplayRoot.gameObject.SetActive(true);
        victoryPanel.gameObject.SetActive(true);

        StartCoroutine(AnimateVictoryPanel());
    }

    void OnDestroy() {
        QuestManager.Instance.onQuestStarted -= QuestManager_OnQuestStarted;
        QuestManager.Instance.onQuestAdvanced -= QuestManager_OnQuestAdvanced;
    }

    private IEnumerator AnimateVictoryPanel() {
    float duration = 0.5f;
    float time = 0f;

    victoryCanvasGroup.alpha = 0f;
    victoryPanel.localScale = Vector3.zero;

    while (time < duration) {
        time += Time.deltaTime;
        float t = time / duration;

        victoryCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
        victoryPanel.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

        yield return null;
    }

    victoryCanvasGroup.alpha = 1f;
    victoryPanel.localScale = Vector3.one;
}

}
