using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IDataPersistence {

    public static TutorialManager Instance;
    [SerializeField] private Quest tutorialQuest;

    public EventHandler onTutorialStarted;
    public EventHandler onTutorialStateChanged;
    public EventHandler onTutorialAdvanced;
    public EventHandler onTutorialFinished;

    private bool isTutorialFinished = false;

    private bool hasShowComboInfo = false;
    private bool hasShowExploreMode = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }

    void Start() {
        StartCoroutine(StartTutorial());

    }

    public IEnumerator StartTutorial() {
        yield return new WaitForSeconds(1.7f);
        this.ChangeLevelQuestState(QuestState.IN_PROGRESS);
        this.tutorialQuest.InstantiateCurrentQuestStep(this.transform);
        this.onTutorialStarted?.Invoke(this, EventArgs.Empty);
    }

    public void FinishTutorial () {
        this.isTutorialFinished = true;
        this.onTutorialStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AdvanceTutorial() {
        if (tutorialQuest == null) return;
        tutorialQuest.MoveToNextStep();
        if (tutorialQuest.CurrentQuestStepExists()) {
            tutorialQuest.InstantiateCurrentQuestStep(transform);
            onTutorialAdvanced?.Invoke(this, EventArgs.Empty);
        }
        else {
            this.FinishTutorial();
        }
    }

    private void dataLoaded() {
        if (isTutorialFinished) {
            Destroy(this);
        } else {
            StartTutorial();
        }
    }

    public bool IsTutorialFinished() {return this.isTutorialFinished; }

    private void ChangeLevelQuestState(QuestState state) {
        this.tutorialQuest.state = state;
        this.onTutorialStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SaveData(ref GameData data) {
        data.finishedTutorial = isTutorialFinished;
        data.tutorialIndex = tutorialQuest.GetCurrentStepIndex();
    }

    public void LoadData(GameData data) {
        isTutorialFinished = data.finishedTutorial;
        if (data.tutorialIndex > 0) tutorialQuest.SkipQuestsStepToIndex(data.tutorialIndex);
        dataLoaded();
    }

    public void ResumeTutorial() {
        // if (tutorialQuest.GetCurrentStepReference().GetType() == PauseTutorialStep)
        tutorialQuest.GetCurrentStepReference().GetComponent<PauseTutorialStep>().ForceFinishStep();
    }


}
