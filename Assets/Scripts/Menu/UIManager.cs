using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // Necesario para Coroutines

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    // Ya no necesitamos 'public Image gameOverPane;' si gameOverPanel tiene el Image.
    // Lo usaremos directamente desde gameOverPanel.
    public Button retryButton;
    public Button goToMainMenu;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f; // Duración de la animación de fade en segundos
    public byte maxAlpha = 200;       // Valor máximo de opacidad (0-255). Por ejemplo, 200 para que sea semitransparente.

    private Image panelImage; // Referencia privada al componente Image del gameOverPanel

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

    void Start()
    {
        // Obtener la referencia al componente Image del gameOverPanel
        if (gameOverPanel != null)
        {
            panelImage = gameOverPanel.GetComponent<Image>();
            if (panelImage == null)
            {
                Debug.LogWarning("GameOverPanel no tiene un componente Image. La animación de fade no funcionará.");
            }

            // Inicialmente, el panel debe estar inactivo y con alpha en 0
            Color startColor = panelImage.color;
            startColor.a = 0;
            panelImage.color = startColor;
            gameOverPanel.SetActive(false); // Asegurarse de que esté oculto al inicio
        }
    }

    // ------------------------------------------------------------------

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null && panelImage != null)
        {
            gameOverPanel.SetActive(true); // Activa el GameObject padre
            StartCoroutine(FadePanel(0, maxAlpha, fadeDuration, true)); // Inicia el fade in
            Time.timeScale = 0f;
        }
        else if (gameOverPanel != null)
        {
            // Si no hay Image pero el panel está, simplemente lo activa
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("GameOverPanel no asignado en UIManager");
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null && panelImage != null)
        {
            StartCoroutine(FadePanel(maxAlpha, 0, fadeDuration, false)); // Inicia el fade out
            // Time.timeScale se reanuda en GameManager.RetryLevel o GoToMainMenu
        }
        else if (gameOverPanel != null)
        {
            // Si no hay Image pero el panel está, simplemente lo desactiva
            gameOverPanel.SetActive(false);
        }
    }

    // ------------------------------------------------------------------
    // COROUTINE PARA LA ANIMACIÓN DE FADE
    // ------------------------------------------------------------------
    private IEnumerator FadePanel(byte startAlpha, byte endAlpha, float duration, bool activateAfterFade)
    {
        float elapsedTime = 0f;
        Color currentColor = panelImage.color;
        byte currentAlpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Usar Time.unscaledDeltaTime porque Time.timeScale puede ser 0
            currentAlpha = (byte)Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            currentColor.a = currentAlpha / 255f; // Normalizar a 0-1 para Color
            panelImage.color = currentColor;
            yield return null; // Esperar al siguiente frame
        }

        // Asegurarse de que el alpha final sea exacto
        currentColor.a = endAlpha / 255f;
        panelImage.color = currentColor;

        // Desactivar el panel si el fade es hacia la invisibilidad
        if (!activateAfterFade)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // ------------------------------------------------------------------

    public void RetryLevel()
    {
        GameManager.Instance.RetryLevel();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }
}