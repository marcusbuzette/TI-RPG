using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPotionAction : BaseAction
{
    [SerializeField] public int potionHealAmount = 20;

    // MUDANÇA 1: Usaremos 'potionId' para a verificação. 
    // O [SerializeField] permite que você defina o ID no Inspector do Unity.
    [SerializeField] private string potionId = "potion_health_01"; // <-- Coloque aqui o ID exato da sua poção!
    
    private HealthSystem healthSystem;
    public int Attack = 1;

    public override void Action()
    {
        // ... (as verificações de sistema de inventário e de vida continuam as mesmas)
        if (InventorySystem.inventorySystem == null) {
            Debug.LogError("Referência ao InventorySystem é NULA.");
            DelayActionFinish();
            return;
        }
        if (healthSystem == null) {
            Debug.LogError("HealthSystem do alvo é NULO.");
            DelayActionFinish();
            return;
        }

        // MUDANÇA 2: Trocamos HasItemNamed por HasItemId e GetInvontoryItemNamed por GetItemById.
        if (InventorySystem.inventorySystem.HasItemId(potionId))
        {
            Debug.Log($"Item com ID '{potionId}' encontrado! Usando a poção.");

            InventoryItemData healthPotion = InventorySystem.inventorySystem.GetItemById(potionId);
            InventorySystem.inventorySystem.Remove(healthPotion);
            healthSystem.Heal(potionHealAmount);

            Debug.Log($"Unidade curada em {potionHealAmount} pontos de vida.");
        }
        else
        {
            Debug.LogWarning($"Ação falhou. O jogador não possui o item com o ID: '{potionId}'");
        }
        
        DelayActionFinish();
    }

    public void DelayActionFinish()
    {
        ActionFinish();
    }

    public override string GetActionName()
    {
        return "Po��o";
    }

    public override List<GridPosition> GetValidGridPositionList()
    {

        if (!LevelGrid.Instance.IsInBattleMode())
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGrid = UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition();

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                    GridPosition testGridPosition = unitGrid + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }


                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).GetHealthSystem().GetHealthState() == HealthSystem.HealthState.FAINT)
                    {
                        continue;
                    }

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    if (targetUnit.IsEnemy())
                    {
                        continue;
                    }

                    if (targetUnit.GetHealthSystem().maxHealthPoints == targetUnit.GetHealthPoints())
                    {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }

            return validGridPositionList;
        }

        unit = UnitActionSystem.Instance.GetSelectedUnit();
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete)
    {
        this.actionType = ActionType.ITEM;
        this.unit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
        this.healthSystem = unit.GetComponent<HealthSystem>();
        unit.PlayAnimation("Item");
        ActionStart(onActionComplete);
        AudioManager.instance?.PlaySFX("Potion");
        Action();
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

}