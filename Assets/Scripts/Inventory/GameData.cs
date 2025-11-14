using System.Collections.Generic;
using UnityEngine;

// GameData.cs (No necesita heredar de MonoBehaviour)
public static class GameData
{
    // --- ESTADO INICIAL ---
    private const int DefaultHealth = 100;
    private const int DefaultAmmo = 30;
    private const int DefaultScore = 0;

    // --- DATOS ALMACENADOS ---
    public static int SavedHealth { get; private set; } = DefaultHealth;
    public static int SavedAmmo { get; private set; } = DefaultAmmo;
    public static int SavedScore { get; private set; } = DefaultScore;
    public static System.Collections.Generic.List<ItemData> SavedInventory { get; private set; }
        = new System.Collections.Generic.List<ItemData>();


    // --- MÉTODOS DE MANIPULACIÓN ---
    public static void StoreCurrentPlayerData(PlayerStats playerStats, InventoryManager inventoryManager)
    {
        // ... (Guardar estadísticas)
        SavedHealth = playerStats.GetCurrentHealth();
        SavedScore = playerStats.score;
        SavedAmmo = playerStats.currentAmmo;

        // **Esto requiere que el getter GetInventory() exista en InventoryManager.**
        SavedInventory = new System.Collections.Generic.List<ItemData>(inventoryManager.GetInventory());
    }

    /// <summary>
    /// Reinicia los datos al estado inicial (como si se cargara desde el menú).
    /// </summary>
    public static void ResetToDefaults()
    {
        SavedHealth = DefaultHealth;
        SavedAmmo = DefaultAmmo;
        SavedScore = DefaultScore;
        SavedInventory.Clear();
    }

}