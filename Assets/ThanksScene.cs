using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThanksScene : MonoBehaviour
{
    [SerializeField] private float timeBeforeFade = 5f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private void Start()
    {
        StartCoroutine(ThanksRoutine());
    }

    private IEnumerator ThanksRoutine()
    {
        // Aguarda o tempo definido
        yield return new WaitForSeconds(timeBeforeFade);

        // Inicia o fade out
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;

        // Aguarda um pouco e muda para o menu
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(menuSceneName);
    }
}
