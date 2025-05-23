using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenMapHUB : MonoBehaviour, IChangeCamera {
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera thisCamera;
    [SerializeField] private Canvas mapWorldCanvas;
    [SerializeField] private List<LevelButton> levelButtons;
    private BoxCollider thisCollider;
    bool isActive;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private FadingScript fadingScript;

    private CameraHUB cameraHUB;

    private void Start() {
        if (GetComponent<BoxCollider>() != null) {
            thisCollider = GetComponent<BoxCollider>();
        }
    }

    private void Update() {
        if (isActive) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackToMainCameraHUB();
            }
        }
    }

    public void EnterOnThisCamera(CameraHUB cameraHUB) {
        cameraHUB.TurnOffAllColliders();

        thisCollider.enabled = false;
        thisCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        isActive = true;

        this.mapWorldCanvas.enabled = true;
        this.CreateLevelButtons();

        this.cameraHUB = cameraHUB;

        // StartCoroutine(enumerator());
    }

    IEnumerator enumerator() {
        fadingScript.FadeIn();
        yield return new WaitForSeconds(2.5f);
        DoSomething();
    }

    public void BackToMainCameraHUB() {
        StopCoroutine(enumerator());
        cameraHUB.TurnOnAllColliders();
        thisCollider.enabled = true;
        thisCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }

    public void DoSomething() {
        AudioManager.instance.PlayMusic("Combat");
        AudioManager.instance.PlayAmbient("AmbientFloresta");
        SceneManager.LoadScene(sceneToLoad);
    }

    private void CreateLevelButtons() {
        foreach (LevelButton lb in levelButtons) {
            Debug.Log(lb.level);
            lb.gameObject.SetActive(lb.level <= GameController.controller.GetCurrentLevel());
        }
    }
}
