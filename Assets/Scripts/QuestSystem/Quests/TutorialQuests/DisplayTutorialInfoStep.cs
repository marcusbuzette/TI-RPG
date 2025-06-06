using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTutorialInfoStep : TutorialStep {

    [SerializeField] private GameObject[] messages;
    private Canvas canvas;
    private int messageIndex = 0;
    private GameObject currentMessage;

    void Start() {
        GameObject canvasObj = GameObject.FindGameObjectWithTag("UICanvas");
        if (canvasObj != null) {
            canvas = canvasObj.GetComponent<Canvas>();
        }
        else {
            Debug.LogError("Canvas com a tag 'TutorialCanvas' não foi encontrado.");
        }
        ShowMessage(messageIndex);
    }

    private void ShowMessage(int index) {
        if (currentMessage != null) Destroy(currentMessage);

        currentMessage = Instantiate(messages[index], canvas.transform);


        StartCoroutine(AnimateMessage(currentMessage));
    }


    void Update() {
        // Exemplo simples: avança com clique do mouse ou pressionar espaço
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
            NextMessage();
        }
    }

    private void NextMessage() {
        messageIndex++;
        if (messageIndex > messages.Length) {
            FinishQuestStep();
        }
        else {
            ShowMessage(messageIndex);
        }
    }

    private IEnumerator AnimateMessage(GameObject message) {
        CanvasGroup canvasGroup = message.GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = message.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        message.transform.localScale = Vector3.zero;

        float duration = 0.6f;
        float time = 0f;

        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;

            float bounceT = EaseOutBack(t);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            message.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, bounceT);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        message.transform.localScale = Vector3.one;
    }

    private float EaseOutBack(float t) {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }





}
