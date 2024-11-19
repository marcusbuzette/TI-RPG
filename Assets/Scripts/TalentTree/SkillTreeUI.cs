using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUi : MonoBehaviour
{
    public Talento talento;
    public Text nome;
    public Text descricao;
    public Text custo;
    public Button botaoDesbloquear;
    public TalentManager talentManager;

    //Define e altera os nodes da arvore de talentos.
    void Start()
    {
        nome.text = talento.nome;
        descricao.text = talento.descricao;
        custo.text = talento.custo.ToString();
    }

    //Mantem o botao interagivel se cumprir as condições.
    void Update()
    {
        botaoDesbloquear.interactable =
            talentManager.PodeSerDesbloqueado(talento) &&
            talentManager.pontosDisponiveis >= talento.custo;
    }
}
