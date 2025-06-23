using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseSkills : BaseAction {
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

    public virtual BuffType? GetBuffType() { return null; }

    public void SetSkill() {
        this.actionType = ActionType.SKILL;
        unit = GetComponent<Unit>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void CopyFrom(BaseSkills other) {
        this.nome = other.nome;
        this.descricao = other.descricao;
        this.custo = other.custo;

        // Cria nova lista para evitar referência compartilhada
        this.preRequisitos = other.preRequisitos != null
            ? new List<BaseSkills>(other.preRequisitos)
            : new List<BaseSkills>();

        this.onCoolDown = other.onCoolDown;
        this.coolDown = other.coolDown;
        this.currentCoolDown = other.currentCoolDown;

        // Eventos não são normalmente copiados diretamente, pois eles mantêm referências de objetos.
        // Se necessário, pode ser feito manualmente, mas normalmente omitido:
        // this.onEndEffect = other.onEndEffect;
    }


}
