using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRecords {
    public int xp;
    public UnitStats unitStats;
    public List<BaseSkills> baseSkills;


    public UnitRecords(int xp, UnitStats unitStats) {
        this.xp = xp;
        this.unitStats = unitStats;
        this.baseSkills = new List<BaseSkills>();
    }

    public void AddSkill(BaseSkills skill) {
        baseSkills.Add(skill);
    }

    public List<BaseSkills> GetUnitSKills() {return this.baseSkills;}
}
