using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class InventorySystem : MonoBehaviour, IDataPersistence {
    private SerializableDictionary<InventoryItemData, SerializableInventoryItem> m_itemDictionary;
    [SerializeField] public List<SerializableInventoryItem> inventory;
    public static InventorySystem inventorySystem;


    private void Awake() {
        if (inventorySystem == null) {
            inventorySystem = this;
            DontDestroyOnLoad(this);
        }
        else {
            DestroyImmediate(gameObject);
        }
        inventory = new List<SerializableInventoryItem>();
        m_itemDictionary = new SerializableDictionary<InventoryItemData, SerializableInventoryItem>();
    }

    public void Add(InventoryItemData referenceData) {
        if (m_itemDictionary.TryGetValue(referenceData, out SerializableInventoryItem value)) {
            value.AddToStack();
        }
        else {
            SerializableInventoryItem newItem = new SerializableInventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }
    }

    public bool IsEmpty() {
        return inventory.Count < 1;
    }

    public void Remove(InventoryItemData referenceData) {
        if (m_itemDictionary.TryGetValue(referenceData, out SerializableInventoryItem value)) {
            value.RemoveFromStack();

            if (value.stackSize == 0) {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }
    }

    public bool HasItem(InventoryItemData referenceData) {
        return m_itemDictionary.ContainsKey(referenceData) && m_itemDictionary[referenceData].stackSize > 0;
    }

    public bool HasItemNamed(string name) {
        foreach (InventoryItemData key in m_itemDictionary.Keys) {
            if (key.displayName == name) return true;
        }
        return false;
    }

    public InventoryItemData GetInvontoryItemNamed(string name) {
        foreach (InventoryItemData key in m_itemDictionary.Keys) {
            if (key.displayName == name) return key;
        }
        return null;
    }

    public Dictionary<InventoryItemData, SerializableInventoryItem> GetInventoryContent() { return m_itemDictionary; }

    public void LoadData(GameData data) {
        this.inventory = data.inventory;
        this.m_itemDictionary = data.m_inventory;
    }

    public void SaveData(ref GameData data) {
        data.inventory = this.inventory;
        data.m_inventory = this.m_itemDictionary;
    }
}
