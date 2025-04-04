using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SniperActionFieldOfView : MonoBehaviour
{
    private SniperAction sniperAction;

    private List<GridPosition> gridObjects;
    private Material viewMaterial;
    private int damage;
    private GameObject grid;

    private void Start() {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += CheckHasEnemyOnFieldOfView;
    }

    public void SetSniperFieldOfView(SniperAction sniperAction, List<GridPosition> gridObjects, int damage) {
        this.sniperAction = sniperAction;
        this.gridObjects = gridObjects;
        this.damage = damage;

        viewMaterial = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridSystemVisual.GridVisualType.Red);

        DrawFieldOFView();
    }

    private void CheckHasEnemyOnFieldOfView(object sender, EventArgs e) {
        foreach (GridPosition gridPos in gridObjects) {
            if(LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPos)) {
                if (LevelGrid.Instance.GetUnitAtGridPosition(gridPos).IsEnemy())
                {
                    LevelGrid.Instance.GetUnitAtGridPosition(gridPos).Damage(damage);
                    StopSniper();
                }

            }
        }
    }
    private void DrawFieldOFView() {

        foreach(GridPosition gridPos in gridObjects) {
            grid = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Quad), LevelGrid.Instance.GetWorldPosition(gridPos), Quaternion.Euler(new Vector3(90, 0,0)), transform);
            grid.transform.position += new Vector3(0,0.03f,0);
            grid.transform.localScale *= 2;
            grid.GetComponent<MeshRenderer>().material = viewMaterial;
        }
    }

    public void StopSniper() {
        sniperAction.SetFiewdOfViewNull();
        Destroy(gameObject);
    }
}
