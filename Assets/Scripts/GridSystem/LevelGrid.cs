using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelGrid : MonoBehaviour {
    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMovedGridPosition;
    public event EventHandler OnGameModeChanged;

    [SerializeField] Transform gridDebugObjectPrefab;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;

    private GridSystem<GridObject> gridSystem;

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
                    zoneList.Add(new GridPosition(x, z, item.zoneNumber));
                }
            }
            List<GridPosition> spListAux = new List<GridPosition>();
            foreach (ZoneSpawnPoint sp in item.spawnPoints) {
                spListAux.Add(new GridPosition(sp.x, sp.z, item.zoneNumber));
            }
            zoneStartPositions.Add(item.zoneNumber, spListAux);
        }

        gridSystem = new GridSystem<GridObject>(width, height, cellSize,
                (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition), zoneList);

        if (GameController.controller.GetDebugMode()) gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start() {
        this.currentBattleZone = 0;
        PathFinding.Instance.Setup(width, height, cellSize);
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
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();

    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
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

    public GameMode GetGameMode() { return gameMode; }

    public List<GridPosition> GetZoneList() { return this.zoneList; }
    public int GetCurrentBattleZone() { return this.currentBattleZone; }
    public void SetCurrentBattleZone(int zone) {this.currentBattleZone = zone;}

    public List<GridPosition> GetZoneSpawnList(int zone) {
        return this.zoneStartPositions[zone];
    }

    public void RemoveZoneFromGrid(int zone) {
        this.gridSystem.RemoveZone(zone);
    }


}
