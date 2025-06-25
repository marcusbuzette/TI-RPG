using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeUnitButtonUI : MonoBehaviour {

    private string unitId;
    private string unitName;

    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private GameObject upgradeAvailableIcon;
    [SerializeField] private Sprite selectedImage;
    [SerializeField] private Color selectedColor;

    private Sprite originalSprite;
    private Color originalColor;
    private Button buttonRef;
    private Vector3 originalScale;
    private Coroutine currentAnimation;

    private void OnEnable() {
        if (TalentManager.Instance != null) {
            TalentManager.Instance.onSkillUpdate += TalentManager_OnSkillUpdate;
        }
        UpdateUpgradeAvailableIcon();
    }

    private void OnDisable() {
        if (TalentManager.Instance != null)
            TalentManager.Instance.onSkillUpdate -= TalentManager_OnSkillUpdate;
    }

    private void TalentManager_OnSkillUpdate(object sender, EventArgs e) {
        UpdateUpgradeAvailableIcon();
    }

    public void SetUnitData(string unitId, string unitName) {
        this.unitId = unitId;
        this.unitName = unitName;

        buttonRef = GetComponent<Button>();
        originalSprite = buttonRef.image.sprite;
        originalColor = buttonRef.image.color;
        originalScale = transform.localScale;

        buttonText.text = unitName;

        UpdateUpgradeAvailableIcon();
    }

    public void SetSelected() {
        buttonRef.image.color = selectedColor;
        buttonRef.image.sprite = selectedImage;

        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateSelection());
    }

    public void ResetSelected() {
        buttonRef.image.sprite = originalSprite;
        buttonRef.image.color = originalColor;

        if (currentAnimation != null) StopCoroutine(currentAnimation);
        transform.localScale = originalScale;
    }

    private System.Collections.IEnumerator AnimateSelection() {
        float duration = 0.2f;
        float time = 0f;
        Vector3 targetScale = originalScale * 1.1f;

        while (time < duration) {
            time += Time.deltaTime;
            float t = Mathf.Sin((time / duration) * Mathf.PI * 0.5f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            float t = 1f - Mathf.Cos((time / duration) * Mathf.PI * 0.5f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
        currentAnimation = null;
    }

    public string GetUnitId() => unitId;
    public string GetUnitName() => unitName;

    private void UpdateUpgradeAvailableIcon() {
        if (TalentManager.Instance == null || string.IsNullOrEmpty(unitId) || upgradeAvailableIcon == null) {
            if (upgradeAvailableIcon != null) upgradeAvailableIcon.SetActive(false);
            return;
        }

        Unit unit = TalentManager.Instance.GetUnitList()
            .Find(u => u.GetComponent<Unit>().GetUnitId() == unitId)
            ?.GetComponent<Unit>();

        if (unit == null) {
            upgradeAvailableIcon.SetActive(false);
            return;
        }

        bool hasAvailable = TalentManager.Instance.HasAvailableUpgradesOrSkills(unitId);
        upgradeAvailableIcon.SetActive(hasAvailable);
    }
}
