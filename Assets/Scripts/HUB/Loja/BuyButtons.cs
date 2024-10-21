using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButtons : MonoBehaviour
{
    public int ItemID;
    public Text PriceTxt;
    public Text QuantityTxt;

    void Update(){
        PriceTxt.text = ShopManager.shop.shopItems[1, ItemID].ToString() + "$";
        QuantityTxt.text = ShopManager.shop.shopItems[2, ItemID].ToString();
    }
}
