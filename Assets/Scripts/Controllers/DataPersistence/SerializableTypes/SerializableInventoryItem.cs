using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableInventoryItem : InventoryItem, ISerializationCallbackReceiver {

    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int qtd;
    [SerializeField] private InventoryItemData scriptableObj;



    public SerializableInventoryItem(InventoryItemData source) : base(source) {}

    public void OnAfterDeserialize() {
        // this.data.id = this.id;
        // this.data.displayName = this.displayName;
        // this.data.prefab = this.prefab;
        this.data = this.scriptableObj;
        this.stackSize = this.qtd;

    }

    public void OnBeforeSerialize() {
        // this.id = null;
        // this.displayName = null;
        // this.prefab = null;
        // this.qtd = 0;
        this.scriptableObj = null;

        this.scriptableObj = this.data;
        this.qtd = this.stackSize;
    }

    public int GetItemAmount() {
        return this.qtd;
    }
}
