using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopItemOnSaleButton : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;

    public void SetItem(Sprite image, string name, int price) {
        itemImage.sprite = image;
        itemName.text = name;
        itemPrice.text = price + "$";
    }

    public void InactiveItemSale() {
        gameObject.GetComponent<Button>().interactable = false;
        itemImage.gameObject.SetActive(false);
        itemName.transform.parent.gameObject.SetActive(false);
        itemPrice.transform.parent.gameObject.SetActive(false);
    }

    public void ActiveItemSale() {
        gameObject.GetComponent<Button>().interactable = true;
        itemImage.gameObject.gameObject.SetActive(true);
        itemName.transform.parent.gameObject.gameObject.SetActive(true);
        itemPrice.transform.parent.gameObject.gameObject.SetActive(true);
    }

    public Sprite GetImage() { return itemImage.sprite; }
    public string GetName() { return itemName.text; }
    public string GetPrice() { return itemPrice.text;}
}
