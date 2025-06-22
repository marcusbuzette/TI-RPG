using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitRecords {
    public int xp;
    public UnitStats unitStats;
    public List<BaseSkills> baseSkills;
    public List<string> baseSkillsId;
    public SerializableDictionary<int, int> levelUpgrades;


    public UnitRecords(int xp, UnitStats unitStats, List<BaseSkills> baseSkills = null, List<string> baseSkillsId = null,
         SerializableDictionary<int, int> levelUpgrades = null) {
        this.xp = xp;
        this.unitStats = unitStats;
        // this.baseSkills = baseSkills != null ? baseSkills : new List<BaseSkills>();
        this.baseSkillsId = baseSkillsId != null ? baseSkillsId : new List<string>();
        this.levelUpgrades = levelUpgrades != null ? levelUpgrades : new SerializableDictionary<int, int>();
    }

    public void AddSkill(BaseSkills skill) {
        if (!baseSkillsId.Contains(skill.nome)) {
            // baseSkills.Add(skill);
            baseSkillsId.Add(skill.nome);
        }

    }

    public List<BaseSkills> GetUnitSKills() {
        return this.baseSkills;
    }

    public List<string> GetUnitSKillsIDs() {
        return this.baseSkillsId;
    }

    public void AddLevelUpgrade(int level, int chosenIndex, UpgradeObject upgrade) {
        switch (upgrade.upgradeType) {
            case UpgradeType.HEALTH:
                unitStats.UpgradeHP(upgrade.upgradeAmount);
                break;
            case UpgradeType.ACCURACY:
                unitStats.UpgradeAccuracy(upgrade.upgradeAmount);
                break;
            case UpgradeType.ATTACK:
                unitStats.UpgradeAttack(upgrade.upgradeAmount);
                break;
            case UpgradeType.DEFENCE:
                unitStats.UpgradeDefence(upgrade.upgradeAmount);
                break;
            case UpgradeType.MOVEMENT:
                unitStats.UpgradeMove(upgrade.upgradeAmount);
                break;
            case UpgradeType.SPEED:
                unitStats.UpgradeSpeed(upgrade.upgradeAmount);
                break;
            default:
                break;
        }
        this.levelUpgrades.Add(level, chosenIndex);
    }

    public SerializableDictionary<int, int> GetLevelUpgrades() { return this.levelUpgrades; }

    public UnitStats GetUnitStats() { return unitStats; }
}
