using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeUnitButtonUI : MonoBehaviour {

    private string unitId;
    private string unitName;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private GameObject upgardeAvailave;
    [SerializeField] private Sprite selectedImage;
    [SerializeField] private Color selecteColor;
    private Sprite originalSprite;
    private Color originalColor;
    private Button buttonRef;
    private Vector3 originalScale;
    private Coroutine currentAnimation;

    public void SetUnitData(string unitId, string unitName) {
        this.unitId = unitId;
        this.unitName = unitName;
        this.buttonText.text = unitName;
        buttonRef = GetComponent<Button>();
        originalSprite = buttonRef.image.sprite;
        originalColor = buttonRef.image.color;
        originalScale = transform.localScale;
    }

    public void SetSelected() {
        buttonRef.image.color = this.selecteColor;
        buttonRef.image.sprite = selectedImage;

        // Cancela animação anterior se estiver rolando
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateSelection());
    }

    public void ResetSelected() {
        buttonRef.image.sprite = this.originalSprite;
        buttonRef.image.color = originalColor;

        // Cancela animação anterior e reseta escala
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        transform.localScale = originalScale;
    }

    private IEnumerator AnimateSelection() {
        float duration = 0.2f;
        float time = 0f;
        Vector3 targetScale = originalScale * 1.1f;

        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // EaseOutSine

            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Retorna ao normal suavemente
        time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f); // EaseInSine

            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
        currentAnimation = null;
    }

    public string GetUnitId() { return unitId; }
    public string GetUnitName() { return unitName; }

    public void ShowUpgradeAvailable(bool show) {
        if (upgardeAvailave != null) {
            upgardeAvailave.SetActive(show);
        }
    }
}
