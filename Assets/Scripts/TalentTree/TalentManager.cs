using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TalentManager : MonoBehaviour {
    [SerializeField] private Transform charactersContainer;
    [SerializeField] private Transform skillTreeContainer;
    [SerializeField] private Button unitButtonPrefab;

    [SerializeField] private List<Unit> playerUnitList = new List<Unit>();

    private List<BaseSkills> skills;
    public int pontosDisponiveis = 4;
    public Text pontos;

    private String SelectedUnit;

    public static TalentManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    public void Start() {
        foreach(string unitId in GameController.controller.playerUnitsIds()) {
            Button unitButton = Instantiate(unitButtonPrefab, charactersContainer);
            unitButton.gameObject.AddComponent<SkillTreeUnitButtonUI>();
            unitButton.gameObject.GetComponent<SkillTreeUnitButtonUI>().SetUnitData(unitId, unitId);
            unitButton.onClick.AddListener(() => OnSelectedUnitChanged(unitId));
        }
        pontos.text = "Pontos: " + pontosDisponiveis;

    }

    //Verifica se existem pontos suficientes e todas as condições estão cumpridas para o desbloqueio do skills
    public void TentarDesbloquearskills(BaseSkills skills) {
        if (pontosDisponiveis >= skills.custo && PodeSerDesbloqueado(skills)) {
            pontosDisponiveis -= skills.custo;
            pontos.text = "Pontos: " + pontosDisponiveis;
            DesbloquearSkills(skills);
        }
        else {
            Debug.Log("skills não pode ser desbloqueado!");
        }
    }

    //Muda o status do skills de desbloqueado para true
    public void DesbloquearSkills(BaseSkills skill) {
        AdicionarSkill(skill);
    }

    //Verifica se será possível ser desbloqueado, para possível compra
    public bool PodeSerDesbloqueado(BaseSkills skills) {
        if (skills.desbloqueado) {
            return false;
        }
        else if (skills.preRequisitos == null || skills.preRequisitos.Count == 0)
            return true;
        foreach (var preRequisito in skills.preRequisitos) {
            if (!preRequisito.desbloqueado)
                return false;
        }

        return true;
    }

    public void AdicionarSkill(BaseSkills skills) {
        if (GameController.controller.HasUnitRecords(SelectedUnit)) {
            GameController.controller.UpdateUnitRecordsByID(SelectedUnit, skills);
        }
    }

    public void OnSelectedUnitChanged(String unitId) {
        this.SelectedUnit = unitId;

    }

    private void UpdatedSkillTree() {
        UnitRecords unitAux = GameController.controller.GetUnitRecords(this.SelectedUnit);
        skills = unitAux.GetUnitSKills();

    }

}