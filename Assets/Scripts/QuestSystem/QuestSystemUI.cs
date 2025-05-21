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
    float duration = 0.6f;
    float time = 0f;

    victoryCanvasGroup.alpha = 0f;
    victoryPanel.localScale = Vector3.zero;

    while (time < duration) {
        time += Time.deltaTime;
        float t = time / duration;

        // Aplica bounce na escala (easeOutBounce estilo)
        float bounceT = EaseOutBack(t);

        victoryCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
        victoryPanel.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, bounceT);

        yield return null;
    }

    victoryCanvasGroup.alpha = 1f;
    victoryPanel.localScale = Vector3.one;
}

    private float EaseOutBack(float t) {
    float c1 = 1.70158f;
    float c3 = c1 + 1;

    return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
}

}
