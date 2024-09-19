using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;

        UpdateVisal();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty) {
        UpdateVisal();
    }

    private void UpdateVisal() {
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit) {
            meshRenderer.enabled = true;
        }
        else {
            meshRenderer.enabled = false;
        }
    }
}
