using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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
    [SerializeField] private TMP_Text moveText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text speedText;

    [Header("Habilidades Básicas (BaseActions)")]
    [SerializeField] private Transform actionsContainer;
    [SerializeField] private GameObject skillEntryPrefab;

    [Header("Botão de Fechar")]
    [SerializeField] private Button closeButton;


    private UnitsCache unitsCache;
    private SkillCache skillCache;
    private string selectedUnit;
    private Button selectedButton;

    private CloseUpCharCam closeUpCharCam;

    private void Awake() {
        // Garante que o botão fecha a janela
        if (closeButton != null) {
            closeButton.onClick.AddListener(() => this.CloseCharPanel());
        }
        unitsCache = new UnitsCache();
        skillCache = new SkillCache();
    }

    void Start() {
        unitsCache.Initialize();
        skillCache.Initialize();
    }

    void OnEnable() {
        StartCoroutine(AnimateCharsPanel());

        foreach (string unitId in GameController.controller.playerUnitsIds()) {
            Button unitButton = Instantiate(characterButtonPrefab, characterSelectionContainer).GetComponent<Button>();
            SkillTreeUnitButtonUI skillUI = unitButton.gameObject.GetComponent<SkillTreeUnitButtonUI>();
            Unit unitTest = unitsCache.GetUnitPrefab(unitId).GetComponent<Unit>();
            if (unitTest == null) continue;
            unitButton.gameObject.GetComponent<SkillTreeUnitButtonUI>().SetUnitData(unitId, unitTest.GetUnitName());
            unitButton.onClick.AddListener(() => {
                OnSelectedUnitChanged(unitId);
                SetSelectedButton(unitButton.GetComponent<SkillTreeUnitButtonUI>());
            });

            if (selectedButton == null) {
                SetSelectedButton(unitButton.GetComponent<SkillTreeUnitButtonUI>());
                this.selectedUnit = unitId;
            }
        }

        if (closeUpCharCam == null) {
            GameObject closecamPrefab = Resources.Load<GameObject>("RenderImage_R/CharCloseup");
            if (closecamPrefab == null) return;
            closeUpCharCam = Instantiate(closecamPrefab).GetComponent<CloseUpCharCam>();
            closeUpCharCam.ShowUnit(selectedUnit);
        }
        else {
            closeUpCharCam.gameObject.SetActive(true);
        }

        Show();
    }

    private void CloseCharPanel() {
        closeUpCharCam.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);

        UnitRecords unit = GameController.controller.GetUnitRecords(this.selectedUnit);

        // Dados básicos
        // unitNameText.text = unit.unitName;

        // unitImage.sprite = unit.unitSprite; // Ative se tiver sprite

        // Status
        Unit unitAux = unitsCache.GetUnitPrefab(this.selectedUnit).GetComponent<Unit>();
        UnitStats status = unitAux.GetUnitStats();
        hpText.text = status.GetMaxHP().ToString();
        attackText.text = status.GetAttack().ToString();
        defenseText.text = status.GetDefence().ToString();
        accuracyText.text = status.GetAccuracy().ToString();
        speedText.text = status.GetSpeed().ToString();
        moveText.text = status.GetMaxMoveStats().ToString();

        List<BaseAction> baseActionList = new List<BaseAction>();

        List<BaseAction> baseActionAux = unitAux.GetComponents<BaseAction>().ToList();

        int bolsaIndex = baseActionAux.FindIndex((ba) => ba.GetType().Name == "InventoryAction");
        if(bolsaIndex >= 0) {
            baseActionAux.RemoveAt(bolsaIndex);
        }

        baseActionList.AddRange(baseActionAux);
        foreach (string skillId in GameController.controller.GetUnitRecords(this.selectedUnit).GetUnitSKillsIDs()) {
            bool hasSkillOnList = baseActionList.Find((ba) => {
                return ba.GetType().Name == skillId;
            });
            if (!hasSkillOnList) {
                baseActionList.Add(skillCache.GetSkillPrefab(skillId));
            }
        }

        // // Habilidades básicas
        PreencherLista(actionsContainer, baseActionList);
    }

    private void PreencherLista(Transform container, List<BaseAction> lista) {
        // Remove entradas anteriores
        foreach (Transform child in container) {
            Destroy(child.gameObject);
        }

        // Cria nova entrada para cada skill
        foreach (BaseAction action in lista) {
            ActionDetailsUI actionDetails = Instantiate(skillEntryPrefab, container).GetComponent<ActionDetailsUI>();
            actionDetails.SetActionDetails(action);
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
        closeUpCharCam.ShowUnit(unitId);
        Show();

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
