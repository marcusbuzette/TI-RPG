using System.Collections.Generic;
using UnityEngine;

public class TalentManager : MonoBehaviour
{
    public List<Talento> talentos;
    public int pontosDisponiveis = 4;

    //Faz com que os talentos estejam desbloqueados ao iniciar a cena
    public void Start(){
      foreach (Talento talento in talentos) 
        {
            talento.desbloqueado = false; 
        }
    }

    //Verifica se existem pontos suficientes e todas as condições estão cumpridas para o desbloqueio do talento
    public void TentarDesbloquearTalento(Talento talento)
    {
        if (pontosDisponiveis >= talento.custo && PodeSerDesbloqueado(talento))
        {
            pontosDisponiveis -= talento.custo;
            DesbloquearTalento(talento); 
        }
        else
        {
            Debug.Log("Talento não pode ser desbloqueado!");
        }
    }

    //Muda o status do talento de desbloqueado para true
    public void DesbloquearTalento(Talento talento)
    {
        talento.desbloqueado = true;
        Debug.Log(talento.nome + " desbloqueado!");
    }

    //Verifica se será possível ser desbloqueado, para possível compra
    public bool PodeSerDesbloqueado(Talento talento)
    {
        if(talento.desbloqueado){
            return false;
        }
        else if (talento.preRequisitos == null || talento.preRequisitos.Count == 0)
            return true;
        foreach (var preRequisito in talento.preRequisitos)
        {
            if (!preRequisito.desbloqueado)
                return false;
        }

        return true;
    }

}