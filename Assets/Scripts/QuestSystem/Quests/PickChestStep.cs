using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickChestStep : QuestStep {
    [SerializeField] private string chestId = "bearLevel1";

    public EventHandler onFinishQuestStep;


    private void OnEnable() {
       Chest.onChestOpened += Chest_OnChestOpened;
    }

    private void OnDisable() {
        Chest.onChestOpened -= Chest_OnChestOpened;
    }

    private void Chest_OnChestOpened(string openedChestId) {
        if (openedChestId == chestId) {
            Debug.Log($"Baú {chestId} coletado! Passo da quest concluído.");
            onFinishQuestStep?.Invoke(this, EventArgs.Empty);
            this.FinishQuestStep();
        }
    }


}
