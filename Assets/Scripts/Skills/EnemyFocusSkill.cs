using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyFocusSkill : BaseSkills
{
    [SerializeField] private int skillRounds;
    public string provokeSFX;
    public override void Action() {
        unit.FocusOnMe(skillRounds);
        ActionFinish();
        ActiveCoolDown();
    }

    public override string GetActionName() {
        return "Provocar";
    }

    public override List<GridPosition> GetValidGridPositionList() {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> {
            unitGridPosition
        };
    }

    public override void TriggerAction(GridPosition mouseGridPosition, Action onActionComplete) {
        if (!string.IsNullOrEmpty(provokeSFX)) {
            AudioManager.instance?.PlaySFX(provokeSFX);  // vai tocar o sfx q ta no inspector da skill favor n mudar nada sem avisar
        }
        ActionStart(onActionComplete);

    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override void IsAnotherRound() {
        if (currentCoolDown != 0) {
            currentCoolDown--;
        }
        if (currentCoolDown == 0) {
            onCoolDown = false;
        }
    }

    public override bool GetOnCooldown() { return onCoolDown; }
}
