using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public BaseSkills skills;
    public Text nome;
    public Text descricao;
    public Text custo;
    public Sprite skillUI;
    public Button botaoDesbloquear; 
    [SerializeField] private TooltipPosition tooltipPosition = TooltipPosition.NULL;

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
            TalentManager.Instance.GetXPPoints() >= skills.custo && 
            !TalentManager.Instance.CheckSelectedSkillOnLevel(skills.custo);

        if (!botaoDesbloquear.interactable && TalentManager.Instance.AlreadySelected(skills)) {
            var colorAux = GetComponent<Button>().colors;
            colorAux.disabledColor = skills.GetActionImage() != null ? Color.white : Color.yellow;
            GetComponent<Button>().colors = colorAux;
        } else if (!botaoDesbloquear.interactable 
                && TalentManager.Instance.CheckSelectedSkillOnLevel(skills.custo)
                && !TalentManager.Instance.AlreadySelected(skills)) {
                    if (skills.GetActionBlockedImage() != null) {
                        botaoDesbloquear.GetComponent<Image>().sprite = skills.GetActionBlockedImage();
                    } else {
                        var colorAux = GetComponent<Button>().colors;
                        colorAux.disabledColor = Color.grey;
                        GetComponent<Button>().colors = colorAux;
                    }
        }
    }

    //Define e altera os nodes da arvore de talentos.  
    public void SetBaseSkill(BaseSkills skill) {
        this.skills = skill;
        nome.text = skills.nome;
        if (skill.GetActionImage() != null) {
            botaoDesbloquear.GetComponent<Image>().sprite = skill.GetActionImage();
            nome.enabled = false;
            botaoDesbloquear.transform.Rotate(new Vector3(0,0,-45));
        }
        // descricao.text = skills.descricao;
        // custo.text = skills.custo.ToString();
    }

    private void OnDestroy() {
        TalentManager.Instance.onSkillUpdate -= TalentManager_OnSkillUpdate;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Tooltip.Instance.ShowTooltip(skillTooltipText(), transform, tooltipPosition);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Tooltip.Instance.HideTooltip();
    }

    private string skillTooltipText() {
        return "<b><size=28>" + nome.text + ": </size></b> <br><br>" + this.skills.descricao;
    }

    public void SetSkillToolTipPos(TooltipPosition pos) {
        this.tooltipPosition = pos;
    }
}
