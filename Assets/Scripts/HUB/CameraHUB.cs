using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraHUB : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private OpenCharacterSkillTreeHUB[] charactersSkillTrees;
    [SerializeField] private BoxCollider[] colliders;
    [SerializeField] private GameObject[] texts;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                switch (hit.transform.tag) {
                    case "HUB_Character":
                        hit.transform.gameObject.GetComponent<OpenCharacterSkillTreeHUB>().EnterOnThisCamera(this);
                        break;
                    case "HUB_Map":
                        hit.transform.gameObject.GetComponent<OpenMapHUB>().EnterOnThisCamera(this);
                        break;
                    case "HUB_Shop":
                        hit.transform.gameObject.GetComponent<OpenShopHUB>().EnterOnThisCamera(this);
                        break;
                    case "HUB_Campfire":
                        hit.transform.gameObject.GetComponent<DialogueTrigger>().TriggerDialogue();
                        break;
                }
            }
        }
    }

    public void AddSkillTree(OpenCharacterSkillTreeHUB skillTree, int index) {
        charactersSkillTrees[index] = skillTree;
    }

    public OpenCharacterSkillTreeHUB GetSkillTreeCharacter(int index) {
        return charactersSkillTrees[index];
    }

    public void TurnOffAllColliders() {
        foreach(BoxCollider collider in colliders) {
            collider.enabled = false;
        }
        foreach (GameObject text in texts) {
            text.SetActive(false);
        }
    }

    public void TurnOnAllColliders() {
        foreach (BoxCollider collider in colliders) {
            collider.enabled = true;
        }
        foreach (GameObject text in texts) {
            text.SetActive(true);
        }
    }
}
