using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameController controller;
    public UIController uicontroller;

    private void Awake() {
        if (controller == null) {
            controller = this;
            DontDestroyOnLoad(this);
        }
        else {
            DestroyImmediate(gameObject);
        }
    }

    void Start() {

    }


    void Update() {

    }
}
