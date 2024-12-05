using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUi : MonoBehaviour
{
    public BaseSkills skills;
    public Text nome;
    public Text descricao;
    public Text custo;
    public Button botaoDesbloquear;

    private void Start() {
        TalentManager.Instance.onSkillUpdate += TalentManager_OnSkillUpdate;
        UpdateSkillButtonState();
    }

    public void TalentManager_OnSkillUpdate(object sender, EventArgs e) {
        UpdateSkillButtonState();
    }

    private void UpdateSkillButtonState() {
        botaoDesbloquear.interactable =
            TalentManager.Instance.PodeSerDesbloqueado(skills) &&
            TalentManager.Instance.GetXPPoints() >= skills.custo;
    }

    //Define e altera os nodes da arvore de talentos.  
    public void SetBaseSkill(BaseSkills skill) {
        this.skills = skill;
        nome.text = skills.nome;
        descricao.text = skills.descricao;
        custo.text = skills.custo.ToString();
    }

    private void OnDestroy() {
        TalentManager.Instance.onSkillUpdate -= TalentManager_OnSkillUpdate;
    }
}
