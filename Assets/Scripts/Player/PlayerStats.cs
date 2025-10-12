using UnityEngine;
using System;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Player Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Optional Stats")]
    public int maxAmmo = 30;
    public int currentAmmo = 30;
    public int score = 0;

    // Eventos para notificar cambios (opcional pero útil)
    public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public event Action OnPlayerDeath;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantener entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Inicializar vida al máximo
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player recibió " + damage + " de daño. Vida: " + currentHealth + "/" + maxHealth);

        // Flash rojo (opcional)
        StartCoroutine(DamageFlash());

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white;
        }
    }

    // Método para curar
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player curado " + amount + " puntos. Vida: " + currentHealth + "/" + maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Método para establecer vida al máximo
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("¡Player murió!");
        OnPlayerDeath?.Invoke();

        // Aquí puedes agregar lógica de muerte:
        // - Mostrar pantalla de Game Over
        // - Reiniciar nivel
        // - Desactivar controles
        // etc.
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

    // Método para agregar puntos
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
    }
}