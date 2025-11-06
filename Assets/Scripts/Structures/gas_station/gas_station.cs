using System;
using System.Collections;
using UnityEngine;

public class gas_station : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 100;
    public int currentHealth;
    private bool isDead = false;

    [Header("Animaciones")]
    public Animator animator;

    [Header("Colliders")]
    public Collider2D normalCollider;
    public Collider2D deadCollider;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (normalCollider != null) normalCollider.enabled = true;
        if (deadCollider != null) deadCollider.enabled = false;
    }

    IEnumerator PlayDamageAnimation()
    {
        animator.SetTrigger("shot");
        yield return new WaitForSeconds(0.3f); // duración aproximada
        animator.ResetTrigger("shot"); // limpia el trigger
        animator.Play("Idle"); // fuerza volver al idle
    }


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            StartCoroutine(PlayDamageAnimation());
        }
        else
        {
            isDead = true;
            
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {

        animator.SetTrigger("dead");
        if (normalCollider != null) normalCollider.enabled = false;
        if (deadCollider != null) deadCollider.enabled = true;

        yield break;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(bullet.damage);
            Destroy(bullet.gameObject);
        }
    }
}
