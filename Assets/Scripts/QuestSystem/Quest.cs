using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class Quest {
    public QuestInfoSO info;
    public QuestState state;
    private int currentQuestStepIndex;

    public Quest(QuestInfoSO questInfoSO) {
        this.info = questInfoSO;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
    }

    public void MoveToNextStep() {
        this.currentQuestStepIndex++;
    }

    public bool CurrentQuestStepExists() {
        return (currentQuestStepIndex < info.questStepPrefabs.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform) {
        GameObject questStepPrefab = GetCurrentStepPrefab();
        if (questStepPrefab != null) {
            Object.Instantiate(questStepPrefab, parentTransform);
        }
    }

    private GameObject GetCurrentStepPrefab() {
        GameObject questStepPrefab = null;
        if (CurrentQuestStepExists()) {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        } else {
            Debug.LogWarning("Tentou acessar uma passo de quest nulo ou inexistente");
        }
        return questStepPrefab;
    }
}
