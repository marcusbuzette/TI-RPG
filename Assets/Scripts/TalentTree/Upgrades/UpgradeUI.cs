using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public PossibleUpgrade upgrade;
    public int upgradeIndex;
    public Text nome;
    public Text descricao;
    public Text custo;
    public Button botaoDesbloquear;

    private void Start() {
        TalentManager.Instance.onSkillUpdate += TalentManager_OnSkillUpdate;
        UpdateUpgradeButtonState();
    }

    public void TalentManager_OnSkillUpdate(object sender, EventArgs e) {
        UpdateUpgradeButtonState();
    }

    private void UpdateUpgradeButtonState() {
        botaoDesbloquear.interactable = TalentManager.Instance.CanUpgrade(upgrade);

        if (!botaoDesbloquear.interactable && TalentManager.Instance.AlreadyUpgraded(upgrade, upgradeIndex)) {
            var colorAux = GetComponent<Button>().colors;
            colorAux.disabledColor = Color.yellow;
            GetComponent<Button>().colors = colorAux;
        }
    }

    //Define e altera os nodes da arvore de talentos.  
    public void SetBaseUpgrade(PossibleUpgrade upgrade, int index) {
        this.upgrade = upgrade;
        this.upgradeIndex = index;
        nome.text = upgrade.upgrade[index].name;
        if (upgrade.upgrade[index].upgradeImage != null) {
            botaoDesbloquear.GetComponent<Image>().sprite = upgrade.upgrade[index].upgradeImage;
            nome.enabled = false;
            botaoDesbloquear.transform.Rotate(new Vector3(0,0,-45));
        }
    }

    private void OnDestroy() {
        TalentManager.Instance.onSkillUpdate -= TalentManager_OnSkillUpdate;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Tooltip.Instance.ShowTooltip(nome.text);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Tooltip.Instance.HideTooltip();
    }
}
