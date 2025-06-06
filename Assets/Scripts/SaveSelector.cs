using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSelector : MonoBehaviour
{
public GameObject pauseMenuUI;
public GameObject mainMenuUI;
public GameObject startMenuUI;

public void Open()
{
pauseMenuUI.SetActive(true);
mainMenuUI.SetActive(false);
}

public void Close()
{
mainMenuUI.SetActive(true);
pauseMenuUI.SetActive(false);
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
