using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI healthText;

    [Header("Health Colors")]
    public Color fullHealthColor = new Color(0.4f, 0.7f, 0.4f);    // Verde cactus apagado
    public Color midHealthColor = new Color(0.8f, 0.7f, 0.3f);     // Amarillo apagado
    public Color lowHealthColor = new Color(0.8f, 0.3f, 0.3f);     // Rojo apagado

    void Start()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(PlayerStats.Instance.GetCurrentHealth(), PlayerStats.Instance.GetMaxHealth());
        }
        else
        {
            Debug.LogError("PlayerStats.Instance no encontrado!");
        }
    }

    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            // Mostrar solo el n�mero actual (sin "/100")
            healthText.text = currentHealth.ToString();

            // Cambiar color seg�n porcentaje de vida
            float healthPercentage = (float)currentHealth / maxHealth;

            if (healthPercentage == 1.0f) // Vida al 100%
            {
                healthText.color = fullHealthColor; // Verde cactus
            }
            else if (healthPercentage > 0.5f) // M�s del 50%
            {
                // Transici�n suave de verde a amarillo
                healthText.color = Color.Lerp(midHealthColor, fullHealthColor, (healthPercentage - 0.5f) * 2f);
            }
            else if (healthPercentage > 0.25f) // Entre 25% y 50%
            {
                // Transici�n suave de amarillo a rojo
                healthText.color = Color.Lerp(lowHealthColor, midHealthColor, (healthPercentage - 0.25f) * 4f);
            }
            else // Menos del 25%
            {
                healthText.color = lowHealthColor; // Rojo apagado
            }
        }
    }
}