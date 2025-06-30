using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineSceneLoader : MonoBehaviour {
    public string nextScene = "MenuPrincipal";

    public void LoadScene() {
        SceneManager.LoadScene(nextScene);
    }
}
