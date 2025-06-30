using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineSceneLoader : MonoBehaviour {
    public string nextScene = "MenuPrincipal";
    private float musicVolume;

    private void OnEnable() {
        this.musicVolume = AudioManager.instance.GetComponent<AudioSource>().volume;
        AudioManager.instance?.MusicVolume(0);
    }

    public void LoadScene() {
        AudioManager.instance.MusicVolume(this.musicVolume);
        SceneManager.LoadScene(nextScene);
    }
}
