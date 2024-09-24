using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth;
    public int potionHealAmount = 5;
    public InventoryItemData healthPotion;
    private InventorySystem inventorySystem;

    void Start()
    {
        currentHealth = maxHealth;
        inventorySystem = FindObjectOfType<InventorySystem>();

        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem not found in the scene!");
        }
    }

    void Update()
    {
    
        if (Input.GetKeyDown(KeyCode.P))
        {
            UsePotion();
        }
    }

  
    void UsePotion()
    {
        if (inventorySystem != null && inventorySystem.HasItem(healthPotion))
        {
            if (currentHealth < maxHealth)
            {
                inventorySystem.Remove(healthPotion);
                Heal(potionHealAmount);
                Debug.Log("Used health potion. Health is now: " + currentHealth);
            }
            else
            {
                Debug.LogWarning("Health is already full.");
            }
        }
        else
        {
            Debug.LogWarning("No health potions available.");
        }
    }


    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
