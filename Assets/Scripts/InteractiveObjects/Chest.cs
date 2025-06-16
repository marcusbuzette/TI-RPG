using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour, IInteractiveObjects
{
    private bool goingTo = false;
    private GridPosition targetToUnit;
    private Unit currentUnit;

    InventorySystem inventory;

    [Space, Header("Chest Items"),SerializeField] private List<InventoryItemData> chestItems;
    [Space, Header("Chest Gold"), SerializeField] private int gold;

    [Space, Header("Chest Canvas"),SerializeField]
    private GameObject chestCanvas;

    private InventoryItemData _item;
    public float imageTimer;
    public RuntimeAnimatorController animController;

    bool used = false;

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

                used = true;
                goingTo = false;
            }
        }
    }

    public void GetFowardGridObject() {
        Vector3 pos = (transform.forward * 2) + transform.position;
        targetToUnit = LevelGrid.Instance.GetGridPosition(pos);
    }

    public void MoveUnitToGridPostion(Unit unit) {
        if (used) return;

        goingTo = true;
        currentUnit = unit;


        UnitActionSystem.Instance.MoveUnitToGridPosition(currentUnit, targetToUnit);
        LevelGrid.Instance.OnGameModeChanged += UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath += UnitStopGoingTo;
    }

    public void UnitStopGoingTo(object sender, EventArgs e) {
        LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

        goingTo = false;
        currentUnit = null;
    }

    public void UnitStopGoingTo() {
        LevelGrid.Instance.OnGameModeChanged -= UnitStopGoingTo;
        PathFinding.Instance.OnRecalculatedpath -= UnitStopGoingTo;

        goingTo = false;
        currentUnit = null;
    }

    public void AddItens(List<InventoryItemData> items, int gold = 0) {
        chestItems = items;
        this.gold = gold;
    }

    private void RecolherItens() {
        if (chestItems.Count == 0) {
            if(gold != 0) {
                GameController.controller.AddMoney(gold);
                gold = 0;
                StartCoroutine(CreateGoldImage());
            }
            return;
        }

        _item = chestItems[0];
        inventory.Add(_item);
        chestItems.Remove(_item);

        StartCoroutine(CreateItemImage());
    }

    private IEnumerator CreateItemImage() {
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

    private IEnumerator CreateGoldImage() {
        var _obj = Instantiate(new GameObject("ImageItemChest"), chestCanvas.transform);
        var _text = _obj.AddComponent<TextMeshPro>();

        _text.text = ("+" + gold + " GOLD");
        _text.color = Color.yellow;
        _text.alignment = TextAlignmentOptions.Center;
        _text.fontSize = 5;

        var anim = _obj.AddComponent<Animator>();
        anim.runtimeAnimatorController = animController;

        yield return new WaitForSeconds(imageTimer);
        Destroy(_obj);
    }
}
