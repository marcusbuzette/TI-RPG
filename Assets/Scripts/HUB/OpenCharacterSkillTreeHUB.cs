using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OpenCharacterSkillTreeHUB : MonoBehaviour, IChangeCamera {

    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera thisCamera;
    
    private BoxCollider thisCollider;
    private bool isActive;

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
        cameraHUB.TurnOnAllColliders();

        if (thisCollider != null) {
            thisCollider.enabled = true;
        }
        thisCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        skillTree.SetActive(false);
    }

    public void DoSomething() {
        skillTree.SetActive(true);
    }

    public void NextSkillTree() {
        int nextIndex = index;
        nextIndex = nextIndex + 1;
        if (nextIndex > 3) {
            nextIndex = 0;
        }

        BackToMainCameraHUB();
        cameraHUB.GetSkillTreeCharacter(nextIndex).EnterOnThisCamera(cameraHUB);
    }

    public void BackSkillTree() {
        int nextIndex = index;
        nextIndex = nextIndex - 1;
        if (nextIndex < 0) {
            nextIndex = 3;
        }

        BackToMainCameraHUB();
        cameraHUB.GetSkillTreeCharacter(nextIndex).EnterOnThisCamera(cameraHUB);
    }
}
