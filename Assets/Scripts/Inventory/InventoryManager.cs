using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    public Image[] itemSlots; // Array de las 5 imágenes de slots
    public Sprite emptySlotSprite; // Sprite cuando el slot está vacío

    private List<ItemData> inventory = new List<ItemData>();
    private int maxSlots = 5;

    void Awake()
    {
        // Singleton pattern - solo una instancia
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    public bool AddItem(ItemData item)
    {
        // Verificar si hay espacio
        if (inventory.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno!");
            return false;
        }

        // Agregar item al inventario
        inventory.Add(item);
        UpdateUI();
        Debug.Log("Item agregado: " + item.itemName);
        return true;
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < inventory.Count)
        {
            inventory.RemoveAt(slotIndex);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // Actualizar la UI de todos los slots
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.Count && inventory[i] != null)
            {
                itemSlots[i].sprite = inventory[i].itemIcon;
                itemSlots[i].enabled = true;
            }
            else
            {
                itemSlots[i].sprite = emptySlotSprite;
                itemSlots[i].enabled = true;
            }
        }
    }
}