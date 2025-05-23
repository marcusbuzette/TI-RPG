using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPTextFollower : MonoBehaviour
{
    public Transform target3D;
    public Vector3 offset;
    public Camera targetCamera;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target3D == null || targetCamera == null) return;
        Vector3 screenPos = targetCamera.WorldToScreenPoint(target3D.position + offset);
        rectTransform.position = screenPos;
    }
}

