﻿using UnityEngine;

public class ZombieHealth : MonoBehaviour, IHealth
{
    public float MaxHealth;

    float currentHealth;

    Animator animator;

    public event System.Action OnHit;

    private void Awake()
    {
        currentHealth = MaxHealth;
        animator = GetComponent<Animator>();
    }

    public bool IsDead() => currentHealth <= 0;

    public void Die()
    {
        if(IsDead())
        {
            animator.SetBool("Died", true);
            GetComponent<ZombieAI>().enabled = false;
            GetComponent<AudioSource>().Stop();
            Destroy(gameObject, 5);
        }
    }

    private void Update()
    {
        Die();
    }

    public void GiveDamage(int ammount)
    {
        currentHealth -= ammount;
        OnHit?.Invoke();
    }
}
