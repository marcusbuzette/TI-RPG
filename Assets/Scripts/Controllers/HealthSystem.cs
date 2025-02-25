using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{

    public event EventHandler OnDead;
    public event EventHandler OnDamage;
    public int healthPoints = 100;
    public int maxHealthPoints = 100;
    public Animator animator;

    private void Awake()
    {
    animator = GetComponentInChildren<Animator>();
    }

    public void Damage(int damage)
    {

        healthPoints -= damage;
        animator.SetTrigger("TakeDamage");

        if (healthPoints < 0) healthPoints = 0;
        OnDamage?.Invoke(this, EventArgs.Empty);
        if (healthPoints == 0) Die();
    }

    private void Die()
    {
        OnDead.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPointsNormalized()
    {
        return (float)healthPoints / maxHealthPoints;
    }

    public void Heal(int amount)
    {
        amount = 20;
        healthPoints += amount;
        if (healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
        OnDamage?.Invoke(this, EventArgs.Empty);
    }

    public HealthSystem GetHealthSystem()
    {
        return this;
    }

    public int GetHealthPoints()
    {
        Debug.Log(healthPoints);
        return this.healthPoints;
    }

}
