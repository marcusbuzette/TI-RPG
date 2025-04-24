using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour, IInteractiveObjects
{
    private bool goingTo = false;
    private GridPosition targetToUnit;
    private Unit currentUnit;

    InventorySystem inventory;

    [Space, Header("Chest Items"),SerializeField] private List<InventoryItemData> chestItems;

    [Space, Header("Chest Canvas"),SerializeField]
    private GameObject chestCanvas;

    private InventoryItemData _item;
    public float imageTimer;
    public AnimatorController animController;

    private void Start() {
        inventory = InventorySystem.inventorySystem;

        GetFowardGridObject();
    }

    private void Update() {
        if(goingTo) {
            if(currentUnit.GetGridPosition() == targetToUnit) {
                LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
                PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

                RecolherItens();

                goingTo = false;
            }
        }
    }

    public void GetFowardGridObject() {
        Vector3 pos = (transform.forward * 2) + transform.position;
        targetToUnit = LevelGrid.Instance.GetGridPosition(pos);
        Debug.Log(targetToUnit);
    }

    public void MoveUnitToGridPostion(Unit unit) {
        goingTo = true;
        currentUnit = unit;


        UnitActionSystem.Instance.MoveUnitToGridPosition(currentUnit, targetToUnit);
        LevelGrid.Instance.OnGameModeChanged += UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath += UnitStopGoingTo;
    }

    public void UnitStopGoingTo(object sender, EventArgs e) {
        LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

        Debug.Log("PAROU DE IR");

        goingTo = false;
        currentUnit = null;
    }

    public void UnitStopGoingTo() {
        LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

        goingTo = false;
        currentUnit = null;
    }

    private void RecolherItens() {
        if (chestItems.Count == 0) return;

        _item = chestItems[0];
        inventory.Add(_item);
        chestItems.Remove(_item);

        StartCoroutine(CreateImage());
    }

    private IEnumerator CreateImage() {
        var _obj = Instantiate(new GameObject("ImageItemChest"), chestCanvas.transform);
        var image = _obj.AddComponent<Image>();

        if(_item.image != null)
            image.sprite = _item.image;

        var anim = _obj.AddComponent<Animator>();
        anim.runtimeAnimatorController = animController;

        yield return new WaitForSeconds(imageTimer);
        Destroy(_obj);
        
        RecolherItens();
    }
}
