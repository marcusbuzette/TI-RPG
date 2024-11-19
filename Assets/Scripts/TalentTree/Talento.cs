using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo Talento", menuName = "Talento")]
public class Talento : ScriptableObject
{
    public string nome;
    public string descricao;
    public int custo;
    public List<Talento> preRequisitos;
    public bool desbloqueado;

}