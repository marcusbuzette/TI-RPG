using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class MouseWorld : MonoBehaviour {

    static private MouseWorld instancce;

    [SerializeField] private LayerMask mousePlaneLayer;
    [SerializeField] private LayerMask unitLayer;

    bool isHide;

    private void Awake() {
        if (!instancce) {
            instancce = this;
        }
    }

    private void Update() {
        transform.position = MouseWorld.GetPosition() == null? Vector3.zero : MouseWorld.GetPosition();

        if (transform.position != Vector3.zero) {
            //Se o mouse está em cima da UI ele esconde o Visual do Mouse
            if (LevelGrid.Instance.GetGridPosition(transform.position) != null &&
            !EventSystem.current.IsPointerOverGameObject()) {
                GridSystemVisual.Instance.MousePosVisualHide(false);
                GridSystemVisual.Instance.MousePositionVisual(transform.position);
            }
        }
        else {
            GridSystemVisual.Instance.MousePosVisualHide(true);
        }

        if (LevelGrid.Instance.IsInBattleMode()) {
            if (TurnSystem.Instance.IsPlayerTurn()) GridSystemVisual.Instance.MousePosVisualHide(false);
            else GridSystemVisual.Instance.MousePosVisualHide(true);
        }
    }

    public static Vector3 GetPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hitOnUnit, float.MaxValue, instancce.unitLayer)) {
            return hitOnUnit.transform.position;
        }
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue,instancce. mousePlaneLayer );
        
        return hit.point;
    }
}
