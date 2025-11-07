using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
        else
        {
            Debug.LogWarning("GameOverPanel no asignado en UIManager");
        }
    }

    public void RetryLevel()
    {
        GameManager.Instance.RetryLevel();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
