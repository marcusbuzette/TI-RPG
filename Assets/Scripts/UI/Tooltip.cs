using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Tooltip : MonoBehaviour {
    public static Tooltip Instance;
    [SerializeField] private RectTransform canvasTranform;
    private TMP_Text text;
    private RectTransform background;
    private RectTransform rectTransform;

    private Vector2 toolTipPos;

    void Awake() {
        Instance = this;
        background = transform.Find("Background").GetComponent<RectTransform>();
        text = background.Find("Text").GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
        HideTooltip();
    }

    void Update() {
        this.toolTipPos = Input.mousePosition / canvasTranform.localScale.x;

        if (toolTipPos.x + background.rect.width > canvasTranform.rect.width) {
            toolTipPos.x = canvasTranform.rect.width - background.rect.width; 
        }

        if (toolTipPos.y + background.rect.height > canvasTranform.rect.height) {
            toolTipPos.y = canvasTranform.rect.height - background.rect.height; 
        }

        rectTransform.anchoredPosition = toolTipPos;
    }



    private void Show(string showText) { 
        gameObject.SetActive(true); 
        background.gameObject.SetActive(true);
        this.text.text = showText;
        Vector2 backgroundSize =  new Vector2(
            this.text.preferredWidth, this.text.preferredHeight );

        this.background.sizeDelta = backgroundSize;
    }

    private void Hide() { 
        background.gameObject.SetActive(false);
        gameObject.SetActive(false); 
    }

    public void ShowTooltip(string textToShow) {
        Instance.Show(textToShow);
    }

    public void HideTooltip() {
        Instance.Hide();
    }
}
