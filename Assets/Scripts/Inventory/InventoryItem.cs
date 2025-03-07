using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem {
    public InventoryItemData data { get; protected set; }
    
    [SerializeField] public int stackSize { get; protected set; } 

    public InventoryItem(InventoryItemData source) {
        data = source;
        AddToStack();
    }


    public void AddToStack() {
        stackSize++;
    }

    public void RemoveFromStack() {
        stackSize--;
    }
}
