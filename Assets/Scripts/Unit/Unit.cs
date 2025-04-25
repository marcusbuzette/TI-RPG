using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct PossibleUpgrade {
    public int level;
    public UpgradeObject[] upgrade;

}

[Serializable]
public class Unit : MonoBehaviour {

    public static event EventHandler OnAnyActionPerformed;
    public static event EventHandler OnAnyUnitSpawn;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    [SerializeField] private List<BaseSkills> possibleSkills = new List<BaseSkills>();
    [SerializeField] private List<PossibleUpgrade> possibleUpgrades = new List<PossibleUpgrade>();
    [SerializeField] private XpSystem xpSystem;
    [SerializeField] public string unitId = "";
    [SerializeField] public string unitName = "";
    [SerializeField] private UnitStats baseUnitStats;
    private UnitStats unitStats;
    [SerializeField] private UnitStatsModifiers statsModifiers;
    [SerializeField] private BaseAction[] actionsArray;
    [SerializeField] private bool hasMoved = false;
    [SerializeField] private bool hasPerformedAction = false;
    [SerializeField] private bool hasPerformedSkill = false;
    [SerializeField] private List<Unit> modifiedBy = new List<Unit>();
    public bool isUnitTurn = false;

    private Dictionary<string, bool> animationTriggersStack = new Dictionary<string, bool>();
    private Animator animator;

    private GridPosition newGridPosition;
    private Vector3 lastPosition;


    private int intimidateCoolDown = 0;
    [SerializeField] private int enemyFocus = 0;

    private void Awake() {
        healthSystem = GetComponent<HealthSystem>();
        xpSystem = GetComponent<XpSystem>();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        if (!isEnemy && GameController.controller.HasUnitRecords(unitId)) {
            UnitRecords unitRecords = GameController.controller.GetUnitRecords(unitId);
            this.xpSystem.SetXp(unitRecords.xp);
            this.unitStats = unitRecords.unitStats;
            foreach (BaseSkills skill in unitRecords.baseSkills) {
                BaseSkills aux = possibleSkills.Find((s) => s.nome == skill.nome);
                BaseSkills bs = gameObject.AddComponent(skill.GetType()) as BaseSkills;
                if (aux != null) {
                    bs.SetSkillImage(aux.GetActionImage());
                }
            }
            actionsArray = GetComponents<BaseAction>();
            OnAnyActionPerformed?.Invoke(this, EventArgs.Empty);

        }
        else {
            this.unitStats = baseUnitStats;
        }
        this.healthSystem.SetMaxHP(this.unitStats.GetMaxHP());
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.onTurnChange += TurnSystem_OnTurnChange;

        healthSystem.OnDead += HealthSystem_OnDie;
        OnAnyUnitSpawn?.Invoke(this, EventArgs.Empty);
        statsModifiers = new UnitStatsModifiers();
    }

    private void Update() {
        if (transform.position != lastPosition) {
            lastPosition = transform.position;
            newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

            if (newGridPosition != gridPosition) {
                GridPosition oldGridPosition = gridPosition;
                gridPosition = newGridPosition;

                LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
            }
        }
    }

    public T GetAction<T>() where T : BaseAction {
        foreach (BaseAction baseAction in actionsArray) {
            if (baseAction is T) {
                return (T)baseAction;
            }
        }
        return null;
    }

