using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstruction : MonoBehaviour {
    public Transform[] targets; // Personagens que precisam ficar visíveis
    public float fadeSpeed = 3f;
    public float minAlpha = 0.2f;

    private MaterialPropertyBlock propBlock;
    private Dictionary<Renderer, float> fadingObjects = new Dictionary<Renderer, float>();
    private Renderer[] potentialObstructors;

    void Start() {
        propBlock = new MaterialPropertyBlock();
        // Assume que os objetos que podem obstruir estão com tag "Obstruction"
        GameObject[] obstructionObjects = GameObject.FindGameObjectsWithTag("Obstruction");
        List<Renderer> renderers = new List<Renderer>();

        foreach (var obj in obstructionObjects) {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
                renderers.Add(rend);
        }

        potentialObstructors = renderers.ToArray();
    }

    void Update() {
        HashSet<Renderer> obstructingThisFrame = new HashSet<Renderer>();

        foreach (var target in targets) {
            if (target == null) continue;
            Vector3 camPos = transform.position;
            Vector3 dirToTarget = target.position - camPos;
            float distToTarget = dirToTarget.magnitude;

            foreach (var rend in potentialObstructors) {
                Bounds b = rend.bounds;
                Vector3 objPos = b.center;

                Vector3 toObj = objPos - camPos;
                float proj = Vector3.Dot(toObj, dirToTarget.normalized);

                if (proj > 0 && proj < distToTarget) {
                    Vector3 closestPoint = camPos + dirToTarget.normalized * proj;
                    float distanceFromLine = Vector3.Distance(closestPoint, objPos);

                    if (distanceFromLine < Mathf.Max(b.extents.x, b.extents.z)) // largura da bounding box
                    {
                        FadeRenderer(rend, minAlpha);
                        obstructingThisFrame.Add(rend);
                    }
                }
            }
        }

        // Voltar à opacidade para objetos que não obstruem mais
        var toRestore = new List<Renderer>(fadingObjects.Keys);
        foreach (var rend in toRestore) {
            if (!obstructingThisFrame.Contains(rend))
                FadeRenderer(rend, 1f);
        }
    }

    void FadeRenderer(Renderer rend, float targetAlpha) {
        if (!fadingObjects.ContainsKey(rend))
            fadingObjects[rend] = 1f;

        float currentAlpha = fadingObjects[rend];
        float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        fadingObjects[rend] = newAlpha;

        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Alpha", newAlpha);
        rend.SetPropertyBlock(propBlock);
    }

}
