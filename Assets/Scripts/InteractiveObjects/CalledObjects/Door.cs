using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ICalledObject {
    [Header("Transform target"), SerializeField]
    Transform targetTransform;

    public float transformTime;

    public void Action() {
        StartCoroutine(GoToPosition(transformTime));
        
        SetIsWalkableNode(true);
    }

    private void SetIsWalkableNode(bool isWalkable) {
        PathFinding.Instance.SetNodeIsWalkable(transform.position ,isWalkable);
    }

    /*private IEnumerator GoToPosition(float time) {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = targetTransform.position;
        Debug.Log("Entrou");
        float elapsedTime = 0;

        while (elapsedTime < time) {
            Debug.Log("indo");
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }*/

    private IEnumerator GoToPosition(float time) {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = targetTransform.position;

        Quaternion startingRot = transform.rotation;
        Quaternion finalRot = targetTransform.rotation;

        float elapsedTime = 0;

        while (elapsedTime < time) {
            float t = elapsedTime / time;
            transform.position = Vector3.Lerp(startingPos, finalPos, t);
            transform.rotation = Quaternion.Lerp(startingRot, finalRot, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que a posição e rotação finais sejam exatamente as do target
        transform.position = finalPos;
        transform.rotation = finalRot;
    }

}
