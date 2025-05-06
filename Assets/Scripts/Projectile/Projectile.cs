using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 targetPosition;
    HealthSystem enemy;
    int damage;
    Unit attackedBy;
    bool miss;
    [SerializeField] TrailRenderer trailRenderer;

    public void Setup(Unit attackedBy, Vector3 targetPosition, HealthSystem enemy, int damage, bool miss) {
        this.attackedBy = attackedBy;
        this.targetPosition = targetPosition;
        this.enemy = enemy;
        this.damage = damage;
        this.miss = miss;

        trailRenderer.material.color = Color.white;
        trailRenderer.material.SetColor("_EmissionColor", Color.white);

        if (miss) {
            this.targetPosition.y = this.targetPosition.y + 5;
        }
    }
    public void Setup(Vector3 targetPosition, Color color) {
        miss = false;

        this.targetPosition = targetPosition;
        trailRenderer.material.color = color;
        trailRenderer.material.SetColor("_EmissionColor", color);
    }

    private void Update() {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if(!miss && distanceBeforeMoving < distanceAfterMoving ) {
            enemy?.Damage(damage, attackedBy);
            Destroy(gameObject);
        }
        Destroy(gameObject, 10f);
    }
}
