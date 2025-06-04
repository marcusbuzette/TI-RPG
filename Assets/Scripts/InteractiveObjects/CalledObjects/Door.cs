using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ICalledObject {
    [Header("Transform target"), SerializeField]
    Transform targetTransform;

    [Header("Transform target"), SerializeField]
    Transform[] pathNodesPositions;

    public float transformTime;

    private GameObject m_Camera;

    public void Action(GameObject m_Camera) {
        m_Camera.SetActive(true);
        this.m_Camera = m_Camera;

        StartCoroutine(GoToPosition(transformTime));
        
        SetIsWalkableNodes(true);
    }

    public void Start() {
        SetIsWalkableNodes(false);
    }

    private void SetIsWalkableNodes(bool isWalkable) {
        foreach (var node in pathNodesPositions) {
            PathFinding.Instance.SetNodeIsWalkable(node.position, isWalkable);
        }
    }

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

        m_Camera.SetActive(false);

        // Garante que a posição e rotação finais sejam exatamente as do target
        transform.position = finalPos;
        transform.rotation = finalRot;
    }

}
