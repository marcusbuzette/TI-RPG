using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitRecords {
    public int xp;
    public UnitStats unitStats;
    public List<BaseSkills> baseSkills;
    public SerializableDictionary<int, int> levelUpgrades;


    public UnitRecords(int xp, UnitStats unitStats, List<BaseSkills> baseSkills = null,
         SerializableDictionary<int, int> levelUpgrades = null) {
        this.xp = xp;
        this.unitStats = unitStats;
        this.baseSkills = baseSkills != null ? baseSkills : new List<BaseSkills>();
        this.levelUpgrades = levelUpgrades != null ? levelUpgrades : new SerializableDictionary<int, int>();
    }

    public void AddSkill(BaseSkills skill) {
        baseSkills.Add(skill);
    }

    public List<BaseSkills> GetUnitSKills() {
        return this.baseSkills;
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
    public UnitStats GetUnitBaseStats() { 
        BaseUnitStats aux = unitStats.GetBaseUnitStats(); 

        return  new UnitStats(
            aux.speed, aux.attack, aux.defence, aux.accuracy,
            aux.xpSpoil, aux.maxHealthPoints, aux.maxMove, aux.range, aux
        );

    }
}
