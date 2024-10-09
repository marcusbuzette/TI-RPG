using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour {

    private int turnNumber = 0;

    public static TurnSystem Instance {get; private set;}
    public event EventHandler onTurnChange;

    private void Awake() {
        if(Instance != null) {
            Debug.Log("More than one Turn System");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }



    public void NextTurn() {
        turnNumber++;
        onTurnChange.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber() {return turnNumber;}
}
