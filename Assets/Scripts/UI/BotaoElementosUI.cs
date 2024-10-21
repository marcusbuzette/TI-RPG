using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotaoElementosUI : MonoBehaviour
{
    public GameObject UIElement;

    public void Close(){
        UIElement.SetActive(false);
    }

    public void Open(){
        UIElement.SetActive(true);
    }
}
