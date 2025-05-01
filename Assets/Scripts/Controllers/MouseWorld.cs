using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour {

    static private MouseWorld instancce;

    [SerializeField] private LayerMask mousePlaneLayer;
    [SerializeField] private LayerMask unitLayer;

    private void Awake() {
        if (!instancce) {
            instancce = this;
        }
    }

    private void Update() {
        transform.position = MouseWorld.GetPosition();
        GridSystemVisual.Instance.MousePositionVisual(transform.position);
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
