using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour, IDataPersistence {

    public static TutorialManager Instance;
    [SerializeField] private Quest tutorialQuest;

    public EventHandler onTutorialStarted;
    public EventHandler onTutorialStateChanged;
    public EventHandler onTutorialAdvanced;
    public EventHandler onTutorialFinished;

    [SerializeField] private bool isTutorialFinished = false;

    [SerializeField] private bool hasShowComboInfo = false;
    [SerializeField] private GameObject comboInfoStep;
    [SerializeField] private GameObject treeInfoStep;
    [SerializeField] private bool hasShowCamping = false;
    [SerializeField] private bool hasFinishedTutorialLevel = false;
     [SerializeField]private bool hasShownSkillTree = false;
    [SerializeField] private bool isWaitingStep = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }

    void Start() {
        this.tutorialQuest.GenerateStepList();
        if (SceneManager.GetActiveScene().name == "HUB") {
            StartCoroutine(this.StartFromHUB());
        }
        else {
            StartCoroutine(StartTutorial());
        }
        Unit.OnAnyUnitDead += Unit_OnAnyUnityDead;
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    public IEnumerator StartTutorial() {
        yield return new WaitForSeconds(1.7f);
        this.ChangeLevelQuestState(QuestState.IN_PROGRESS);
        this.tutorialQuest.InstantiateCurrentQuestStep(this.transform);
        this.onTutorialStarted?.Invoke(this, EventArgs.Empty);
    }

    public void FinishTutorial() {
        this.isTutorialFinished = true;
        this.onTutorialStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AdvanceTutorial() {
        if (tutorialQuest == null) return;
        tutorialQuest.MoveToNextStep();
        if (tutorialQuest.CurrentQuestStepExists() &&
            (
                (tutorialQuest.GetCurrentStepIndex() < tutorialQuest.questStepPrefabs.Count) 
                &&
                (!hasShowCamping || !hasShownSkillTree || !hasFinishedTutorialLevel)
             )
            ) {
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
        }
        else {
            StartTutorial();
        }
    }

    public bool IsTutorialFinished() { return this.isTutorialFinished; }
    public bool IsWaitingStep() { return this.isWaitingStep; }
    public bool HasShownSkillTree() { return this.hasShownSkillTree; }

    public void PauseTutorial() {
        this.isWaitingStep = true;
        if (this.hasShowComboInfo && this.hasShownSkillTree) {
            Debug.Log("Ultimo pause step");
            AdvanceTutorial();
        }
    }

    public void ShowSkillTreeStep() {
        GameObject stepAux = tutorialQuest.questStepPrefabs[tutorialQuest.GetCurrentStepIndex()];
        this.tutorialQuest.InsertStepAtIndex(this.treeInfoStep, this.tutorialQuest.GetCurrentStepIndex() + 1);
        this.tutorialQuest.InsertStepAtIndex(stepAux, this.tutorialQuest.GetCurrentStepIndex() + 2);
        if (isWaitingStep) {
            ResumeTutorial();
            this.hasShownSkillTree = true;
        }
        else {
            Destroy(stepAux);
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
        // isTutorialFinished = data.finishedTutorial;
        // if (data.tutorialIndex > 0) {
        //     int skipStepsNumber = data.tutorialIndex;
        //     if (hasShowComboInfo) skipStepsNumber -= 2;
        //     tutorialQuest.SkipQuestsStepToIndex(data.tutorialIndex);
        // }
        // dataLoaded();
    }

    public void ResumeTutorial() {
        this.isWaitingStep = false;
        tutorialQuest.GetCurrentStepReference().GetComponent<PauseTutorialStep>().ForceFinishStep();
    }

    private void Unit_OnAnyUnityDead(object sender, EventArgs e) {
        if (this.hasShowComboInfo) return;
        Unit unit = sender as Unit;
        if (!unit.IsEnemy()) return;
        this.hasShowComboInfo = true;
        GameObject stepAux = tutorialQuest.questStepPrefabs[tutorialQuest.GetCurrentStepIndex()];
        this.tutorialQuest.InsertStepAtIndex(this.comboInfoStep, this.tutorialQuest.GetCurrentStepIndex() + 1);
        this.tutorialQuest.InsertStepAtIndex(stepAux, this.tutorialQuest.GetCurrentStepIndex() + 2);
        if (isWaitingStep) {
            ResumeTutorial();
        }
        else {
            Destroy(stepAux);
            AdvanceTutorial();
        }
        Unit.OnAnyUnitDead -= Unit_OnAnyUnityDead;
    }

    private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene) {
        if (!this.isTutorialFinished && !this.hasShowCamping && !hasFinishedTutorialLevel && newScene.name == "HUB") {
            this.hasShowCamping = true;
            this.hasFinishedTutorialLevel = true;
            GameObject stepAux = tutorialQuest.questStepPrefabs[tutorialQuest.GetCurrentStepIndex()];
            if (isWaitingStep) {
                ResumeTutorial();
            }
            else {
                Destroy(stepAux);
                AdvanceTutorial();
            }
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

        }
    }


    private IEnumerator StartFromHUB() {
        yield return new WaitForSeconds(.5f);
        this.hasFinishedTutorialLevel = true;
        this.hasShowCamping = true;
        this.ChangeLevelQuestState(QuestState.IN_PROGRESS);
        this.tutorialQuest.InstantiateCurrentQuestStep(this.transform);
        this.onTutorialStarted?.Invoke(this, EventArgs.Empty);
    }


}
