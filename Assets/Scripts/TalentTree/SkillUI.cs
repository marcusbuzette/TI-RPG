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
    private GameObject treeBranch;
    [SerializeField] private GameObject selectedBG;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unavailableColor;
    [SerializeField] private Color availableColor;

    private void Start() {
        TalentManager.Instance.onSkillUpdate += TalentManager_OnSkillUpdate;
        UpdateSkillButtonState();
    }

    public void TalentManager_OnSkillUpdate(object sender, EventArgs e) {
        UpdateSkillButtonState();
    }

    private void UpdateSkillButtonState() {
        bool isSelected = TalentManager.Instance.AlreadySelected(skills);
        bool hasPoints = TalentManager.Instance.GetXPPoints() >= skills.custo;
        bool canBeUnlocked = TalentManager.Instance.PodeSerDesbloqueado(skills);
        bool alreadyChoseOnThisLevel = TalentManager.Instance.CheckSelectedSkillOnLevel(skills.custo);

        botaoDesbloquear.interactable = canBeUnlocked && hasPoints && !alreadyChoseOnThisLevel;

        // Aplica cor ao botão e branch dependendo do estado
        if (isSelected) {
            treeBranch.SetActive(true);
            SetBranchColor(selectedColor);
            selectedBG?.SetActive(true);
        } else if (alreadyChoseOnThisLevel) {
            // Outra skill foi escolhida no mesmo nível, então oculta o branch
            treeBranch.SetActive(false);
        } else if (!hasPoints || !canBeUnlocked) {
            treeBranch.SetActive(true);
            SetBranchColor(unavailableColor);
        } else {
            treeBranch.SetActive(true);
            SetBranchColor(availableColor);
        }

        // Ajustes de imagem/cores no botão
        if (!botaoDesbloquear.interactable && isSelected) {
            var colorAux = GetComponent<Button>().colors;
            colorAux.disabledColor = skills.GetActionImage() != null ? Color.white : Color.yellow;
            GetComponent<Button>().colors = colorAux;
        }
        else if (!botaoDesbloquear.interactable && alreadyChoseOnThisLevel && !isSelected) {
            if (skills.GetActionBlockedImage() != null) {
                botaoDesbloquear.GetComponent<Image>().sprite = skills.GetActionBlockedImage();
            }
            else {
                var colorAux = GetComponent<Button>().colors;
                colorAux.disabledColor = Color.grey;
                GetComponent<Button>().colors = colorAux;
            }
        }
    }

    //Define e altera os nodes da arvore de talentos.  
    public void SetBaseSkill(BaseSkills skill, int index) {
        this.skills = skill;
        nome.text = skills.nome;
        if (index % 2 == 0) { //se for par
            treeBranch = transform.Find("BranchL").gameObject;
        }
        else { // se for impar
            treeBranch = transform.Find("BranchR").gameObject;
        }
        if (skill.GetActionImage() != null) {
            botaoDesbloquear.GetComponent<Image>().sprite = skill.GetActionImage();
            nome.enabled = false;
            botaoDesbloquear.transform.Rotate(new Vector3(0, 0, -45));
        }
        treeBranch.SetActive(true);
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

    private void SetBranchColor(Color color) {
        if (treeBranch != null) {
            Image branchImage = treeBranch.GetComponent<Image>();
            if (branchImage != null) {
                branchImage.color = color;
            }
        }
    }

}
