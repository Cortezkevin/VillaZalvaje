using UnityEngine;
using System;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Optional Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0.1f;

    // --- NUEVO: Audio de Daño ---
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitClip;
    // ----------------------------

    // Eventos para notificar cambios de vida
    public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public event Action OnEnemyDeath;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Obtener AudioSource si no está asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // NUEVO: Reproducir sonido de daño al recibir impacto
        PlayHitSound();

        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Vida: " + currentHealth + "/" + maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- NUEVO MÉTODO PARA REPRODUCIR SONIDO DE DAÑO ---
    private void PlayHitSound()
    {
        if (audioSource != null && hitClip != null)
        {
            // Usamos PlayOneShot para que el audio de daño no interrumpa el audio de pasos
            audioSource.PlayOneShot(hitClip);
        }
        // No mostramos Warning aquí, ya que el AudioSource puede estar en EnemyMovement.
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " curado " + amount + " puntos. Vida: " + currentHealth + "/" + maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " murio!");
        PlayerStats.Instance.AddScore(10); // Otorga 10 puntos al jugador al morir el enemigo
        OnEnemyDeath?.Invoke();

        // Puedes añadir una reproducción de sonido de muerte aquí si quieres

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    // Getters públicos
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}