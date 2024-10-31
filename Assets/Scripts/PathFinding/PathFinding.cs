using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    [SerializeField] private Transform gridDebugPrefab;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

    private void Awake() {
        gridSystem = new GridSystem<PathNode>(16, 15, 2f,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));

        if (GameController.controller.GetPathFindingDebugMode()) gridSystem.CreateDebugObjects(gridDebugPrefab);
    }
}
