using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShopHUB : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera thisCamera;
    [SerializeField] private BoxCollider thisCollider;
    bool isActive;

    private void Update() {
        if (isActive) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackToMainCameraHUB();
            }
        }
    }

    public void EnterOnThisCamera() {
        thisCollider.enabled = false;
        thisCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        isActive = true;
        DoSomething();
    }
    public void BackToMainCameraHUB() {
        thisCollider.enabled = true;
        thisCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        //Close Skill Tree
        Debug.Log("Fechar Loja");
    }

    public void DoSomething() {
        //Open Skill Tree
        Debug.Log("Abrir Loja");
    }
}
