using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeObject", menuName = "UpgradeObjectSkillTree")]
public class UpgradeObject : ScriptableObject {
    public string id;
    public string name;
    public string description;
    public UpgradeType upgradeType;
    public int upgradeAmount;
    
}

public enum UpgradeType {
    HEALTH,
    DEFENCE,
    ATTACK,
    MOVEMENT,
    ACCURACY,
    SPEED,
    RANGE
}
