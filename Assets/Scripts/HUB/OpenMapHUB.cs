using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMapHUB : MonoBehaviour, IChangeCamera
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera thisCamera;
    [SerializeField] private BoxCollider thisCollider;
    bool isActive;
    [SerializeField] private string sceneToLoad;

    private CameraHUB cameraHUB;

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
        yield return new WaitForSeconds(2.5f);
        DoSomething();
    }

    public void BackToMainCameraHUB() {
        cameraHUB.TurnOnAllColliders();
        thisCollider.enabled = true;
        thisCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }

    public void DoSomething() {
        SceneManager.LoadScene(sceneToLoad);
    }
}
