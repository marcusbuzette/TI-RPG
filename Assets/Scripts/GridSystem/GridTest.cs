using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    [SerializeField] Transform gridDebugObjectPrefab;
    GridSystem gridSystem;

    private void Start() {

        gridSystem = new GridSystem(10, 10 , 2f);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Update() {

        //Debug.Log(gridSystem.GetGridPosition(MouseWorld.GetPosition()));
    }

    public GridSystem GetGridSystem() {
        return gridSystem;
    }

    public Vector3 MoveInGrid() {
        return new Vector3(gridSystem.GetGridPosition(MouseWorld.GetPosition()).x, 0, 0);
    }
}
