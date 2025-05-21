using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherVFXController : MonoBehaviour
{
[SerializeField] private GameObject fireMagicCastObject;
[SerializeField] private GameObject iceMagicCastObject;
[SerializeField] private GameObject poisonMagicCastObject; // Objeto "Fire Magic Cast" (pode ser atribuído manualmente ou encontrado automaticamente)
    private FireAttack fireAttackScript; // Referência ao script FireAttack
    private FreezeAttack iceAttackScript;
    private PoisonAttack poisonAttackScript;

    private void Awake()
    {

        fireMagicCastObject = GameObject.Find("Fire Magic Cast");
        fireAttackScript = GetComponentInChildren<FireAttack>();

        iceMagicCastObject = GameObject.Find("Ice Magic Cast");
        iceAttackScript = GetComponentInChildren<FreezeAttack>();


        poisonMagicCastObject = GameObject.Find("Poison Arrow Cast");
        poisonAttackScript = GetComponentInChildren<PoisonAttack>();

        // Se o script FireAttack NÃO for encontrado, desativa o "Fire Magic Cast"
        if (fireAttackScript == null && fireMagicCastObject != null)
        {
            fireMagicCastObject.SetActive(false);
            Debug.LogWarning("FireAttack script não encontrado. 'Fire Magic Cast' desativado.");
        }

        if ( iceAttackScript == null && iceMagicCastObject != null)
        {
            iceMagicCastObject.SetActive(false);
            Debug.LogWarning("IceAttack script não encontrado. 'Ice Magic Cast' desativado.");
        }

        if (poisonAttackScript == null &&  poisonMagicCastObject != null)
        {
            poisonMagicCastObject.SetActive(false);
            Debug.LogWarning("PoisonAttack script não encontrado. 'Fire Magic Cast' desativado.");
        }
    }

    public void FireCast()
    {

         fireMagicCastObject.SetActive(true);

    }

    public void CastEnd()
    {
    fireMagicCastObject?.SetActive(false);
    iceMagicCastObject?.SetActive(false);
    poisonMagicCastObject?.SetActive(false);
    }
}