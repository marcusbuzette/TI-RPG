/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadingScript : MonoBehaviour
{

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private Slider loadingBar;


public void FadeIn(string nextScene)
{
    StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration, nextScene));
}

public void FadeOut()
{
    StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration, ""));
}

private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, string nextScene)
{
    float elapsedTime = 0.0f;
    while(elapsedTime<fadeDuration)
    {

        elapsedTime += Time.deltaTime;
        cg.alpha = Mathf.Lerp(start, end, elapsedTime/duration);
        yield return null;

    }
    cg.alpha = end;
    if (nextScene != "") GameController.controller.uicontroller.ChangeScene(nextScene);
}

    private IEnumerator FadeAndLoad(string nextScene) {
        // Aparecer o fade
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));

        // Carregamento assíncrono
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f) {
            if (loadingBar != null)
                loadingBar.value = op.progress;

            yield return null;
        }

        if (loadingBar != null)
            loadingBar.value = 1;

        // Delay opcional antes de ativar a nova cena
        yield return new WaitForSeconds(0.5f);
        op.allowSceneActivation = true;
    }

}*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadingScript : MonoBehaviour {
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private Slider loadingBar; // opcional

    public void FadeOut() {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
    }

    public void FadeAndLoadScene(string nextScene) {
        StartCoroutine(FadeAndLoad(nextScene));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration) {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    private IEnumerator FadeAndLoad(string nextScene) {
        // Executa o fade
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));

        // Carrega a cena assincronamente
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f) {
            if (loadingBar != null)
                loadingBar.value = op.progress;
            yield return null;
        }

        if (loadingBar != null)
            loadingBar.value = 1;

        yield return new WaitForSeconds(0.5f);
        op.allowSceneActivation = true;
    }
}

