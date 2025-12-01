using System.Collections;
using UnityEngine;

public class SoundtrackController : MonoBehaviour
{
    [Header("Soundtrack Configuration")]
    [SerializeField] private AudioClip[] soundtracks; // Array de soundtracks disponibles
    [SerializeField] private int initialSoundtrackIndex = 0; // Índice inicial
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private bool loop = true;
    [SerializeField] private float fadeTime = 1f; // Tiempo de transición entre canciones

    [Header("Combat Music Settings")]
    [SerializeField] private int explorationMusicIndex = 0;
    [SerializeField] private int combatMusicIndex = 1;

    private AudioSource audioSource;
    private int currentIndex;
    private int enemiesAware = 0; // Contador de enemigos que detectaron al jugador
    private bool inCombat = false;
    private static SoundtrackController instance;

    // Singleton para acceso global
    public static SoundtrackController Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = loop;
        audioSource.volume = volume;

        StartCoroutine(PreloadAllTracks());
    }

    private IEnumerator PreloadAllTracks()
    {
        foreach (var clip in soundtracks)
        {
            if (clip == null) continue;

            // Si no está cargado, pídele a Unity que cargue los datos de audio
            if (!clip.preloadAudioData && clip.loadState == AudioDataLoadState.Unloaded)
            {
                clip.LoadAudioData();
                // Espera hasta que termine de cargar para no bloquear todo de golpe
                while (clip.loadState == AudioDataLoadState.Loading)
                    yield return null;
            }
        }
    }

    #region Combat Music Management


    /// Llamado cuando un enemigo detecta al jugador

    public void RegisterEnemyAwareness()
    {
        enemiesAware++;

        if (!inCombat && enemiesAware > 0)
        {
            EnterCombat();
        }
    }

    
    /// Llamado cuando un enemigo pierde al jugador
    
    public void UnregisterEnemyAwareness()
    {
        enemiesAware--;

        if (enemiesAware < 0) enemiesAware = 0; // Seguridad

        if (inCombat && enemiesAware == 0)
        {
            ExitCombat();
        }
    }

    private void EnterCombat()
    {
        inCombat = true;
        Debug.Log("¡Entrando en combate!");
        ChangeSoundtrack(combatMusicIndex);
    }

    private void ExitCombat()
    {
        inCombat = false;
        Debug.Log("Saliendo de combate");
        ChangeSoundtrack(explorationMusicIndex);
    }

    public int GetEnemiesAware()
    {
        return enemiesAware;
    }

    public bool IsInCombat()
    {
        return inCombat;
    }

    #endregion

    #region Soundtrack Control

    
    /// Cambia al soundtrack especificado por índice
    
    public void ChangeSoundtrack(int index)
    {
        if (index < 0 || index >= soundtracks.Length)
        {
            Debug.LogError($"Índice {index} fuera de rango. Hay {soundtracks.Length} soundtracks disponibles.");
            return;
        }

        if (index == currentIndex && audioSource.isPlaying)
        {
            Debug.Log("Ya está reproduciendo este soundtrack.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeToNewTrack(index));
    }

    
    /// Reproduce directamente sin fade
    
    public void PlaySoundtrack(int index)
    {
        if (index < 0 || index >= soundtracks.Length)
        {
            Debug.LogError($"Índice {index} fuera de rango.");
            return;
        }

        currentIndex = index;
        audioSource.clip = soundtracks[index];
        audioSource.Play();
        Debug.Log($"Reproduciendo: {soundtracks[index].name}");
    }

    
    /// Transición suave entre soundtracks
    
    private System.Collections.IEnumerator FadeToNewTrack(int newIndex)
    {
        float startVolume = audioSource.volume;

        // Fade out
        float elapsed = 0f;
        while (elapsed < fadeTime / 2)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (fadeTime / 2));
            yield return null;
        }

        // Cambiar canción
        audioSource.Stop();
        currentIndex = newIndex;
        audioSource.clip = soundtracks[newIndex];
        audioSource.Play();

        // Fade in
        elapsed = 0f;
        while (elapsed < fadeTime / 2)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, elapsed / (fadeTime / 2));
            yield return null;
        }

        audioSource.volume = volume;
        Debug.Log($"Cambiado a: {soundtracks[newIndex].name}");
    }

    
    /// Pausa la música
    
    public void Pause()
    {
        audioSource.Pause();
    }

    
    /// Resume la música
    
    public void Resume()
    {
        audioSource.UnPause();
    }

    
    /// Detiene la música
    
    public void Stop()
    {
        audioSource.Stop();
    }

    
    /// Cambia el volumen
    
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }

    
    /// Obtiene el índice actual
    
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    /// Obtiene el total de soundtracks disponibles
    public int GetTotalSoundtracks()
    {
        return soundtracks.Length;
    }

    #endregion
}