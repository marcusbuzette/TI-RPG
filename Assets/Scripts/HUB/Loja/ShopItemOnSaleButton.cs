using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using static UnityEditor.Progress;

public class ShopItemOnSaleButton : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI itemQuant;

    public void SetItem(Sprite image, string name, int price = -1, int quant = 0) {
        itemImage.sprite = image;
        itemName.text = name;
        if(price >= 0)itemPrice.text = price + "$";
        if(quant != 0) itemQuant.text = quant.ToString();
    }

    public void InactiveItemSale() {
        gameObject.GetComponent<Button>().interactable = false;
        itemImage.gameObject.SetActive(false);
        itemName.transform.parent.gameObject.SetActive(false);
        itemPrice?.transform.parent.gameObject.SetActive(false);
        itemQuant?.transform.parent.gameObject.SetActive(false);
    }

    public void ActiveItemSale() {
        gameObject.GetComponent<Button>().interactable = true;
        itemImage.gameObject.SetActive(true);
        itemName.transform.parent.gameObject.SetActive(true);
        itemPrice?.transform.parent.gameObject.SetActive(true);
        itemQuant?.transform.parent.gameObject.SetActive(true);
    }

    public Sprite GetImage() { return itemImage.sprite; }
    public string GetName() { return itemName.text; }
    public string GetPrice() { return itemPrice.text;}
}
