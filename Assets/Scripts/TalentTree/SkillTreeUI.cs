using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUi : MonoBehaviour
{
    public BaseSkills skills;
    public Text nome;
    public Text descricao;
    public Text custo;
    public Button botaoDesbloquear;
    public TalentManager talentManager;

    //Define e altera os nodes da arvore de talentos.   
    void Start()
    {
        nome.text = skills.nome;
        descricao.text = skills.descricao;
        custo.text = skills.custo.ToString();
    }

    //Mantem o botao interagivel se cumprir as condições.
    void Update()
    {
        botaoDesbloquear.interactable =
            talentManager.PodeSerDesbloqueado(skills) &&
            talentManager.pontosDisponiveis >= skills.custo;
    }
}
