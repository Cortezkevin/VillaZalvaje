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

    [Header("Audio")] // NUEVO: Configuración de Audio
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionClip;

    [Header("Colliders")]
    public Collider2D normalCollider;
    public Collider2D deadCollider;

    public float explosionRadius = 3f;
    public int maxDamage = 80;    // daño más cercano
    public int midDamage = 50;    // daño intermedio
    public int minDamage = 20;    // daño lejano

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // NUEVO: Intentar obtener el AudioSource si no está asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

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
        // 1. Reproducir audio de explosión
        PlayExplosionSound(); // NUEVO

        animator.SetTrigger("dead");

        Explode();

        CameraShakeController shake = FindAnyObjectByType<CameraShakeController>();
        if (shake != null)
        {
            shake.Shake(2f, 5f, 0.3f);
        }


        if (normalCollider != null) normalCollider.enabled = false;
        if (deadCollider != null) deadCollider.enabled = true;

        yield break;

    }

    // NUEVO MÉTODO PARA REPRODUCIR EL SONIDO
    private void PlayExplosionSound()
    {
        if (audioSource != null && explosionClip != null)
        {
            // Reproduce el clip de una sola vez
            audioSource.PlayOneShot(explosionClip);
        }
        else
        {
            Debug.LogWarning("AudioSource o ExplosionClip no asignado en la estación de gas.");
        }
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("enemy"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                int damageToApply = 0;

                if (distance <= explosionRadius * 0.33f)
                    damageToApply = maxDamage;      // cerca
                else if (distance <= explosionRadius * 0.66f)
                    damageToApply = midDamage;      // medio
                else
                    damageToApply = minDamage;      // lejos

                // Asume que cada objetivo tiene un método TakeDamage(int)
                hit.SendMessage("TakeDamage", damageToApply, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el radio de daño en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
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