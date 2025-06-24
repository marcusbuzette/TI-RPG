using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public event EventHandler onDestory;

    Vector3 targetPosition;
    Vector3 startPosition;
    HealthSystem enemy;
    int damage;
    Unit attackedBy;
    bool miss;
    Vector3 missMoveDir;

    [SerializeField] TrailRenderer trailRenderer;

    bool useParabola = false;
    float totalTravelTime = 1f;
    float travelTimer = 0f;
    float arcHeight = 5f;

    public void Setup(Unit attackedBy, Vector3 targetPosition, HealthSystem enemy, int damage, bool miss) {
        this.attackedBy = attackedBy;
        this.targetPosition = targetPosition;
        this.enemy = enemy;
        this.damage = damage;
        this.miss = miss;

        trailRenderer.material.color = Color.white;
        trailRenderer.material.SetColor("_EmissionColor", Color.white);

        if (miss) {
            this.targetPosition.y += 5f;
            missMoveDir = (this.targetPosition - transform.position).normalized;
        }

        startPosition = transform.position;
    }

    public void Setup(Vector3 targetPosition, Color color,bool useParabola = false) {
        this.miss = false;
        this.useParabola = useParabola;
        this.targetPosition = targetPosition;

        startPosition = transform.position;

        trailRenderer.material.color = color;
        trailRenderer.material.SetColor("_EmissionColor", color);
    }

    private void Update() {
        if (miss) {
            MoveMiss();
        }
        else if (useParabola) {
            MoveParabola();
        }
        else {
            MoveStraight();
        }

        Destroy(gameObject, 4f); // Destruição de segurança
    }

    void MoveStraight() {
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        float distanceBefore = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 50f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfter = Vector3.Distance(transform.position, targetPosition);

        if (distanceBefore < distanceAfter) {
            enemy?.Damage(damage, attackedBy);
            Destroy(gameObject);
        }
    }

    void MoveParabola() {
        travelTimer += Time.deltaTime;
        float t = Mathf.Clamp01(travelTimer / totalTravelTime);

        Vector3 linearPos = Vector3.Lerp(startPosition, targetPosition, t);
        float heightOffset = arcHeight * 4 * (t - t * t); // parábola suave
        linearPos.y += heightOffset;

        transform.position = linearPos;

        if (t >= 1f) {
            enemy?.Damage(damage, attackedBy);
            Destroy(gameObject);
        }
    }

    void MoveMiss() {
        float moveSpeed = 50f;
        transform.position += missMoveDir * moveSpeed * Time.deltaTime;
    }

    private void OnDestroy() {
        onDestory?.Invoke(this, EventArgs.Empty);
    }
}
