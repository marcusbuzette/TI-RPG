using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PathFinding : MonoBehaviour {

    public static PathFinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugPrefab;
    [SerializeField] private LayerMask obstaclesLayers;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private Transform pathFindingLinkContainer;

    private int width;
    private int height;
    private float cellSize;
    private int floorAmount;
    private List<GridSystem<PathNode>> gridSystemList;
    private List<PathFindingLink> pathFindingLinkList;

    private void Awake() {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }

    public void Setup(int width, int height, float cellSize, int floorAmount, List<GridPosition> zoneList) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floorAmount = floorAmount;

        gridSystemList = new List<GridSystem<PathNode>>();

        for (int floor = 0; floor < floorAmount; floor++) {
            GridSystem<PathNode> gridSystem = new GridSystem<PathNode>(width, height, cellSize, floor, LevelGrid.FLOOR_HEIGHT,
                (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition), zoneList);

            if (GameController.controller.GetPathFindingDebugMode()) gridSystem.CreateDebugObjects(gridDebugPrefab);

            gridSystemList.Add(gridSystem);
        }

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                for (int floor = 0; floor < floorAmount; floor++) {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    float raycastOffsetDistance = 1f;

                    GetNode(x, z, floor).SetIsWalkable(false);

                    if (Physics.Raycast(
                        worldPosition + Vector3.up * raycastOffsetDistance,
                        Vector3.down,
                        raycastOffsetDistance * 2,
                        floorLayerMask)) {
                        GetNode(x, z, floor).SetIsWalkable(true);
                    }

                    if (Physics.Raycast(
                        worldPosition + Vector3.down * raycastOffsetDistance,
                        Vector3.up,
                        raycastOffsetDistance * 2,
                        obstaclesLayers)) {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                }
            }
        }

        pathFindingLinkList = new List<PathFindingLink>();
        foreach (Transform pathFindingLinkTransform in pathFindingLinkContainer) {
            if (pathFindingLinkTransform.TryGetComponent(out PathFindingLinkMonoBehaviour pathFindingLinkMonoBehaviour)) {
                pathFindingLinkList.Add(pathFindingLinkMonoBehaviour.GetPathFindingLink());
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLenght) {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                for (int floor = 0; floor < floorAmount; floor++) {
                    GridPosition gridPosition = new GridPosition(x, z, 0);
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();
                }
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode) {
                //Reached final node
                pathLenght = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) {
                    continue;
                }

                if (!neighbourNode.IsWalkable()) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost =
                    currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost < neighbourNode.GetGCost()) {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        //No path found
        pathLenght = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB) {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()) {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

    private GridSystem<PathNode> GetGridSystem(int floor) {
        return gridSystemList[floor];
    }

    private PathNode GetNode(int x, int z, int floor) {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0) {
            // Left node

            if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE ||
                GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor));
            }
            if (gridPosition.z - 1 >= 0) {
                // Left Down node
                if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE ||
                    GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));
                }
            }
            if (gridPosition.z + 1 < height) {
                // Left Up node

                if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE ||
                    GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
                }
            }
        }

        if (gridPosition.x + 1 < width) {
            // Right node
            if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE ||
                GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor));
            }
            if (gridPosition.z - 1 >= 0) {
                // Right Down node
                if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE ||
                    GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
                }
            }
            if (gridPosition.z + 1 < height) {
                // Right Up node
                if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE ||
                    GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
                }
            }
        }

        if (gridPosition.z - 1 >= 0) {
            // Down node
            if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE ||
                GetNode(gridPosition.x - 0, gridPosition.z - 1, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                neighbourList.Add(GetNode(gridPosition.x - 0, gridPosition.z - 1, gridPosition.floor));
            }
        }

        if (gridPosition.z + 1 < height) {
            // Up node
            if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE || GetNode(gridPosition.x - 0, gridPosition.z + 1, gridPosition.floor).GetGridPosition().zone == gridPosition.zone) {
                neighbourList.Add(GetNode(gridPosition.x - 0, gridPosition.z + 1, gridPosition.floor));
            }
        }

        List<PathNode> totalNeighbourList = new List<PathNode>();
        totalNeighbourList.AddRange(neighbourList);

        List<GridPosition> pathFindingLinkGridPostionList = GetPathFindingLinkConnectedGridPositonList(gridPosition);

        foreach (GridPosition pathFindingLinkGridPostion in pathFindingLinkGridPostionList) {
            totalNeighbourList.Add(
                GetNode(
                    pathFindingLinkGridPostion.x,
                    pathFindingLinkGridPostion.z,
                    pathFindingLinkGridPostion.floor
                    )
                );
        }

        return totalNeighbourList;
    }

    private List<GridPosition> GetPathFindingLinkConnectedGridPositonList(GridPosition gridPosition) {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathFindingLink pathFindingLink in pathFindingLinkList) {
            if (pathFindingLink.gridPositionA == gridPosition) {
                gridPositionList.Add(pathFindingLink.gridPositionB);
            }
            if (pathFindingLink.gridPositionB == gridPosition) {
                gridPositionList.Add(pathFindingLink.gridPositionA);
            }
        }

        return gridPositionList;
    }

    private List<GridPosition> CalculatePath(PathNode endPath) {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endPath);
        PathNode currentNode = endPath;
        while (currentNode.GetCameFromPathNode() != null) {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList) {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) {
        return FindPath(startGridPosition, endGridPosition, out int pathLenght) != null;
    }

    public int GetPathLenght(GridPosition startGridPosition, GridPosition endGridPosition) {
        FindPath(startGridPosition, endGridPosition, out int pathLenght);
        return pathLenght;
    }
}
