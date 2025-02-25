using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class HitAction : BaseAction
{
    private float totalSpinAmmount = 0;
    [SerializeField] private int maxShootDistance = 1;
    [SerializeField] private int hitDamage = 50;
     public Animator animator;

    private Unit targetUnit;


    public override string GetActionName()
    {
        return "Hit";
    }

     protected override void Awake() {
        animator = GetComponentInChildren<Animator>();
    }

    public override void Action()
    {
    targetUnit.Damage(hitDamage);
    AudioManager.instance?.PlaySFX("Melee");
    animator.SetTrigger("Attack");
    ActionFinish();
    }

    public override List<GridPosition> GetValidGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
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
        return damage;
    }
}
