using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    [SerializeField] private GameObject charsButton;
    [SerializeField] private GameObject menuContainer;

    void OnEnable() {
        if (GameController.controller.uicontroller.GetCurrentSceneName() == "MenuPrincipal") {
            charsButton?.SetActive(false);
        } else {
            charsButton?.SetActive(true);
        }

        StartCoroutine(AnimateMenu());
    }

    public void MainMenu() {
        GameController.controller.uicontroller.ChangeScene("MenuPrincipal");
    }

    public void OpenCharsScreen() {
        ResumeGame();
        GameController.controller.ToggleCharsPanel();
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void ResumeGame() {
        GameController.controller.ResumeGame();
    }

    private IEnumerator AnimateMenu() {
        CanvasGroup canvasGroup = menuContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = menuContainer.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        menuContainer.transform.localScale = Vector3.zero;

        float duration = 0.6f;
        float time = 0f;

        while (time < duration) {
            time += Time.unscaledDeltaTime; // importante se Time.timeScale = 0
            float t = time / duration;

            float bounceT = EaseOutBack(t);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            menuContainer.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, bounceT);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        menuContainer.transform.localScale = Vector3.one;
    }

    private float EaseOutBack(float t) {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
}
