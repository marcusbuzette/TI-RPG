using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigButtons : MonoBehaviour {

    public void CharButton() {
        GameController.controller.ToggleCharsPanel();
    }

    public void PauseButton() {
        GameController.controller.PauseGame();
    }
}
