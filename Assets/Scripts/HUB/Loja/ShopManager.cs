using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour {
    public static ShopManager shop;
    public int[,] shopItems = new int[3, 3];
    public Text DinheiroTXT;
    public Button potionButton; //TODO: Fazer preenchimento dos itens de forma procedural
    public InventoryItemData[] itemData;

    private void Awake() {
        if (shop == null) {
            shop = this;
        }
        else {
            DestroyImmediate(gameObject);
        }
    }

    void Start() {
        //IDS
        shopItems[0, 0] = 1;

        //Preco
        shopItems[1, 0] = 5;

        //QuantidadeNoInventário
        shopItems[2, 0] = 0;

    }

    private void OnEnable() {
        this.UpdateMoneyText();
        foreach (KeyValuePair<InventoryItemData, SerializableInventoryItem> item in InventorySystem.inventorySystem.GetInventoryContent()) {
            this.UpdateItemQuantityOnInventory(0, item.Value.GetItemAmount());
            // this.UpdateItemQuantityOnInventory(item.Key.id);
        }

    }

    public void Buy() {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;
        int itemId = ButtonRef.GetComponent<BuyButtons>().ItemID;

        if (GameController.controller.dinheiro >= shopItems[1, ButtonRef.GetComponent<BuyButtons>().ItemID]) {
            GameController.controller.dinheiro -= shopItems[1, ButtonRef.GetComponent<BuyButtons>().ItemID];
            InventorySystem.inventorySystem.Add(itemData[itemId]);
            this.UpdateMoneyText();
            this.UpdateItemQuantityOnInventory(itemId, InventorySystem.inventorySystem.GetInventoryContent()[itemData[0]].GetItemAmount());
        }
    }

    private void UpdateMoneyText() {
        DinheiroTXT.text = "Seu Dinheiro:" + GameController.controller.dinheiro + "$";
    }

    private void UpdateItemQuantityOnInventory(int itemId, int amount) {
        this.shopItems[2, itemId] = amount;
        potionButton.GetComponent<BuyButtons>().QuantityTxt.text = amount.ToString();
    }
}
