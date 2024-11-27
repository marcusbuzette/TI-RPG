using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class OutlineOnHover : MonoBehaviour {

    private Outline outline;

    private void Awake() {
        outline = GetComponent<Outline>();
        if (outline == null) {
            Debug.Log("Missing Outline Coponent!");
        }
    }

    private void OnMouseEnter() {
        this.outline.enabled = true;
    }
    
    private void OnMouseExit() {
        this.outline.enabled = false;
    }
}
