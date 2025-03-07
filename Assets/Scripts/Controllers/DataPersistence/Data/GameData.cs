using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public int currentLevel;
    public int money;

    public List<SerializableInventoryItem> inventory;
    public SerializableDictionary<InventoryItemData, SerializableInventoryItem> m_inventory;

    public GameData() {
        this.currentLevel = 0;
        this.money = 0;
        this.inventory = new List<SerializableInventoryItem>();
        this.m_inventory = new SerializableDictionary<InventoryItemData, SerializableInventoryItem>();
    }
}
