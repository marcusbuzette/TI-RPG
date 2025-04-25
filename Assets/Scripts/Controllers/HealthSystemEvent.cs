using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystemEvent : EventArgs {
    public Unit attacker;

    public HealthSystemEvent(Unit attacker) {
        this.attacker = attacker;
    }
}
