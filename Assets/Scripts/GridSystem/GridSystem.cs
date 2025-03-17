using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class GridSystem<TGridObject> {
    private int width;
    private int height;
    private float cellSize;
    private int floor;
    private float floorHeight;
    private TGridObject[,] gridObjectArray;
    private GridPosition[,] gridPositionList;

    public GridSystem(int width, int height, float cellSize, int floor, float floorHeight, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject,
         List<GridPosition> zoneList = null) {

        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floor = floor;
        this.floorHeight = floorHeight;

        this.gridObjectArray = new TGridObject[width, height];
        this.gridPositionList = new GridPosition[width, height];

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition gridPosition = new GridPosition(x, z, floor);
                if (zoneList != null) {
                    GridPosition zoneItem = zoneList.Find(pos => pos.x == x && pos.z == z);
                    if (zoneItem != null) gridPosition.zone = zoneItem.zone;
                }
                gridObjectArray[x, z] = createGridObject(this, gridPosition);
                gridPositionList[x, z] = gridPosition;
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) {

        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize +
            new Vector3(0, gridPosition.floor, 0) * floorHeight;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        int auxX = Mathf.RoundToInt(worldPosition.x / cellSize);
        int auxZ = Mathf.RoundToInt(worldPosition.z / cellSize);

        /*if (auxX < 0) {
            auxX = 0;
        }
        if (auxZ < 0){
            auxZ = 0;
        }*/
        
        return gridPositionList[auxX, auxZ];
    }

    public void CreateDebugObjects(Transform debugPrefab) {

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition gridPosition = new GridPosition(x, z, floor);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();

                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition) {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition) {
        return gridPosition.x >= 0 &&
                gridPosition.z >= 0 &&
                gridPosition.x < width &&
                gridPosition.z < height &&
                gridPosition.floor == floor; ;
    }

    public void RemoveZone(int zone) {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                if (gridPositionList[x, z].zone == zone) {
                    gridPositionList[x, z].zone = 0;
                }
            }
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }
}
