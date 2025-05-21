using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMapHUB : MonoBehaviour, IChangeCamera
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera thisCamera;
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

        this.cameraHUB = cameraHUB;

        StartCoroutine(enumerator());
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
}
