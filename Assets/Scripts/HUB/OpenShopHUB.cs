using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShopHUB : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera thisCamera;
    private BoxCollider thisCollider;
    bool isActive;
    private CameraHUB cameraHUB;
    [SerializeField] private GameObject store;

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
        yield return new WaitForSeconds(1.5f);
        DoSomething();
    }

    public void BackToMainCameraHUB() {
        cameraHUB.TurnOnAllColliders();
        thisCollider.enabled = true;
        thisCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        store.SetActive(false);
    }

    public void DoSomething() {
        store.SetActive(true);
    }
}
