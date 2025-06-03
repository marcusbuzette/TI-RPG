using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public int level;
    public string sceneToLoad;
    public string levelName;
    private Button levelButton;

    [SerializeField] private FadingScript fadingScript;

    void OnEnable() {
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(() => {
            fadingScript.FadeIn(sceneToLoad);
            AudioManager.instance.PlayMusic("Combat");
            AudioManager.instance.PlayAmbient("AmbientFloresta");
       
        });
    }
}
