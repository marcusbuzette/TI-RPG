using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{

    private Vector3 originalScale;
    private Vector3 targetScale;
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float speed = 10f;
    private Coroutine scaleCoroutine;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StartScaling(originalScale * scaleMultiplier);
    }

    public void OnPointerExit(PointerEventData eventData) {
        StartScaling(originalScale);
    }

    private void StartScaling(Vector3 newTargetScale) {
        targetScale = newTargetScale;

        // Se já tiver uma coroutine rodando, para ela antes
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleButton());
    }

    private IEnumerator ScaleButton() {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f) {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
            yield return null; // Espera o próximo frame
        }

        transform.localScale = targetScale; // Garante que termina exatamente no tamanho certo
    }
}
