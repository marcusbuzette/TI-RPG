using System;
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
    [SerializeField] private TooltipPosition tooltipPosition = TooltipPosition.NULL;
    [SerializeField] private GameObject selectedBG;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unavailableColor;
    [SerializeField] private Color availableColor;

    private GameObject treeBranch;

    private void Start() {
        TalentManager.Instance.onSkillUpdate += TalentManager_OnSkillUpdate;
        UpdateUpgradeButtonState();
    }

    public void TalentManager_OnSkillUpdate(object sender, EventArgs e) {
        UpdateUpgradeButtonState();
    }

    private void UpdateUpgradeButtonState() {
        bool isSelected = TalentManager.Instance.AlreadyUpgraded(upgrade, upgradeIndex);
        bool canBeUnlocked = TalentManager.Instance.CanUpgrade(upgrade);
        bool alreadyChoseOnThisLevel = TalentManager.Instance.AlreadyChoseUpgradeFromLevel(upgrade);

        botaoDesbloquear.interactable = canBeUnlocked && !alreadyChoseOnThisLevel;

        // Aplica cor e visibilidade no branch
        if (isSelected) {
            treeBranch.SetActive(true);
            SetBranchColor(selectedColor);
            selectedBG?.SetActive(true);
        } else if (alreadyChoseOnThisLevel && !isSelected) {
            treeBranch.SetActive(false);
        } else if (!canBeUnlocked) {
            treeBranch.SetActive(true);
            SetBranchColor(unavailableColor);
        } else {
            treeBranch.SetActive(true);
            SetBranchColor(availableColor);
        }

        // Ajuste de cores e imagens
        if (!botaoDesbloquear.interactable && isSelected) {
            var colorAux = GetComponent<Button>().colors;
            colorAux.disabledColor = upgrade.upgrade[upgradeIndex].upgradeImage != null ? Color.white : Color.yellow;
            GetComponent<Button>().colors = colorAux;
        }
        else if (!botaoDesbloquear.interactable &&
                 alreadyChoseOnThisLevel &&
                 !isSelected) {

            if (upgrade.upgrade[upgradeIndex].upgradeBlockedImage != null) {
                botaoDesbloquear.GetComponent<Image>().sprite = upgrade.upgrade[upgradeIndex].upgradeBlockedImage;
            } else {
                var colorAux = botaoDesbloquear.colors;
                colorAux.disabledColor = Color.gray;
                botaoDesbloquear.colors = colorAux;
            }
        }
    }

    public void SetBaseUpgrade(PossibleUpgrade upgrade, int index) {
        this.upgrade = upgrade;
        this.upgradeIndex = index;
        nome.text = upgrade.upgrade[index].name;
        this.tooltipPosition = index % 2 == 0 ? TooltipPosition.LEFT : TooltipPosition.RIGHT;

        // Define imagem e rotação (caso sprite esteja presente)
        if (upgrade.upgrade[index].upgradeImage != null) {
            botaoDesbloquear.GetComponent<Image>().sprite = upgrade.upgrade[index].upgradeImage;
            nome.enabled = false;
            botaoDesbloquear.transform.Rotate(new Vector3(0, 0, -45));
        }

        // Localiza o branch correto
        treeBranch = transform.Find(index % 2 == 0 ? "BranchL" : "BranchR")?.gameObject;
        if (treeBranch != null) {
            treeBranch.SetActive(true);
        }
    }

    private void OnDestroy() {
        TalentManager.Instance.onSkillUpdate -= TalentManager_OnSkillUpdate;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Tooltip.Instance.ShowTooltip(nome.text, transform, tooltipPosition);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Tooltip.Instance.HideTooltip();
    }

    private void SetBranchColor(Color color) {
        if (treeBranch != null) {
            Image img = treeBranch.GetComponent<Image>();
            if (img != null) {
                img.color = color;
            }
        }
    }
}
