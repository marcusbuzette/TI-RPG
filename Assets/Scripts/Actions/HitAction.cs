using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class HitAction : BaseAction
{
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private int hitDamage = 50;
    [SerializeField] private float rotateSpeed = 10f;
    public int Attack = 1;

    private Unit targetUnit;


    public override string GetActionName()
    {
        return "Ataque";
    }

    public override void Action()
{
        if(Attack == 1){
        targetUnit?.Damage(hitDamage);
        animator?.SetTrigger("Attack");
        AudioManager.instance?.PlaySFX("Melee");
        Attack=0;
    }
   StartCoroutine(DelayActionFinish());

}

private IEnumerator DelayActionFinish()
{
    yield return new WaitForSeconds(0.5f); // Ajuste o tempo conforme necessário
    ActionFinish();
    Attack=1;

}

    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }


                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public override bool GetOnCooldown() { return false; }

    public override void IsAnotherRound() { }

    public int GetDamage()
    {
        int damage = hitDamage;
        AudioManager.instance?.PlaySFX("DamageTaken");
        return damage;

    }
}
