using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class Quest {
    public QuestInfoSO info;
    public List<GameObject> questStepPrefabs = new List<GameObject>();
    public QuestState state;
    [SerializeField] private int currentQuestStepIndex;
    private GameObject currentStepReference;

    public Quest(QuestInfoSO questInfoSO) {
        this.info = questInfoSO;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
    }

    public void MoveToNextStep() {
        this.currentQuestStepIndex++;
    }

    public int GetCurrentStepIndex() {return this.currentQuestStepIndex;}

    public bool CurrentQuestStepExists() {
        return (currentQuestStepIndex < questStepPrefabs.Count);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform) {
        GameObject questStepPrefab = GetCurrentStepPrefab();
        if (questStepPrefab != null) {
            currentStepReference = Object.Instantiate(questStepPrefab, parentTransform);
        }
    }

    private GameObject GetCurrentStepPrefab() {
        GameObject questStepPrefab = null;
        if (CurrentQuestStepExists()) {
            questStepPrefab = this.questStepPrefabs[currentQuestStepIndex];
        } else {
            Debug.LogWarning("Tentou acessar uma passo de quest nulo ou inexistente");
        }
        return questStepPrefab;
    }

    public GameObject GetCurrentStepReference() {return this.currentStepReference;}

    public void SkipQuestsStepToIndex(int index) {
        this.currentQuestStepIndex = index;
    }

    public void InsertStepAtIndex(GameObject step, int index) {
        this.questStepPrefabs.Insert(index, step);
    }

    public void GenerateStepList() {
        this.questStepPrefabs = new List<GameObject>(info.questStepPrefabs);
    }
}
