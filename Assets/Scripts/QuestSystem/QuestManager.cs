using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour {

    public static QuestManager Instance { get; private set; }
    private Quest levelQuest;

    public EventHandler onQuestStarted;
    public EventHandler onQuestAdvanced;
    public EventHandler onQuestFinished;
    public EventHandler onQuestStepUpdate;
    public EventHandler onQuestStateChanged;

    void Awake() {
        if (Instance != null) {
            Destroy(this);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }


    public void SetLevelQuest(Quest quest) { this.levelQuest = quest; }

    public Quest GetLevelQuest() { return this.levelQuest; }

    public void StartQuest() {

        this.ChangeLevelQuestState(QuestState.IN_PROGRESS);
        this.levelQuest.InstantiateCurrentQuestStep(this.transform);
        this.onQuestStarted?.Invoke(this, EventArgs.Empty);
    }

    public void AdvanceQuest() {
        if (levelQuest == null) return;
        levelQuest.MoveToNextStep();
        if (levelQuest.CurrentQuestStepExists()) {
            levelQuest.InstantiateCurrentQuestStep(transform);
            onQuestAdvanced?.Invoke(this, EventArgs.Empty);
        }
        else {
            this.FinishQuest();
        }
    }

    public void FinishQuest() {
        foreach (Unit u in TurnSystem.Instance.GetUnitsOrderList()) {
            if (!u.IsEnemy()) u.AddXp(levelQuest.info.playerXp);
        }
        GameController.controller.AddMoney(levelQuest.info.moneyRewards);
        onQuestFinished?.Invoke(this, EventArgs.Empty);
        GameController.controller.NextLevel();
        // GameController.controller.uicontroller.ChangeScene("HUB");
    }

    public void QuestStateChange(Quest quest) {

    }

    public void QuestStepUpdated() {
        this.onQuestStepUpdate?.Invoke(this, EventArgs.Empty);
    }

    private void ChangeLevelQuestState(QuestState state) {
        this.levelQuest.state = state;
        this.onQuestStateChanged?.Invoke(this, EventArgs.Empty);
    }

}
