using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public static UIController instance;
    public Slider _masterSlider, _musicSlider, _sfxSlider, _ambientSlider;

    private void Awake() {
        // if (instance == null) {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else {
        //     Destroy(gameObject);
        // }
        GameController.controller.uicontroller = this;

    }

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void ToggleMaster()
    {
        AudioManager.instance.ToggleMaster();
    }

    public void ToggleAmbient() {
        AudioManager.instance.ToggleAmbient();
    }

    public void MusicVolume()
    {
        AudioManager.instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(_sfxSlider.value);
    }

    public void AmbientVolume() {
        AudioManager.instance.AmbientVolume(_ambientSlider.value);
    }

    public void MasterVolume()
    {
        AudioManager.instance.MasterVolume(_masterSlider.value);
    }

    public void ChangeScene(string scene) {
        
        string musicToPlay = ""; // Nome da m�sica da pr�xima cena
        string ambientToPlay = "";
        switch (scene) {
            case "HUB":
                musicToPlay = "HUB"; // Substitua com o nome da m�sica
                ambientToPlay = "aHUB";
                break;
            case "MenuPrincipal":
                musicToPlay = "Menu"; // M�sica do menu
                break;
            /*case "Tutorial":
                musicToPlay = "Combat"; // M�sica padr�o, se necess�rio
                break;*/
        }
        
        AudioManager.instance?.PlayMusic(musicToPlay);
        AudioManager.instance?.PlayAmbient(ambientToPlay);
        SceneManager.LoadScene(scene);

    } 

    public void PlayGame() {
        //Muda pra cena do jogo, lembrar de mudar o nome aqui quando criar a cena definitiva
        SceneManager.LoadScene("Playground");
    }

    public void FecharJogo() {
        // Fechar o jogo
        Application.Quit();

        // Para testes dentro do Editor (opcional)
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

}
