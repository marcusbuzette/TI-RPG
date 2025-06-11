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
    [SerializeField] private GameObject comboInfoStep;
    private bool hasShowExploreMode = false;
    private bool hasShownSkillTree = false;
    [SerializeField] private bool isWaitingStep = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
    }

    void Start() {
        this.tutorialQuest.GenerateStepList();
        StartCoroutine(StartTutorial());
        Unit.OnAnyUnitDead += Unit_OnAnyUnityDead;
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
    public bool IsWaitingStep() {return this.isWaitingStep; }

    public void PauseTutorial() {
        this.isWaitingStep = true;
        if (this.hasShowComboInfo && this.hasShowExploreMode &&  this.hasShownSkillTree) {
            AdvanceTutorial();
        }
    }

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
        this.isWaitingStep = false;
        tutorialQuest.GetCurrentStepReference().GetComponent<PauseTutorialStep>().ForceFinishStep();
    }

    public void ShowExploreMode() {
        this.hasShowExploreMode = true;
    }

    private void Unit_OnAnyUnityDead(object sender, EventArgs e) {
        if (this.hasShowComboInfo) return;
        Unit unit = sender as Unit;
        if (!unit.IsEnemy()) return;
        this.hasShowComboInfo = true;
        GameObject stepAux = tutorialQuest.questStepPrefabs[tutorialQuest.GetCurrentStepIndex()];
        Debug.Log(stepAux);
        this.tutorialQuest.InsertStepAtIndex(this.comboInfoStep, this.tutorialQuest.GetCurrentStepIndex() + 1);
        this.tutorialQuest.InsertStepAtIndex(stepAux, this.tutorialQuest.GetCurrentStepIndex() + 2);
        if (isWaitingStep) {
            ResumeTutorial();
        } else {
            Destroy(stepAux);
            AdvanceTutorial();
        }
        Unit.OnAnyUnitDead -= Unit_OnAnyUnityDead;


    }


}
