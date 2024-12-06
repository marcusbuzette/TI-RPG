using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHUB : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

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
                        hit.transform.gameObject.GetComponent<OpenCharacterSkillTreeHUB>().EnterOnThisCamera();
                        break;
                    case "HUB_Map":
                        hit.transform.gameObject.GetComponent<OpenMapHUB>().EnterOnThisCamera();
                        break;
                    case "HUB_Shop":
                        hit.transform.gameObject.GetComponent<OpenShopHUB>().EnterOnThisCamera();
                        break;
                    case "HUB_Campfire":

                        break;
                }
            }
        }
    }
}
