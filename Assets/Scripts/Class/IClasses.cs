using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class IClasses : MonoBehaviour {
    [SerializeField] private UnitStats baseClassStats;

    private void Awake() {
        SetActions();
    }

    private void Start() {
    }

    public abstract void SetActions(); //each class has different actions

    public UnitStats GetUnitStats() {
        return baseClassStats;
    }
}