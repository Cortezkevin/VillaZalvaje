// GameData.cs
using System.Collections.Generic;
using UnityEngine; // Puede que necesites UnityEngine o no, si solo usas tipos de datos básicos.

// ¡CRÍTICO!: Debe ser 'public static class'
public static class GameData
{
    // --- DATOS INICIALES POR DEFECTO ---
    private const int DefaultHealth = 100;
    private const int DefaultAmmo = 30;
    private const int DefaultScore = 0;

    // --- DATOS ALMACENADOS PARA EL INICIO DE CADA NIVEL (LevelStartData) ---
    // ¡CRÍTICO!: Estas propiedades deben ser 'public static' para ser accedidas.
    public static int LevelStartHealth { get; private set; } = DefaultHealth;
    public static int LevelStartScore { get; private set; } = DefaultScore;
    public static int LevelStartAmmo { get; private set; } = DefaultAmmo;
    
    // Lista para el inventario. La inicializamos vacía.
    public static List<ItemData> LevelStartInventory { get; private set; }
        = new List<ItemData>();


    // --- MÉTODOS DE MANIPULACIÓN ---

    public static void StoreLevelStartData(PlayerStats playerStats, InventoryManager inventoryManager)
    {
        LevelStartHealth = playerStats.GetCurrentHealth();
        LevelStartScore = playerStats.score;
        LevelStartAmmo = playerStats.currentAmmo; 

        // CRÍTICO: Se debe CLONAR la lista del inventario para persistir los datos.
        LevelStartInventory = new List<ItemData>(inventoryManager.GetInventory());
    }

    public static void ResetToDefaults()
    {
        LevelStartHealth = DefaultHealth;
        LevelStartScore = DefaultScore;
        LevelStartAmmo = DefaultAmmo;
        LevelStartInventory.Clear();
    }
}