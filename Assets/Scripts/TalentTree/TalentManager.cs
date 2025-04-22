using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Linq;


public class TalentManager : MonoBehaviour {
    public static TalentManager Instance;
    [SerializeField] private Transform charactersContainer;
    [SerializeField] private Transform skillTreeContainer;
    [SerializeField] private Transform upgradesTreeContainer;
    [SerializeField] private Transform levelTransform;
    [SerializeField] private Button unitButtonPrefab;
    [SerializeField] private Button skillButtonPrefab;
    [SerializeField] private Button upgradeButtonPrefab;

    [SerializeField] private List<GameObject> playerUnitList = new List<GameObject>();

    public EventHandler onSkillUpdate;

    private List<BaseSkills> skills;
    private List<PossibleUpgrade> upgrades;
    private Dictionary<int, BaseSkills> selectedLevelSkill = new Dictionary<int, BaseSkills>();
    private int pontosDisponiveis = 0;
    public Text pontos;


    private String SelectedUnit;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this);
        }
    }

    private void OnEnable() {
        foreach (GameObject playerObj in playerUnitList) {
            Unit playerUnit = playerObj.GetComponent<Unit>();
            string unitId = playerUnit.GetUnitId();
            playerUnit.GetUnitXpSystem().ResetXP();
            if (SelectedUnit == null) SelectedUnit = unitId;
            if (!playerUnit.IsEnemy() && !GameController.controller.HasUnitRecords(unitId)) {
                GameController.controller.AddUnitToRecords(playerUnit);
            }
            else if (!playerUnit.IsEnemy()) {
                UnitRecords urAux = GameController.controller.GetUnitRecords(unitId);
                UpdateLocalUnitValues(unitId, urAux);
            }



            Button unitButton = Instantiate(unitButtonPrefab, charactersContainer);
            unitButton.gameObject.AddComponent<SkillTreeUnitButtonUI>();
            unitButton.gameObject.GetComponent<SkillTreeUnitButtonUI>().SetUnitData(unitId, playerUnit.GetUnitName());
            unitButton.onClick.AddListener(() => OnSelectedUnitChanged(unitId));

        }
        this.OnSelectedUnitChanged(this.SelectedUnit);
        if (this.onSkillUpdate != null) { this.onSkillUpdate.Invoke(this, EventArgs.Empty); }
    }

    //Verifica se existem pontos suficientes e todas as condições estão cumpridas para o desbloqueio do skills
    public void TentarDesbloquearskills(BaseSkills skills) {
        if (pontosDisponiveis >= skills.custo && PodeSerDesbloqueado(skills)) {
            Unit unitAux = playerUnitList.Find(unit => unit.GetComponent<Unit>().GetUnitId() == this.SelectedUnit).GetComponent<Unit>();
            this.UpdateLevelBar();
            DesbloquearSkills(skills);
        }
        else {
            Debug.Log("skills não pode ser desbloqueado!");
        }
    }

    public void TryToUpgrade(PossibleUpgrade upgrade, int index) {
        if (CanUpgrade(upgrade)) {
            Unit unitAux = playerUnitList.Find(unit => unit.GetComponent<Unit>().GetUnitId() == this.SelectedUnit).GetComponent<Unit>();
            this.UpdateLevelBar();
            DesbloquearUpgrade(upgrade, index);
        }
        else {
            Debug.Log("upgrade não pode ser desbloqueado!");
        }
    }

    //Muda o status do skills de desbloqueado para true
    public void DesbloquearSkills(BaseSkills skill) {
        AdicionarSkill(skill);
    }

    private void DesbloquearUpgrade(PossibleUpgrade upgrade, int index) {
        if (GameController.controller.HasUnitRecords(SelectedUnit)) {
            GameController.controller.AddUpgradeToRecordsById(SelectedUnit, upgrade, index);
            UpdateLocalUnitValues(this.SelectedUnit, GameController.controller.GetUnitRecords(this.SelectedUnit));
            this.onSkillUpdate.Invoke(this, EventArgs.Empty);
        }
    }

    //Verifica se será possível ser desbloqueado, para possível compra
    public bool PodeSerDesbloqueado(BaseSkills skills) {
        if (IsSkillUnlocked(skills)) {
            return false;
        }
        else if (!SkillHasRequirements(skills)) return true;

        return MatchRequirements(skills);
    }

    public bool CanUpgrade(PossibleUpgrade upgrade) {
        Unit unitAux = playerUnitList.Find(unit => unit.GetComponent<Unit>().GetUnitId() == this.SelectedUnit).GetComponent<Unit>();
        if (IsUpgradeLevelSelected(upgrade) || unitAux.GetUnitXpSystem().getXpAmount() < upgrade.level) return false;

        if (upgrades.FindIndex(u => u.level == upgrade.level) == 0) return true;


        return CheckPreviousUpgradesSelected(upgrade);
    }

    public void AdicionarSkill(BaseSkills skills) {
        if (GameController.controller.HasUnitRecords(SelectedUnit)) {
            GameController.controller.AddSkillToRecordById(SelectedUnit, skills);
            UpdateLocalUnitValues(this.SelectedUnit, GameController.controller.GetUnitRecords(this.SelectedUnit));
            this.onSkillUpdate.Invoke(this, EventArgs.Empty);
        }
    }

    public void OnSelectedUnitChanged(String unitId) {
        this.SelectedUnit = unitId;
        Unit unitAux = playerUnitList.Find(unit => unit.GetComponent<Unit>().GetUnitId() == this.SelectedUnit).GetComponent<Unit>();
        this.pontosDisponiveis = unitAux.GetUnitXpSystem().getXpAmount();
        this.UpdateLevelBar();
        this.UpdatedSkillTree(unitAux);

    }

    public int GetXPPoints() { return this.pontosDisponiveis; }

    private void UpdatedSkillTree(Unit unitAux) {
        foreach (Transform item in skillTreeContainer) {
            Destroy(item.gameObject);
        }

        foreach (Transform item in upgradesTreeContainer) {
            Destroy(item.gameObject);
        }
        UnitRecords unitRecordsAux = GameController.controller.GetUnitRecords(this.SelectedUnit);
        skills = unitAux.GetPossibleSkills();
        upgrades = unitAux.GetPossibelUpgrades();

        this.selectedLevelSkill.Clear();
        foreach (BaseSkills unitSkills in unitRecordsAux.GetUnitSKills()) {
            this.selectedLevelSkill.Add(unitSkills.custo, unitSkills);
        }

        foreach(BaseSkills bs in skills) {
            Button skillButton = Instantiate(skillButtonPrefab, skillTreeContainer);
            skillButton.GetComponent<SkillUi>().SetBaseSkill(bs);
            skillButton.GetComponent<SkillUi>().SetSkillToolTipPos(TooltipPosition.RIGHT);
            skillButton.onClick.AddListener(() => { TentarDesbloquearskills(bs); });
        }

        upgrades.Sort((a, b) => a.level.CompareTo(b.level));
        foreach (PossibleUpgrade possibleUpgrade in upgrades) {
            for (int i = 0; i < possibleUpgrade.upgrade.Length; i++) {
                Button upgradeButton = Instantiate(upgradeButtonPrefab, upgradesTreeContainer);
                upgradeButton.GetComponent<UpgradeUI>().SetBaseUpgrade(possibleUpgrade, i);
                upgradeButton.onClick.AddListener(() => { TryToUpgrade(possibleUpgrade, upgradeButton.GetComponent<UpgradeUI>().upgradeIndex); });

            }
        }
    }

    private bool IsSkillUnlocked(BaseSkills skill) {
        List<BaseSkills> unitSkills = GameController.controller.GetUnitRecords(this.SelectedUnit).GetUnitSKills();
        return unitSkills.Contains(skill);
    }

    private bool IsUpgradeLevelSelected(PossibleUpgrade upgrade) {
        Dictionary<int, int> unitUpgardes = GameController.controller.GetUnitRecords(this.SelectedUnit).GetLevelUpgrades();
        return unitUpgardes.Keys.Contains(upgrade.level);
    }

    private bool SkillHasRequirements(BaseSkills skill) {
        return (skill.preRequisitos != null && skill.preRequisitos.Count > 0);
    }

    private bool MatchRequirements(BaseSkills skill) {
        foreach (BaseSkills preRequisito in skill.preRequisitos) {
            if (IsSkillUnlocked(preRequisito)) {
                return true;
            }
        }
        return false;
    }

    public void UpdateLocalUnitValues(string unitId, UnitRecords unitRecords) {
        GameObject playerObj = playerUnitList.Find(p => p.GetComponent<Unit>().GetUnitId() == unitId);
        playerObj.GetComponent<Unit>().GetUnitXpSystem().SetXp(unitRecords.xp);
        playerObj.GetComponent<Unit>().UpdateUnitStats(unitRecords.GetUnitStats());
        this.OnSelectedUnitChanged(this.SelectedUnit);
    }

    private void UpdateLevelBar() {
        foreach (Transform levelBar in levelTransform) {
            levelBar.GetComponent<Slider>().value = this.pontosDisponiveis;
        }
    }

    public bool AlreadySelected(BaseSkills skill) {
        return GameController.controller.GetUnitRecords(this.SelectedUnit).baseSkills.Contains(skill);
    }

    public bool AlreadyUpgraded(PossibleUpgrade upgrade, int index) {
        Dictionary<int, int> unitUpgardes = GameController.controller.GetUnitRecords(this.SelectedUnit).GetLevelUpgrades();
        if (!unitUpgardes.Keys.Contains(upgrade.level)) return false;
        return unitUpgardes[upgrade.level] == index;
    }

    public bool CheckSelectedSkillOnLevel(int level) {
        return this.selectedLevelSkill.Keys.Contains(level);
    }

    private bool CheckPreviousUpgradesSelected(PossibleUpgrade upgrade) {
        Dictionary<int, int> unitUpgardes = GameController.controller.GetUnitRecords(this.SelectedUnit).GetLevelUpgrades();
        List<int> previousLevels = new List<int>();
        foreach (PossibleUpgrade u in upgrades) {
            if (u.level < upgrade.level) previousLevels.Add(u.level);
        }

        foreach (int level in previousLevels) {
            if (!unitUpgardes.Keys.Contains(level)) return false;
        }

        return true;
    }

}