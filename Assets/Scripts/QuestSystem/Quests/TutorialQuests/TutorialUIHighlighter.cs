using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIHighlighter : MonoBehaviour {

    [Tooltip("Nomes dos objetos de UI que devem ser destacados")]
    [SerializeField] private string[] uiElementNames;

    private class HighlightData {
        public Canvas canvas;
        public bool hadCanvas;
        public bool originalOverrideSorting;
        public int originalSortingOrder;
        public Outline outline;
        public Coroutine pulseCoroutine;
    }

    private Dictionary<GameObject, HighlightData> highlightedElements = new();

    void Start() {
        HighlightElements();
    }

    public void HighlightElements() {
        foreach (string name in uiElementNames) {
            GameObject target = GameObject.Find(name);
            if (target == null) continue;

            // Garantir Canvas
            Canvas canvas = target.GetComponent<Canvas>();
            bool hadCanvas = canvas != null;
            if (!hadCanvas) {
                canvas = target.AddComponent<Canvas>();
            }

            var data = new HighlightData {
                canvas = canvas,
                hadCanvas = hadCanvas,
                originalOverrideSorting = canvas.overrideSorting,
                originalSortingOrder = canvas.sortingOrder
            };

            canvas.overrideSorting = true;
            canvas.sortingOrder = 999;

            // Adiciona Outline
            Outline outline = target.GetComponent<Outline>();
            if (outline == null) {
                outline = target.AddComponent<Outline>();
                outline.effectColor = Color.yellow;
                outline.effectDistance = new Vector2(3f, -3f);
            }
            data.outline = outline;

            // Animação de pulsar
            data.pulseCoroutine = StartCoroutine(PulseEffect(target.transform));

            highlightedElements[target] = data;
        }
    }

    public void RestoreElements() {
        foreach (var kvp in highlightedElements) {
            GameObject obj = kvp.Key;
            HighlightData data = kvp.Value;

            // Restaurar Canvas
            data.canvas.overrideSorting = data.originalOverrideSorting;
            data.canvas.sortingOrder = data.originalSortingOrder;
            if (!data.hadCanvas) {
                Destroy(data.canvas);
            }

            // Remover Outline se foi adicionado agora
            if (data.outline != null) {
                Destroy(data.outline);
            }

            // Parar animação
            if (data.pulseCoroutine != null) {
                StopCoroutine(data.pulseCoroutine);
            }

            // Resetar escala
            obj.transform.localScale = Vector3.one;
        }

        highlightedElements.Clear();
    }

    private IEnumerator PulseEffect(Transform target) {
        Vector3 baseScale = Vector3.one;
        float speed = 4f;
        float scaleAmount = 1.05f;

        while (true) {
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f; // 0..1
            target.localScale = Vector3.Lerp(baseScale, baseScale * scaleAmount, t);
            yield return null;
        }
    }

    void OnDestroy() {
        this.RestoreElements();
    }
}
