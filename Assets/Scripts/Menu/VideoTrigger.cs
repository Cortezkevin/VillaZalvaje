using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoTrigger : MonoBehaviour
{
    [Header("Configuración de Video y Escena")]
    [Tooltip("El nombre exacto de la escena de inicio (MainMenu, StartScreen, etc.)")]
    private RawImage videoScreen;
    public string mainMenuSceneName = "Menu";

    private VideoPlayer videoPlayer;
    private bool hasTriggered = false;

    void Start()
    {
        // 🟢 OCULTAR la pantalla de video al inicio
        if (videoScreen != null)
        {
            videoScreen.enabled = false;
        }
    }

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("Error: VideoPlayer no se encontró en este objeto.");
            enabled = false;
        }

        // 🟢 BUSCAR DINÁMICAMENTE el RawImage persistente
        videoScreen = FindObjectOfType<RawImage>(true); // El 'true' busca también objetos inactivos

        if (videoScreen == null)
        {
            Debug.LogWarning("Advertencia: No se encontró la RawImage 'videoScreen' en la escena.");
        }

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // 🛡️ Detección de entrada del jugador en el área de trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("Jugador detectado. Iniciando video.");

            // 1. Mostrar la pantalla de video
            if (videoScreen != null)
            {
                videoScreen.enabled = true;
            }

            Time.timeScale = 0f;
            videoPlayer.Play();
        }
    }

    // 🎥 Evento que se dispara al finalizar el video
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video terminado. Redirigiendo al menú principal...");

        // 🟢 OCULTAR la pantalla de video al finalizar
        if (videoScreen != null)
        {
            videoScreen.enabled = false;
        }

        Time.timeScale = 1f;
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("El nombre de la escena de inicio está vacío. ¡No se puede cargar!");
            return;
        }

        // Cargar la escena. Asegúrate de que la escena está en Build Settings.
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // 🧼 Limpieza al destruir el componente (buena práctica)
    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}