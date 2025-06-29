using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CloseUpCharCam : MonoBehaviour {

    List<CloseUpChar> unitList = new List<CloseUpChar>();

    void Awake() {
        foreach (Transform child in transform) {
            if (child.gameObject.layer == LayerMask.NameToLayer("Victory") ) {
                unitList.Add(child.gameObject.GetComponent<CloseUpChar>());
            }
        }
        Debug.Log(unitList.Count);
    }


    public void ShowUnit(string unitId) {
        foreach (CloseUpChar unit in unitList) {
            unit.gameObject.SetActive(unit.unitId == unitId);
        }
    }

}
