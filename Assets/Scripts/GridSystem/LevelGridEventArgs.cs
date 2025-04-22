using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGridEventArgs : EventArgs {
    public Unit unit;
    public GridPosition currentGridPos;

    public LevelGridEventArgs(Unit unit, GridPosition gridPos) {
        this.unit = unit;
        this.currentGridPos = gridPos;
    }
}
