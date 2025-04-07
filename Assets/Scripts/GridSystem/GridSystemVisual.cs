using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour {
    public static GridSystemVisual Instance { get; private set; }
    public LayerMask obstaclesLayerMask;

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
        Yellow, 
        YellowSoft,
        Green,
        GreenSoft
    }
    [SerializeField] private Transform gridSystemVisualPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;

    public void Awake() {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start() {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight(),
            LevelGrid.Instance.GetFLoorAmount()
        ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                for(int floor = 0; floor < LevelGrid.Instance.GetFLoorAmount(); floor++) {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Transform gridSystemVisualSingleArrayTransform =
                        Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                    gridSystemVisualSingleArray[x, z, floor] = gridSystemVisualSingleArrayTransform.GetComponent<GridSystemVisualSingle>();
                    gridSystemVisualSingleArrayTransform.transform.SetParent(this.transform, false);
                }
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedUnitChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    public void HideAllGridPosition() {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
                for (int floor = 0; floor < LevelGrid.Instance.GetFLoorAmount(); floor++) {
                    gridSystemVisualSingleArray[x, z, floor].Hide();
                }
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType, Vector3 unitWorldPosition, bool haveTestDistance, bool isBlockedByObstacles) {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        //LayerMask obstaclesLayerMask = LayerMask.GetMask("Obstacles");
        for (int x = -range; x <= range; x++) {
            for (int z = -range; z <= range; z++) {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range && haveTestDistance) {
                    continue;
                }

                if (isBlockedByObstacles) {
                    Vector3 targetPos = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                    Vector3 shootDir = (targetPos - unitWorldPosition).normalized;

                    float unitShoulderHeight = 1.7f;
                    if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDir,
                        Vector3.Distance(unitWorldPosition, targetPos),
                        obstaclesLayerMask)) {

                        //Blocked by an Obstacle
                        continue;
                    }
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType) {
        foreach (GridPosition gridPosition in gridPositionList) {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor].Show(GetGridVisualTypeMaterial(gridVisualType));
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
                    gridVisualType = GridVisualType.Red;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft, selectedUnit.transform.position, true, true);
                    break;
                case ItemAction itemAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case HitAction hitAction:
                    gridVisualType = GridVisualType.Red;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), hitAction.GetMaxHitDistance(), GridVisualType.RedSoft, selectedUnit.transform.position, false, true);
                    break;
                case TeleportSkill teleportSkill:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case IntimidateSkill intimidateSkill:
                    gridVisualType = GridVisualType.Blue;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), intimidateSkill.GetMaxIntimidateDistance(), GridVisualType.BlueSoft, selectedUnit.transform.position, true, false);
                    break;
                case EnemyFocusSkill enemyFocusSkill:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case FireAttack fireAttack:
                    gridVisualType = GridVisualType.White;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), fireAttack.GetMaxShootDistance(), GridVisualType.White, selectedUnit.transform.position, true, true);
                    break;
                case PoisonAttack poisonAttack:
                    gridVisualType = GridVisualType.Green;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), poisonAttack.GetMaxShootDistance(), GridVisualType.GreenSoft, selectedUnit.transform.position, true, true);
                    break;
                case FreezeAttack freezeAttack:
                    gridVisualType = GridVisualType.Blue;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), freezeAttack.GetMaxShootDistance(), GridVisualType.BlueSoft, selectedUnit.transform.position, true, true);
                    break;
                case HealAction healAction:
                    gridVisualType = GridVisualType.Green;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), healAction.GetMaxHealDistance(), GridVisualType.GreenSoft, selectedUnit.transform.position, false, true);
                    break;
                case HealArea healArea:
                    gridVisualType = GridVisualType.White;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), healArea.GetMaxShootDistance(), GridVisualType.White, selectedUnit.transform.position, true, true);
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

    public Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList) {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
