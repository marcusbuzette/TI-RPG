using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRecords {
    public int xp;
    public UnitStats unitStats;


    public UnitRecords(int xp, UnitStats unitStats) {
        this.xp = xp;
        this.unitStats = unitStats;
    }
}
