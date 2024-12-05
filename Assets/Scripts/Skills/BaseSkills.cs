using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseSkills : BaseAction
{
    public string nome;
    public string descricao;
    public int custo;
    public List<BaseSkills> preRequisitos;
    [SerializeField] protected bool onCoolDown;
    [SerializeField] protected int coolDown;
    [SerializeField] protected int currentCoolDown = 0;

    protected override void Update() {
        if (onCoolDown || !isActive) { return; }
        Action();
    }

    protected override void Awake() {
        base.Awake();
        this.actionType = ActionType.SKILL;
    }

    protected void ActiveCoolDown() {
        currentCoolDown = coolDown;
        onCoolDown = true;
    }
}
