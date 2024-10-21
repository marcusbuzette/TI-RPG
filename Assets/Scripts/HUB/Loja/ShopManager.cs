using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public static ShopManager shop;
    public int[,] shopItems = new int[3, 3];
    public Text DinheiroTXT;

    private void Awake()
    {
        if (shop == null)
        {
            shop = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    void Start()
    {
        //IDS
        shopItems[0, 0] = 1;

        //Preco
        shopItems[1, 0] = 5;

        //QuantidadeNoInventário
        shopItems[2, 0] = 0;
    }

    void Update()
    {
        DinheiroTXT.text = "Seu Dinheiro:" + GameController.controller.dinheiro + "$";
    }

    public void Buy()
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        if (GameController.controller.dinheiro >= shopItems[1, ButtonRef.GetComponent<BuyButtons>().ItemID])
        {
            GameController.controller.dinheiro -= shopItems[1, ButtonRef.GetComponent<BuyButtons>().ItemID];
            shopItems[2, ButtonRef.GetComponent<BuyButtons>().ItemID]++;
            ButtonRef.GetComponent<BuyButtons>().QuantityTxt.text = shopItems[2, ButtonRef.GetComponent<BuyButtons>().ItemID].ToString();
        }
    }
}
