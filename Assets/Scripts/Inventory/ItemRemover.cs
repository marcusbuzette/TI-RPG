using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRemover : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (inventorySystem != null)
            {
        
                if (inventorySystem.HasItem(referenceItem))
                {
                    inventorySystem.Remove(referenceItem);
                    Debug.Log(referenceItem + " has been removed from the inventory.");
                }
                else
                {
                    Debug.LogWarning(referenceItem + " does not exist in the inventory.");
                }
            }
        }
    }
}