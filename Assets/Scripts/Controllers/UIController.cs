using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _masterSlider, _musicSlider, _sfxSlider;

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

    public void MusicVolume()
    {
        AudioManager.instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(_sfxSlider.value);
    }

    public void MasterVolume()
    {
        AudioManager.instance.MasterVolume(_masterSlider.value);
    }

    public void ChangeScene(string scene) {
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
