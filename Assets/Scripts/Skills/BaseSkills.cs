using System;
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
    public EventHandler onEndEffect;
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

    public int GetCoolDown() {
        return coolDown;
    }

    public void SetSkillImage(Sprite image) {
        this.actionImage = image;
    }

    public virtual BuffType? GetBuffType() {return null;}

    public void SetSkill() {
        this.actionType = ActionType.SKILL;
        unit = GetComponent<Unit>();
        animator = GetComponentInChildren<Animator>();
    }
}
