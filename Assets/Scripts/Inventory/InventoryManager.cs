using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    public Image[] itemSlots;
    public Sprite emptySlotSprite;

    [Header("Selection Indicator")]
    public GameObject selectionIndicator;

    private List<ItemData> inventory = new List<ItemData>();
    private int maxSlots = 2;
    private int selectedSlot = -1;

    void Awake()
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

    void Start()
    {
        UpdateUI();
        UpdateSelection();
    }

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            SwitchSlot();
        }
    }

    public bool AddItem(ItemData item)
    {
        if (inventory.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno! (2/2 slots ocupados)");
            return false;
        }

        inventory.Add(item);

        if (inventory.Count == 1)
        {
            selectedSlot = 0;
            Debug.Log("Primer item recogido - Slot 0 seleccionado");
        }

        UpdateUI();
        UpdateSelection();
        Debug.Log("Item agregado: " + item.itemName + " (" + inventory.Count + "/2)");
        return true;
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < inventory.Count)
        {
            inventory.RemoveAt(slotIndex);

            if (inventory.Count == 0)
            {
                selectedSlot = -1;
            }
            else if (selectedSlot == slotIndex && inventory.Count > 0)
            {
                selectedSlot = 0;
            }

            UpdateUI();
            UpdateSelection();
        }
    }

    private void SwitchSlot()
    {
        if (inventory.Count == 0)
        {
            Debug.Log("No hay items para seleccionar");
            return;
        }

        if (inventory.Count == 1)
        {
            selectedSlot = 0;
            Debug.Log("Solo hay 1 item - Slot 0 sigue seleccionado");
            UpdateSelection();
            return;
        }

        selectedSlot = (selectedSlot == 0) ? 1 : 0;
        Debug.Log("Cambiado a Slot " + selectedSlot);
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        if (selectionIndicator == null)
        {
            Debug.LogWarning("Selection Indicator no est� asignado!");
            return;
        }

        // Si no hay items, ocultar indicador
        if (selectedSlot < 0 || selectedSlot >= inventory.Count)
        {
            selectionIndicator.SetActive(false);
            return;
        }

        // Verificar que el slot seleccionado existe
        if (selectedSlot >= itemSlots.Length || itemSlots[selectedSlot] == null)
        {
            Debug.LogError("Slot " + selectedSlot + " no existe!");
            return;
        }

        // Mostrar el indicador
        selectionIndicator.SetActive(true);

        // Obtener la posici�n del slot seleccionado autom�ticamente
        RectTransform slotRect = itemSlots[selectedSlot].GetComponent<RectTransform>();
        RectTransform indicatorRect = selectionIndicator.GetComponent<RectTransform>();

        // Copiar la posici�n del slot al indicador
        indicatorRect.anchoredPosition = slotRect.anchoredPosition;

        Debug.Log("Indicador movido a posici�n: " + indicatorRect.anchoredPosition);
    }

    private void UpdateUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                Debug.LogError("itemSlots[" + i + "] es NULL!");
                continue;
            }

            if (i < inventory.Count && inventory[i] != null)
            {
                itemSlots[i].sprite = inventory[i].itemIcon;
                itemSlots[i].enabled = true;
                itemSlots[i].color = Color.white;
            }
            else
            {
                if (emptySlotSprite != null)
                {
                    itemSlots[i].sprite = emptySlotSprite;
                }
                itemSlots[i].enabled = true;
                itemSlots[i].color = Color.white;
            }
        }
    }

    public ItemData GetSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < inventory.Count)
        {
            return inventory[selectedSlot];
        }
        return null;
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }
}