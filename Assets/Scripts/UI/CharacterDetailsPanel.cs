using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDetailsPanel : MonoBehaviour {

    [SerializeField] private GameObject charsPanel;

    [Header("Seleção de Personagens")]
    [SerializeField] private Transform characterSelectionContainer;
    [SerializeField] private GameObject characterButtonPrefab;

    [Header("Informações Básicas")]
    [SerializeField] private TMP_Text unitNameText;
    [SerializeField] private Image unitImage; // opcional

    [Header("Status da Unidade")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text speedText;

    [Header("Habilidades Básicas (BaseActions)")]
    [SerializeField] private Transform actionsContainer;
    [SerializeField] private GameObject skillEntryPrefab;

    [Header("Habilidades Especiais (BaseSkills)")]
    [SerializeField] private Transform skillsContainer;

    [Header("Botão de Fechar")]
    [SerializeField] private Button closeButton;


    private UnitsCache unitsCache;
    private string selectedUnit;
    private Button selectedButton;

    private void Awake()
    {
        // Garante que o botão fecha a janela
        if (closeButton != null){
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
        unitsCache = new UnitsCache();
    }

    void Start() {
        unitsCache.Initialize();
    }

    void OnEnable() {
        StartCoroutine(AnimateCharsPanel());

        foreach (string unitId in GameController.controller.playerUnitsIds()) {
            Button unitButton = Instantiate(characterButtonPrefab, characterSelectionContainer).GetComponent<Button>();
            SkillTreeUnitButtonUI skillUI = unitButton.gameObject.GetComponent<SkillTreeUnitButtonUI>();
            Unit unitTest = unitsCache.GetUnitPrefab(unitId).GetComponent<Unit>();
            Debug.Log("Unittest - " + unitId);
            if (unitTest == null) continue;
            unitButton.gameObject.GetComponent<SkillTreeUnitButtonUI>().SetUnitData(unitId, unitTest.GetUnitName());
            unitButton.onClick.AddListener(() => {
                OnSelectedUnitChanged(unitId);
                SetSelectedButton(unitButton.GetComponent<SkillTreeUnitButtonUI>());
            });
        }
    }

    public void Show(Unit unit)
    {
        gameObject.SetActive(true);

        // Dados básicos
        unitNameText.text = unit.unitName;

        // unitImage.sprite = unit.unitSprite; // Ative se tiver sprite

        // Status
        // var status = unit.GetUnitStatus();
        // hpText.text = $"HP: {status.maxHealth}";
        // attackText.text = $"Ataque: {status.attack}";
        // defenseText.text = $"Defesa: {status.defense}";
        // speedText.text = $"Velocidade: {status.speed}";

        // // Habilidades básicas
        // PreencherLista(actionsContainer, unit.GetBaseActions());

        // // Habilidades especiais
        // PreencherLista(skillsContainer, unit.GetBaseSkills());
    }

    private void PreencherLista(Transform container, List<BaseSkills> lista)
    {
        // Remove entradas anteriores
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Cria nova entrada para cada skill
        foreach (var skill in lista)
        {
            var entry = Instantiate(skillEntryPrefab, container);
            var texts = entry.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                // texts[0].text = skill.skillName;
                // texts[1].text = skill.skillDescription;
            }
        }
    }

    private IEnumerator AnimateCharsPanel() {
        CanvasGroup canvasGroup = charsPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = charsPanel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        charsPanel.transform.localScale = Vector3.zero;

        float duration = 0.6f;
        float time = 0f;

        while (time < duration) {
            time += Time.unscaledDeltaTime; // importante se Time.timeScale = 0
            float t = time / duration;

            float bounceT = EaseOutBack(t);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            charsPanel.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, bounceT);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        charsPanel.transform.localScale = Vector3.one;
    }

    private float EaseOutBack(float t) {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    public void OnSelectedUnitChanged(string unitId) {
        this.selectedUnit = unitId;
        Unit unitAux = unitsCache.GetUnitPrefab(unitId).GetComponent<Unit>();
        // this.UpdatedSkillTree(unitAux);

    }

    public void SetSelectedButton(SkillTreeUnitButtonUI newSelected) {
        if (selectedButton != null) {
            selectedButton.GetComponent<SkillTreeUnitButtonUI>().ResetSelected();
        }

        selectedButton = newSelected.GetComponent<Button>();
        newSelected.SetSelected();
    }
}

public class UnitsCache {
    public Dictionary<string, GameObject> unitPrefabs;

    public void Initialize() {
        if (unitPrefabs == null) {
            unitPrefabs = new Dictionary<string, GameObject>();
            GameObject[] loadedUnits = Resources.LoadAll<GameObject>("Units_R");
            Debug.Log(loadedUnits.Length);
            foreach (var unit in loadedUnits) {
                unitPrefabs[unit.GetComponent<Unit>().unitId] = unit;
            }
        }
    }

    public GameObject GetUnitPrefab(string unitId) {
        if (unitPrefabs == null) Initialize();

        unitPrefabs.TryGetValue(unitId, out GameObject unit);
        return unit;
    }
}
