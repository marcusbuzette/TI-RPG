using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject {

    [field: SerializeField] public string id { get; private set; }

    [Header("General1")]
    public string displayName;


    [Header("Requirements")]
    public int levelRequirement;
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;


    [Header("Rewards")]
    public int moneyRewards;
    public int playerXp;

    void OnValidate() {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