    public BaseAction[] GetActionsArray() {
        return actionsArray.ToArray();
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    public bool TryToPerformAction(BaseAction action) {
        if (CanTriggerAction(action)) {
            PerformAction(action);
            return true;
        }
        else {
            return false;
        }
    }

    public bool CanTriggerAction(BaseAction action) {
        switch (action.GetActionType()) {
            case ActionType.MOVE:
                return !hasMoved;
            case ActionType.ACTION:
                return !hasPerformedAction;
            case ActionType.SKILL:
                return !hasPerformedSkill;
            case ActionType.INVENTORY:
                return true;
            case ActionType.ITEM:
                return true;
        }
        return false;
    }

    private void PerformAction(BaseAction action) {
        switch (action.GetActionType()) {
            case ActionType.MOVE:
                hasMoved = true;
                break;
            case ActionType.ACTION:
                hasPerformedAction = true;
                hasPerformedSkill = true;
                hasMoved = true;
                break;
            case ActionType.SKILL:
                hasPerformedSkill = true;
                break;
        }
        OnAnyActionPerformed?.Invoke(this, EventArgs.Empty);
    }

    public bool GetHasMoved() { return hasMoved; }
    public bool GetHasPerformedAction() { return hasPerformedAction; }

    public bool GetHasPerformedSkill() { return hasPerformedSkill; }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e) {
        if ((IsEnemy() && isUnitTurn) || (!IsEnemy() && isUnitTurn)) {
            hasMoved = false;
            hasPerformedAction = false;
            hasPerformedSkill = false;
            isUnitTurn = false;
            // OnAnyActionPerformed.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy() {
        return isEnemy;
    }

    public void Damage(int damage, Unit attackedBy = null) {
        healthSystem.Damage(damage, attackedBy);
    }

    public void AddXp(int xpAmount) {
        GameController.controller.UpdateUnitRecords(this);
        xpSystem.AddXp(xpAmount);
    }

    private void HealthSystem_OnDie(object sender, EventArgs e) {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.RemoveUnitFromList(this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() {
        return healthSystem.GetHealthPointsNormalized();
    }


    public Vector3 GetWorldPosition() { return transform.position; }

    public int GetUnitSpeed() { return unitStats.GetSpeed(); }

    public bool IsUnityTurn() { return isUnitTurn; }

    public void StartUnitTurn() {
        this.isUnitTurn = true;

        if (intimidateCoolDown != 0) {
            hasMoved = true;
            hasPerformedAction = true;
            hasPerformedSkill = true;
            intimidateCoolDown--;
        }

        if (enemyFocus != 0) {
            enemyFocus--;
        }

        for (int i = 0; i < actionsArray.Length; i++) {
            if (actionsArray[i].GetActionType() == ActionType.SKILL || actionsArray[i].GetActionType() == ActionType.ACTION) {
                actionsArray[i].IsAnotherRound();
            }
        }

        UnitActionSystem.Instance.ChangeSelectedUnit(this);
        OnAnyActionPerformed?.Invoke(this, EventArgs.Empty);

    }

    public string GetUnitId() { return this.unitId; }
    public XpSystem GetUnitXpSystem() { return this.xpSystem; }
    public UnitStats GetUnitStats() { return this.unitStats != null ? this.unitStats : this.baseUnitStats; }
    public UnitStats GetBaseUnitStats() { return this.baseUnitStats; }
    public void UpdateUnitStats(UnitStats unitStats) { this.unitStats = unitStats; }

    private void OnDestroy() {
        if (!isEnemy && GameController.controller.HasUnitRecords(unitId)) {
            GameController.controller.UpdateUnitRecords(this);
        }
    }

    public void BeIntimidate() {
        intimidateCoolDown = 1;
    }

    public void FocusOnMe(int focusTime) {
        enemyFocus = focusTime;
    }

    public bool GetEnemyFocus() {
        if (enemyFocus == 0) return false;
        else return true;
    }

    public List<BaseSkills> GetPossibleSkills() {
        return this.possibleSkills;
    }

    public List<PossibleUpgrade> GetPossibelUpgrades() { return this.possibleUpgrades; }

    public int GetHealthPoints() {
        return healthSystem.GetHealthPoints();
    }

    public string GetUnitName() { return this.unitName; }

    public void UpdateGridPositionZone(int zone) {
        this.gridPosition.zone = zone;
    }

    public HealthSystem GetHealthSystem() {
        return healthSystem;
    }

    public void PlayAnimation(string animation, bool isBooleanCondition = false) {
        this.animationTriggersStack.Add(animation, isBooleanCondition);
        if (this.animationTriggersStack.Count <= 1) {
            this.PlayNextAnimation();
        }
    }

    private void PlayNextAnimation() {
        KeyValuePair<string, bool> trigger = animationTriggersStack.ElementAt(0);
        animationTriggersStack.Remove(trigger.Key);
        if (animator == null) return;
        if (trigger.Value) {
            animator?.SetBool(trigger.Key, true);
        }
        else {
            animator?.SetTrigger(trigger.Key);
        }
    }

    public void EndAnimation(string animation, bool isBooleanCondition) {
        if (animator != null) {
            if (isBooleanCondition) {
                animator?.SetBool(animation, false);
            }
            else {
                animator?.ResetTrigger(animation);
            }
        }
        ;
        if (animationTriggersStack.Count > 0) {
            this.PlayNextAnimation();
        }
    }

    public UnitStatsModifiers GetModifiers() { return this.statsModifiers; }
    public void SubscribeToModifiedEvent(BaseSkills baseSkill) {
        baseSkill.onEndEffect += BaseSkill_onEndEffect;
    }

    private void BaseSkill_onEndEffect(object sender, EventArgs e) {
        if ((sender as BaseSkills).GetBuffType() != null) {
            statsModifiers.ResetModifier((BuffType)(sender as BaseSkills).GetBuffType());
        }
        (sender as BaseSkills).onEndEffect -= BaseSkill_onEndEffect;
    }
}
