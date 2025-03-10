using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour {
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType {
        White,
        Blue,
        BlueSoft,
        Red,
        RedSoft,
        Yellow
    }
    [SerializeField] private Transform gridSystemVisualPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    public void Awake() {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start() {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
            ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleArrayTransform =
                    Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleArrayTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedUnitChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    public void HideAllGridPosition() {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {

                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType) {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++) {
            for (int z = -range; z <= range; z++) {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range) {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType) {
        foreach (GridPosition gridPosition in gridPositionList) {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void UpdateGridVisual() {
        HideAllGridPosition();

        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.BATTLE) {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

            if (selectedAction == null || selectedAction.GetActionType() == ActionType.INVENTORY) return;

            GridVisualType gridVisualType = GridVisualType.White;
            switch (selectedAction) {
                default:
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                    break;
                case ItemAction itemAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case HitAction hitAction:
                    gridVisualType = GridVisualType.Red;
                    break;
                case TeleportSkill teleportSkill:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case IntimidateSkill intimidateSkill:
                    gridVisualType = GridVisualType.Blue;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), intimidateSkill.GetMaxIntimidateDistance(), GridVisualType.BlueSoft);
                    break;
                case EnemyFocusSkill enemyFocusSkill:
                    gridVisualType = GridVisualType.Blue;
                    break;
            }
            ShowGridPositionList(selectedAction.GetValidGridPositionList(), gridVisualType);
        }
        else {
            // GridVisualType gridVisualType = GridVisualType.White;
            // for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            //     for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {

            //         gridSystemVisualSingleArray[x, z].Show(GetGridVisualTypeMaterial(gridVisualType));
            //     }
            // }
        }

    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e) {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e) {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList) {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
