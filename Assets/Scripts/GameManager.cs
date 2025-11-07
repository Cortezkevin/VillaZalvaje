using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public UIManager uiManager;
    public PlayerStats playerStats;
    public InventoryManager inventoryManager;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeReferences();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reasignar referencias al cargar una nueva escena
        InitializeReferences();

        if (playerStats != null && playerStats.GetCurrentHealth() <= 0)
        {
            // Evitar revivir muerto sin reiniciar
            playerStats.ResetHealth();
        }

        Time.timeScale = 1f;
        isGameOver = false;
    }

    private void InitializeReferences()
    {
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();

        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();

        // Suscribirse a eventos del jugador
        if (playerStats != null)
        {
            playerStats.OnPlayerDeath -= HandleGameOver;
            playerStats.OnPlayerDeath += HandleGameOver;
        }
    }

    private void HandleGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowGameOverPanel();
        }
    }

    public void RetryLevel()
    {
        if (playerStats != null)
            playerStats.ResetHealth();

        if (inventoryManager != null)
            inventoryManager.ClearInventory();

        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
