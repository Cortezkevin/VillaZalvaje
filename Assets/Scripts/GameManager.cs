using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public UIManager uiManager;
    public PlayerStats playerStats;
    public InventoryManager inventoryManager;
    public Animator transitionAnim;

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
    InitializeReferences();

    // Reubicar jugador si existe SpawnPoint
    Transform spawnPoint = GameObject.FindWithTag("SpawnPoint")?.transform;
    if (spawnPoint != null && playerStats != null)
    {
        playerStats.transform.position = spawnPoint.position;
    }

    // Ocultar panel Game Over
    UIManager.Instance?.HideGameOverPanel();

    // **Nuevo: reproducir animación "Start" al cargar el nuevo nivel**
    Animator transitionAnim = Object.FindFirstObjectByType<Animator>(FindObjectsInactive.Include);
    if (transitionAnim != null && transitionAnim.runtimeAnimatorController != null)
    {
        if (transitionAnim.HasParameter("Start"))
        {
            transitionAnim.ResetTrigger("End");
            transitionAnim.SetTrigger("Start");
        }
    }

    Time.timeScale = 1f;
    isGameOver = false;
}



    private void InitializeReferences()
    {
        // Referencias preferibles via singleton para evitar problemas de reasignación
        if (uiManager == null) uiManager = UIManager.Instance;
        if (playerStats == null) playerStats = PlayerStats.Instance;
        if (inventoryManager == null) inventoryManager = InventoryManager.Instance;

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
        UIManager.Instance?.ShowGameOverPanel();
    }

    // -------- Modificación clave: RetryLevel --------
    public void RetryLevel()
    {
        // Ocultar panel de Game Over
        UIManager.Instance?.HideGameOverPanel();

        // Resetear stats del jugador.
        // ESTO CARGARÁ EL ESTADO GUARDADO EN GAMEDATA (incluyendo score y vida).
        PlayerStats.Instance?.ResetAllStats();

        // *** ELIMINAR CUALQUIER LLAMADA A InventoryManager.Instance.ClearInventory() AQUÍ ***
        // La recarga de inventario se hará en Start() del InventoryManager 
        // al cargar la escena, usando GameData.SavedInventory.

        // Solo necesitamos asegurarnos de que la UI se actualice después de la recarga
        // aunque OnSceneLoaded ya debería encargarse de esto.

        // Reanudar tiempo
        Time.timeScale = 1f;

        // Recargar la escena actual
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);

        // *** ELIMINAR EL LISTENER SceneManager.sceneLoaded += ... QUE LIMPIABA INVENTARIO ***
        // Si tenías este código:
        /*
        SceneManager.sceneLoaded += (scene, mode) =>
        {
             if (InventoryManager.Instance != null)
             {
                 InventoryManager.Instance.ClearInventory(); // ¡ELIMINAR ESTO!
             }
        };
        */
    }


    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        // --- NUEVO: 2. Resetear los datos de persistencia al ir al menú principal. ---
        GameData.ResetToDefaults();

        // NOTA: El inventario actual debe limpiarse antes de cargar la escena del menú,
        // o el menú mostrará los ítems anteriores.
        InventoryManager.Instance?.ClearInventory();

        SceneManager.LoadScene("Menu");
    }

    // (Opcional) método para avanzar al siguiente nivel por build index
    public void LoadNextLevel()
    {
        if (PlayerStats.Instance != null && InventoryManager.Instance != null)
        {
            // Se llama al método para guardar el estado del jugador
            GameData.StoreCurrentPlayerData(PlayerStats.Instance, InventoryManager.Instance);
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
            transitionAnim.SetTrigger("Start");
        }
        else
        {
            Debug.Log("No hay más niveles en Build Settings.");
        }
    }
}
