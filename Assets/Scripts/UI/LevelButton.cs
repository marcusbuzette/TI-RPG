using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public int level;
    public string sceneToLoad;
    public string levelName;
    private Button levelButton;

    void OnEnable() {
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(() => {
            AudioManager.instance.PlayMusic("Combat");
            AudioManager.instance.PlayAmbient("AmbientFloresta");
            GameController.controller.uicontroller.ChangeScene(levelName);
        });
    }

}
