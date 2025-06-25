using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;


public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public int level;
    public string sceneToLoad;
    public string levelName;
    private Button levelButton;
    private Vector3 originalScale;
    private Vector3 targetScale;
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject levelTMP;
    private Coroutine scaleCoroutine;

    [SerializeField] private FadingScript fadingScript;

    void OnEnable() {
        originalScale = transform.localScale;
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(() => {
            fadingScript.FadeAndLoadScene(sceneToLoad);
            AudioManager.instance.PlayMusic("Combat");
            AudioManager.instance.PlayAmbient("AmbientFloresta");
       
        });
    }

    public void OnPointerEnter(PointerEventData eventData) {
        levelTMP.SetActive(true);
        StartScaling(originalScale * scaleMultiplier);
    }

    public void OnPointerExit(PointerEventData eventData) {
        levelTMP.SetActive(false);
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
