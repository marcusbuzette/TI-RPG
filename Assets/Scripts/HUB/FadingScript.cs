using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingScript : MonoBehaviour
{

[SerializeField] private CanvasGroup canvasGroup;
[SerializeField] private float fadeDuration = 1.5f;


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

}
