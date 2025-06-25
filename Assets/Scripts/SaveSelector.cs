using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSelector : MonoBehaviour
{
public GameObject loadMenuUI;
public GameObject mainMenuUI;
public GameObject startMenuUI;


void Start()
{

loadMenuUI = GameObject.Find("LoadMenu");
mainMenuUI = GameObject.Find("MainMenu");
startMenuUI = GameObject.Find("StartMenu");
loadMenuUI.SetActive(false);
startMenuUI.SetActive(false);

}





public void Open()
{
loadMenuUI.SetActive(true);
mainMenuUI.SetActive(false);
}

public void Close()
{
mainMenuUI.SetActive(true);
loadMenuUI.SetActive(false);
startMenuUI.SetActive(false);

}

public void Openstart()
{
startMenuUI.SetActive(true);
mainMenuUI.SetActive(false);
}



public void LoadSlot1() {
    DataPersistenseManager.instance.SetSlot("slot1");
    DataPersistenseManager.instance.LoadGame();
}

public void LoadSlot2() {
    DataPersistenseManager.instance.SetSlot("slot2");
    DataPersistenseManager.instance.LoadGame();
}


public void LoadSlot3() {
    DataPersistenseManager.instance.SetSlot("slot3");
    DataPersistenseManager.instance.LoadGame();
}


public void StartSlot1()
{
    DataPersistenseManager.instance.SetSlot("slot1");
    DataPersistenseManager.instance.NewGame();
}
public void StartSlot2()
{
    DataPersistenseManager.instance.SetSlot("slot2");
    DataPersistenseManager.instance.NewGame();
}
public void StartSlot3()
{
    DataPersistenseManager.instance.SetSlot("slot3");
    DataPersistenseManager.instance.NewGame();
}

}
