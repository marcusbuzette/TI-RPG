using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;
    private InventorySystem inventorySystem;

    void Start()
    {

        inventorySystem = FindObjectOfType<InventorySystem>();


        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem not found in the scene!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inventorySystem != null)
            {
                inventorySystem.Add(referenceItem);
                Debug.Log(referenceItem + " has been added to the inventory.");
            }
        }
    }
}