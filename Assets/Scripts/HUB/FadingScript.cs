using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingScript : MonoBehaviour
{

[SerializeField] private CanvasGroup canvasGroup;
[SerializeField] private float fadeDuration = 1.5f;


public void FadeIn()
{
    StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
}

public void FadeOut()
{
    StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
}

private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
{

    yield return new WaitForSeconds(1.5f);
    float elapsedTime = 0.0f;
    while(elapsedTime<fadeDuration)
    {

        elapsedTime += Time.deltaTime;
        cg.alpha = Mathf.Lerp(start, end, elapsedTime/duration);
        yield return null;

    }
    cg.alpha = end;
}

}
