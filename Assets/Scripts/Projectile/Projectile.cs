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

    public void Setup(Unit attackedBy, Vector3 targetPosition, HealthSystem enemy, int damage, bool miss) {
        this.attackedBy = attackedBy;
        this.targetPosition = targetPosition;
        this.enemy = enemy;
        this.damage = damage;
        this.miss = miss;

        if(miss) {
            this.targetPosition.y = this.targetPosition.y + 5;
        }
    }

    private void Update() {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if(!miss && distanceBeforeMoving < distanceAfterMoving ) {
            enemy.Damage(damage, attackedBy);
            Destroy(gameObject);
        }
        Destroy(gameObject, 10f);
    }
}
