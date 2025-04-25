using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelGrid : MonoBehaviour {
    public static LevelGrid Instance { get; private set; }

    public const float FLOOR_HEIGHT = 2f;

    public event EventHandler OnAnyUnitMovedGridPosition;
    public event EventHandler OnGameModeChanged;

    [SerializeField] Transform gridDebugObjectPrefab;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private int floorAmount;

    [SerializeField] private List<GridSystem<GridObject>> gridSystemList;

    public enum GameMode { BATTLE, EXPLORE }

    [SerializeField] private GameMode gameMode = GameMode.BATTLE;
    [SerializeField] private List<AddSquaredZone> squaredZoneList = new List<AddSquaredZone>();
    [SerializeField] private List<GridPosition> zoneList = new List<GridPosition>();
    private Dictionary<int, List<GridPosition>> zoneStartPositions = new Dictionary<int, List<GridPosition>>();
    private int currentBattleZone = 0;


    private void Awake() {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

        foreach (AddSquaredZone item in squaredZoneList) {
            for (int x = item.startX; x <= item.endX; x++) {
                for (int z = item.startZ; z <= item.endZ; z++) {
                    zoneList.Add(new GridPosition(x, z, item.floor, item.zoneNumber));
                }
            }
            List<GridPosition> spListAux = new List<GridPosition>();
            foreach (ZoneSpawnPoint sp in item.spawnPoints) {
                spListAux.Add(new GridPosition(sp.x, sp.z, sp.floor ,item.zoneNumber));
            }
            zoneStartPositions.Add(item.zoneNumber, spListAux);
        }


        gridSystemList = new List<GridSystem<GridObject>>();

        for (int floor = 0; floor < floorAmount; floor++) {
            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT,
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition), zoneList);
            if (GameController.controller.GetDebugMode()) gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

            gridSystemList.Add(gridSystem);
        }
    }

    private void Start() {
        this.currentBattleZone = 0;
        PathFinding.Instance.Setup(width, height, cellSize, floorAmount, zoneList);
        OnGameModeChanged?.Invoke(this, EventArgs.Empty);
    }

    private GridSystem<GridObject> GetGridSystem(int floor) {
        return floor < gridSystemList.Count ? gridSystemList[floor] : gridSystemList[0];
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            this.ExploreMode();
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            this.BattleMode();
        }
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, new LevelGridEventArgs(unit, toGridPosition));
    }

    public int GetFloor(Vector3 worldPosition) {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }
    public GridPosition GetGridPosition(Vector3 worldPosition) {
        int floor = GetFloor(worldPosition);
        return GetGridSystem(floor).GetGridPosition(worldPosition);
    }
    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) {
        if (gridPosition.floor < 0 || gridPosition.floor >= floorAmount) return false;
        else return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
    }

    public int GetWidth() => GetGridSystem(0).GetWidth();

    public int GetHeight() => GetGridSystem(0).GetHeight();

    public int GetFLoorAmount() => floorAmount;

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public void ExploreMode() {
        this.gameMode = GameMode.EXPLORE;
        OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        GridSystemVisual.Instance.UpdateGridVisual();
    }

    public void BattleMode(int zone = 0) {
        this.currentBattleZone = zone;
        TurnSystem.Instance.SetUpBattleNewZone();
        this.gameMode = GameMode.BATTLE;
        OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        GridSystemVisual.Instance.UpdateGridVisual();
        TurnSystem.Instance.StartBattleNewZone();

    }

    public void OnGameModeChangedTest() {
        OnGameModeChanged?.Invoke(this, EventArgs.Empty);
    }

    public GameMode GetGameMode() { return gameMode; }

    public List<GridPosition> GetZoneList() { return this.zoneList; }
    public int GetCurrentBattleZone() { return this.currentBattleZone; }
    public void SetCurrentBattleZone(int zone) { this.currentBattleZone = zone; }

    public List<GridPosition> GetZoneSpawnList(int zone) {
        return this.zoneStartPositions[zone];
    }

    public void RemoveZoneFromGrid(int zone) {
        foreach (GridSystem<GridObject> gridSystem in gridSystemList) {
            gridSystem.RemoveZone(zone);
        }
    }
}
