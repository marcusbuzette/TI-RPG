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

    private IEnumerator GoToPosition(float time) {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = targetTransform.position;

        float elapsedTime = 0;

        while (elapsedTime < time) {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}
