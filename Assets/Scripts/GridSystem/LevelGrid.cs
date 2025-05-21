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
    [SerializeField] private Quest levelQuest;

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
                spListAux.Add(new GridPosition(sp.x, sp.z, sp.floor, item.zoneNumber));
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
        QuestManager.Instance.SetLevelQuest(this.levelQuest);
        QuestManager.Instance.StartQuest();
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
    public AddSquaredZone GetCurrentSquaredZone(int battleZone) {
        foreach(AddSquaredZone zone in squaredZoneList) {
            if (zone.zoneNumber == battleZone) {
                return zone;
            }
        }

        return null;
    }
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

    public List<Vector3> GetPositionsBehindUnit(Unit unit, int numberOfPositions) {
        List<Vector3> followPositions = new List<Vector3>();
        List<Vector3> path = unit.GetComponent<MoveAction>().GetMovePathList();

        if (path.Count < 2)
            return followPositions;

        Vector3 last = path[path.Count - 1];
        Vector3 secondLast = path[path.Count - 2];
        Vector3 direction = (last - secondLast).normalized;

        // Direção invertida (atrás)
        Vector3 behindDirection = -direction;

        GridPosition centerPos = LevelGrid.Instance.GetGridPosition(last);

        int count = 0;
        int radius = 1;

        while (count < numberOfPositions) {
            for (int dx = -radius; dx <= radius; dx++) {
                for (int dz = -radius; dz <= radius; dz++) {
                    // Posição candidata relativa à posição final da unidade
                    GridPosition candidate = new GridPosition(
                        centerPos.x + dx,
                        centerPos.z + dz,
                        centerPos.floor,
                        centerPos.zone
                    );

                    Vector3 dirToCandidate = new Vector3(dx, 0, dz).normalized;
                    float dot = Vector3.Dot(behindDirection, dirToCandidate);

                    // Considera apenas posições mais "atrás"
                    if (dot < 0.5f) continue;

                    if (IsValidGridPosition(candidate)) {
                        Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(candidate);
                        followPositions.Add(worldPos);
                        count++;

                        if (count >= numberOfPositions)
                            return followPositions;
                    }
                }
            }

            radius++;
        }

        return followPositions;
    }

    public bool IsInBattleMode() {
        if(gameMode == GameMode.BATTLE) return true;
        return false;
    }
}
