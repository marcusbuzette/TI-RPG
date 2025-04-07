using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TooltipPosition  { TOP, LEFT, RIGHT, BOTTOM, NULL }

public class Tooltip : MonoBehaviour {

    public static Tooltip Instance;
    [SerializeField] private RectTransform canvasTranform;
    [SerializeField] private float MAX_WIDTH = 300;
    private TMP_Text text;
    private RectTransform background;
    private RectTransform rectTransform;

    private Vector2 toolTipPos;
    private Transform originButtonRef;
    private RectTransform originButtonRectRef;
    private TooltipPosition tooltipLoc = TooltipPosition.NULL;

    void Awake() {
        Instance = this;
        background = transform.Find("Background").GetComponent<RectTransform>();
        text = background.Find("Text").GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
        HideTooltip();
    }

    void Update() {
        this.toolTipPos = Input.mousePosition / canvasTranform.localScale.x;
        switch(this.tooltipLoc) {
            case TooltipPosition.TOP:
            toolTipPos.x = (this.originButtonRef.position.x / canvasTranform.localScale.x) - rectTransform.rect.width/2;
            toolTipPos.y = 
                (this.originButtonRef.position.y / canvasTranform.localScale.x) + this.originButtonRectRef.rect.height/2;
                break;
            case TooltipPosition.BOTTOM:
                toolTipPos.y = 
                (this.originButtonRef.position.y / canvasTranform.localScale.x) - this.originButtonRectRef.rect.height/2;
                break;
            case TooltipPosition.LEFT:
                toolTipPos.x = 
                (this.originButtonRef.position.x / canvasTranform.localScale.x) - this.originButtonRectRef.rect.width/2 - rectTransform.rect.width;
                break;
            case TooltipPosition.RIGHT:
                toolTipPos.x = 
                (this.originButtonRef.position.x / canvasTranform.localScale.x) + this.originButtonRectRef.rect.width/2;
                break;
        }

        if (toolTipPos.x + background.rect.width > canvasTranform.rect.width) {
            toolTipPos.x = canvasTranform.rect.width - background.rect.width; 
        }

        if (toolTipPos.y + background.rect.height > canvasTranform.rect.height) {
            toolTipPos.y = canvasTranform.rect.height - background.rect.height; 
        }

        rectTransform.anchoredPosition = toolTipPos;
    }



    private void Show(string showText, Transform button, TooltipPosition pos) { 
        this.SetOriginButtonRef(button, pos);
        gameObject.SetActive(true); 
        background.gameObject.SetActive(true);
        this.text.text = showText;
        Vector2 backgroundSize =  new Vector2(
            this.text.preferredWidth > MAX_WIDTH ? MAX_WIDTH : this.text.preferredWidth,
             this.text.preferredHeight );

        this.background.sizeDelta = backgroundSize;
    }

    private void Hide() { 
        background.gameObject.SetActive(false);
        gameObject.SetActive(false); 
    }

    public void ShowTooltip(string textToShow, Transform button, TooltipPosition pos = TooltipPosition.NULL) {
        Instance.Show(textToShow, button, pos);
    }

    public void HideTooltip() {
        Instance.Hide();
    }

    private void SetOriginButtonRef(Transform button, TooltipPosition pos) {
        this.originButtonRef = button;
        this.tooltipLoc = pos;
        this.originButtonRectRef = button.GetComponent<RectTransform>();
    }

    private void RemoveOriginBurronRef() {
        this.originButtonRef = null;
        this.originButtonRectRef = null;
        this.tooltipLoc = TooltipPosition.NULL;
    }
}
