using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Inventory Item Data")]

[System.Serializable]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string displayName;
    public GameObject prefab;
    public Sprite image;
}
