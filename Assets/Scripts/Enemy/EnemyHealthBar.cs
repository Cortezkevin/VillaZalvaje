using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBarFill; // La barra que se llena

    [Header("Colors")]
    public Color highHealthColor = new Color(0.4f, 0.7f, 0.4f);   // Verde
    public Color midHealthColor = new Color(0.8f, 0.7f, 0.3f);    // Amarillo
    public Color lowHealthColor = new Color(0.8f, 0.3f, 0.3f);    // Rojo

    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 1.5f, 0f); // Offset sobre el enemigo
    public bool hideWhenFull = false; // Ocultar barra cuando vida está al 100%
    public bool hideWhenDead = true;  // Ocultar barra cuando muere

    private EnemyStats enemyStats;
    private Transform enemyTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Agregar CanvasGroup si no existe (para controlar visibilidad)
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Buscar automáticamente la barra si no está asignada
        if (healthBarFill == null)
        {
            Transform healthBarTransform = transform.Find("HealthBar");
            if (healthBarTransform != null)
            {
                healthBarFill = healthBarTransform.GetComponent<Image>();
                Debug.Log("HealthBar encontrada automáticamente en " + gameObject.name);
            }
        }
    }

    void Start()
    {
        // Obtener el EnemyStats del padre
        enemyStats = GetComponentInParent<EnemyStats>();

        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats no encontrado en el padre de " + gameObject.name);
            enabled = false;
            return;
        }

        enemyTransform = enemyStats.transform;

        // Suscribirse a eventos
        enemyStats.OnHealthChanged += UpdateHealthBar;
        enemyStats.OnEnemyDeath += OnEnemyDied;

        // Actualizar UI inicial
        UpdateHealthBar(enemyStats.GetCurrentHealth(), enemyStats.GetMaxHealth());
    }

    void LateUpdate()
    {
        // Mantener la barra sobre el enemigo
        if (enemyTransform != null)
        {
            transform.position = enemyTransform.position + offset;

            // Hacer que la barra siempre mire a la cámara
            if (Camera.main != null)
            {
                transform.rotation = Camera.main.transform.rotation;
            }
        }
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (enemyStats != null)
        {
            enemyStats.OnHealthChanged -= UpdateHealthBar;
            enemyStats.OnEnemyDeath -= OnEnemyDied;
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarFill == null)
        {
            Debug.LogWarning("healthBarFill no está asignado en " + gameObject.name);
            return;
        }

        float healthPercentage = (float)currentHealth / maxHealth;
        healthBarFill.fillAmount = healthPercentage;

        // Cambiar color según porcentaje
        if (healthPercentage > 0.5f)
        {
            healthBarFill.color = Color.Lerp(midHealthColor, highHealthColor, (healthPercentage - 0.5f) * 2f);
        }
        else if (healthPercentage > 0.25f)
        {
            healthBarFill.color = Color.Lerp(lowHealthColor, midHealthColor, (healthPercentage - 0.25f) * 4f);
        }
        else
        {
            healthBarFill.color = lowHealthColor;
        }

        // Ocultar/mostrar según configuración
        if (hideWhenFull && healthPercentage >= 1.0f)
        {
            SetVisible(false);
        }
        else
        {
            SetVisible(true);
        }
    }

    private void OnEnemyDied()
    {
        if (hideWhenDead)
        {
            SetVisible(false);
        }
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
        }
        else if (canvas != null)
        {
            canvas.enabled = visible;
        }
    }
}