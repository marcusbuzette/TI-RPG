using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using TMPro;

public class ShopManager : MonoBehaviour {

    [SerializeField] private List<InventoryItemData> ItemOnSale;
    [SerializeField] private List<ShopItemOnSaleButton> shopItemButton;

    [Space, SerializeField] private Image selectedItemImage;
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemPrice;

    [SerializeField] private TextMeshProUGUI buyItemQuantity;
    [SerializeField] private Button buyButton;
    private int itemQuantity;
    private int selectedItemindex;

    private void Start() {
        SetItemsOnSale();
    }

    public void SelectShopItem(int index) {
        selectedItemindex = index;
        buyButton.interactable = true;

        selectedItemImage.gameObject.SetActive(true);
        selectedItemName.gameObject.SetActive(true);
        selectedItemPrice.gameObject.SetActive(true);

        itemQuantity = 0;
        PlusItemQuantity();

        selectedItemImage.sprite = shopItemButton[index].GetImage();
        selectedItemName.text = shopItemButton[index].GetName();
        selectedItemPrice.text = shopItemButton[index].GetPrice() + "$";
    }

    private void DeselectItem() {
        buyButton.interactable = false;

        selectedItemImage.gameObject.SetActive(false);
        selectedItemName.gameObject.SetActive(false);
        selectedItemPrice.gameObject.SetActive(false);

        selectedItemindex = -1;
    }

    public void BuyItem() {

        if (selectedItemindex == -1) return;
        if(GameController.controller.dinheiro < (ItemOnSale[selectedItemindex].price * itemQuantity))

        for (int i = 0; i < itemQuantity; i++) {
            InventorySystem.inventorySystem.Add(ItemOnSale[selectedItemindex], true);
        }

        GameController.controller.dinheiro -= (ItemOnSale[selectedItemindex].price * itemQuantity);

        itemQuantity = 0;
        buyItemQuantity.text = itemQuantity.ToString();
        DeselectItem();
    }

    public void PlusItemQuantity() {
        if (selectedItemindex == -1) return;

        if (GameController.controller.dinheiro < ItemOnSale[selectedItemindex].price * (itemQuantity + 1)) return;

        itemQuantity++;
        selectedItemPrice.text = (ItemOnSale[selectedItemindex].price * itemQuantity).ToString();
        buyItemQuantity.text = itemQuantity.ToString();
    }
    public void MinusItemQuantity() {
        if (itemQuantity == 0 || selectedItemindex == -1) return;
        itemQuantity--;
        selectedItemPrice.text = (ItemOnSale[selectedItemindex].price * itemQuantity).ToString();
        if (itemQuantity == 0) {
            DeselectItem();
        }
        buyItemQuantity.text = itemQuantity.ToString();
    }

    private void SetItemsOnSale() {
        InactiveAllSaleButtons();
        DeselectItem();

        int buttonIndex = 0;
        foreach (InventoryItemData item in ItemOnSale) {
            shopItemButton[buttonIndex].ActiveItemSale();
            shopItemButton[buttonIndex].SetItem(item.image, item.displayName, item.price);
            buttonIndex++;
        }
    }

    private void InactiveAllSaleButtons() {
        foreach (var item in shopItemButton) {
            item.InactiveItemSale();
        }
    }
}
