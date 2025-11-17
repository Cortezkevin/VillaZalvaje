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

    // Eventos
    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnScoreChanged;
    public event Action OnPlayerDeath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // PlayerStats.cs


    void Start()
    {
        // --- CORRECCIÓN: Cargar el estado de LevelStartData al inicio de cualquier escena ---
        // Esto asegura que el jugador inicie el nivel con los stats que tenía al entrar (o por defecto en Nivel 1).
        currentHealth = GameData.LevelStartHealth;
        score = GameData.LevelStartScore;
        currentAmmo = GameData.LevelStartAmmo;

        // Inicializar el evento de UI con los valores cargados.
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnScoreChanged?.Invoke(score);
    }

    // -------- VIDA --------
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player recibió {damage} de daño. Vida: {currentHealth}/{maxHealth}");
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

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player curado {amount} puntos. Vida: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Jugador murió");
        OnPlayerDeath?.Invoke();

        // Desactivar movimiento y armas
        if (TryGetComponent(out PlayerMovement movement))
            movement.enabled = false;

        WeaponDisplay weapon = GetComponentInChildren<WeaponDisplay>();
        if (weapon != null)
            weapon.enabled = false;

        // Mostrar panel de Game Over
        UIManager.Instance?.ShowGameOverPanel();
    }

    // -------- GETTERS --------
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;

    // -------- SCORE --------
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score: {score}");
        OnScoreChanged?.Invoke(score);
    }

    // -------- REINICIO TOTAL --------
    public void ResetAllStats()
    {
        // --- CORRECCIÓN: Reiniciar usando los datos de LevelStartData ---
        currentHealth = GameData.LevelStartHealth;
        score = GameData.LevelStartScore;
        currentAmmo = GameData.LevelStartAmmo;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnScoreChanged?.Invoke(score);

        if (TryGetComponent(out PlayerMovement movement))
            movement.enabled = true;

        WeaponDisplay weapon = GetComponentInChildren<WeaponDisplay>();
        if (weapon != null)
            weapon.enabled = true;

        // Reposicionar en el punto de respawn
        Transform spawnPoint = GameObject.FindWithTag("SpawnPoint")?.transform;
        if (spawnPoint != null)
            transform.position = spawnPoint.position;

        // Nota: El inventario se reinicia por la llamada a InventoryManager.LoadLevelStartInventory() 
        // dentro de GameManager.RetryLevel() (o Start() del InventoryManager al recargar la escena).
    }
}
