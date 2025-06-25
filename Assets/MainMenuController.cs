using System.Collections;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject LogoPuc;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        LogoPuc = GameObject.Find("Splash Screen");
        canvasGroup = LogoPuc.GetComponent<CanvasGroup>();
        Time.timeScale = 1f;
    }

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2f); // Espera 2 segundos

        float duration = 1.5f; // Duração do fade
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        LogoPuc.SetActive(false); // Opcional: desativa após fade
    }
}