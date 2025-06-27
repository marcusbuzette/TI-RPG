using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using System;

public class OpenCharacterSkillTreeHUB : MonoBehaviour, IChangeCamera {

    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera thisCamera;
    
    private BoxCollider thisCollider;
    private bool isActive;

    public EventHandler OpenSkillTree;

    private CameraHUB cameraHUB;
    [SerializeField] private GameObject skillTree;
    [SerializeField] private int index;

    private void Start() {
        if(GetComponent<BoxCollider>() != null) {
            thisCollider = GetComponent<BoxCollider>();
        }
    }

    private void Update() {
        if (isActive) {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                BackToMainCameraHUB();
            }
        }
    }

    public void EnterOnThisCamera(CameraHUB cameraHUB) {
        cameraHUB.TurnOffAllColliders();

        if (thisCollider != null) {
            thisCollider.enabled = false;
        }
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
        if(this.cameraHUB == null) {
            cameraHUB = TalentManager.Instance.GetCameraHUB();
        }

        cameraHUB.TurnOnAllColliders();

        if (thisCollider != null) {
            thisCollider.enabled = true;
        }
        thisCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        skillTree?.SetActive(false);
    }

    public void CloseCamera() {
        thisCamera.gameObject.SetActive(false);
    }

    public void DoSomething() {
        TalentManager.Instance.UpdateSelectedCharButton();
        if (!TutorialManager.Instance.IsTutorialFinished() 
        && TutorialManager.Instance.IsWaitingStep() && !TutorialManager.Instance.HasShownSkillTree()) {
            TutorialManager.Instance.ShowSkillTreeStep();
        }
    }

    public void SkillTreeOpen() {
        skillTree?.SetActive(true);
    }
}
