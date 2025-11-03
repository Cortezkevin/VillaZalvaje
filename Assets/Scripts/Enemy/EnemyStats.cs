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

    // Eventos para notificar cambios de vida
    public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public event Action OnEnemyDeath;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " recibi� " + damage + " de da�o. Vida: " + currentHealth + "/" + maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
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

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    // Getters p�blicos
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