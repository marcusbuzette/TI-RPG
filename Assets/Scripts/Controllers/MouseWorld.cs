using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour {

    static private MouseWorld instancce;

    [SerializeField] private LayerMask mousePlaneLayer;

    private void Awake() {
        if (!instancce) {
            instancce = this;
        }
    }

    private void Update() {
        transform.position = MouseWorld.GetPosition();
    }

    public static Vector3 GetPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue,instancce. mousePlaneLayer );
        return hit.point;
    }

}
