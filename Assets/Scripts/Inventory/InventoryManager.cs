using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    public Image[] itemSlots;
    public Sprite emptySlotSprite;

    [Header("Selection Indicator")]
    public GameObject selectionIndicator;

    [Header("Item Dropping")]
    public GameObject itemWorldPrefab;

    private List<ItemData> inventory = new List<ItemData>();
    private int maxSlots = 2;
    private int selectedSlot = -1;

    void Awake()
    {
        if (InventoryManager.Instance == null)
        {
            InventoryManager.Instance = this;
            DontDestroyOnLoad(gameObject);  

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // InventoryManager.cs

    void Start()
    {
        LoadLevelStartInventory(); // Llama al método que hace todo el trabajo.
    }


    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            SwitchSlot();
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            DropSelectedItem();
        }
    }
    public void DropSelectedItem()
    {
        // 1. Verificar si hay un slot seleccionado y si ese slot contiene un ítem
        if (selectedSlot < 0 || selectedSlot >= inventory.Count)
        {
            Debug.Log("No hay ítem seleccionado para soltar.");
            return;
        }

        ItemData itemToDrop = inventory[selectedSlot];

        // 2. Llamar a la lógica de soltar ítem en el mundo
        DropItem(itemToDrop);

        // 3. Eliminar el ítem del inventario (usa el método RemoveItem existente)
        RemoveItem(selectedSlot);

        Debug.Log($"Ítem {itemToDrop.itemName} soltado del Slot {selectedSlot}.");

        // RemoveItem ya llama a UpdateUI y UpdateSelection.
    }

    public void ClearInventory()
    {
        inventory.Clear();
        selectedSlot = -1;
        UpdateUI();
        UpdateSelection();
    }

    // InventoryManager.cs

    // ... (El resto del script)

    // 🟢 MÉTODO AJUSTADO: Suelta un ítem en el mundo
    public void DropItem(ItemData itemToDrop)
    {
        if (itemToDrop == null) return;

        GameObject player = GameObject.FindWithTag("Player");
        // ... (Chequeos y determinación de dropPosition existentes)

        if (itemWorldPrefab == null)
        {
            Debug.LogError("itemWorldPrefab NO ESTÁ ASIGNADO...");
            return;
        }

        // ... (Determinación de dropPosition)
        Vector3 dropDirection = Vector3.right;
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

        if (playerSprite != null && playerSprite.flipX)
        {
            dropDirection = Vector3.left;
        }

        Vector3 dropPosition = player.transform.position + dropDirection * 0.5f;

        GameObject droppedItem = Instantiate(itemWorldPrefab, dropPosition, Quaternion.identity);

        ItemPickup itemPickupScript = droppedItem.GetComponent<ItemPickup>();

        if (itemPickupScript != null)
        {
            // 1. Asignamos el ItemData al prefab recién creado.
            itemPickupScript.itemData = itemToDrop;

            // 2. BUSCAMOS Y ASIGNAMOS EL SPRITE DEL WEAPONDISPLAY
            SpriteRenderer droppedSprite = droppedItem.GetComponent<SpriteRenderer>();

            if (droppedSprite != null)
            {
                // 🚀 NUEVA LÓGICA: Obtener el sprite desde WeaponDisplay
                if (WeaponDisplay.Instance != null)
                {
                    droppedSprite.sprite = WeaponDisplay.Instance.GetItemSprite(itemToDrop);
                }
                else
                {
                    // Si WeaponDisplay no existe, usamos el ícono por defecto del ItemData
                    droppedSprite.sprite = itemToDrop.itemIcon;
                    Debug.LogWarning("WeaponDisplay.Instance no encontrado. Usando ItemData.itemIcon por defecto.");
                }

                droppedItem.name = "Dropped_" + itemToDrop.itemName;
            }
            // ... (Resto del log)
        }
        else
        {
            Debug.LogError($"El itemWorldPrefab no tiene el componente ItemPickup...");
        }
    }

    public bool AddItem(ItemData item)
    {
        // ⚠️ LÓGICA DE REEMPLAZO ⚠️
        if (inventory.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno! Activando lógica de reemplazo.");

            // 1. Obtener el ítem actual en el slot seleccionado
            if (selectedSlot < 0 || selectedSlot >= inventory.Count)
            {
                Debug.LogError("Selected slot inválido durante AddItem con inventario lleno.");
                return false;
            }

            ItemData itemToReplace = inventory[selectedSlot];

            // 2. Soltar el ítem viejo al suelo
            DropItem(itemToReplace);

            // 3. Reemplazar el ítem en el slot seleccionado
            inventory[selectedSlot] = item;

            Debug.Log($"Ítem {itemToReplace.itemName} reemplazado por {item.itemName} en Slot {selectedSlot}.");

            UpdateUI();
            UpdateSelection();
            return true; // Ítem añadido por reemplazo
        }
        // FIN LÓGICA DE REEMPLAZO

        // Lógica de añadir normal si el inventario NO está lleno
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
                // Asegurar que selectedSlot siempre es un índice válido después de la eliminación
                selectedSlot = Mathf.Clamp(selectedSlot, 0, inventory.Count - 1);
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar nuevamente los slots e indicador de selección en la nueva escena
        if (itemSlots == null || itemSlots.Length == 0)
        {
            itemSlots = GameObject.FindObjectsByType<Image>(FindObjectsSortMode.None)
                .Where(img => img.CompareTag("InventorySlot"))
                .ToArray();
        }

        if (selectionIndicator == null)
        {
            GameObject indicatorObj = GameObject.FindWithTag("SelectionIndicator");
            if (indicatorObj != null)
                selectionIndicator = indicatorObj;
        }

        // Actualizar UI para reflejar los ítems persistentes
        UpdateUI();
        UpdateSelection();

        AmmoDisplay ammoUI = Object.FindAnyObjectByType<AmmoDisplay>();
        if (ammoUI != null)
        {
            ammoUI.UpdateAmmoUI();
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

    // Dentro de InventoryManager.cs
    public System.Collections.Generic.List<ItemData> GetInventory()
    {
        return inventory;
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    public void LoadLevelStartInventory()
    {
        // 1. Limpiamos la lista actual persistente (DontDestroyOnLoad).
        inventory.Clear();
        selectedSlot = -1; // Aseguramos que la selección se reinicie.

        // 2. Cargamos el inventario con el que se debe empezar este nivel.
        if (GameData.LevelStartInventory != null)
        {
            foreach (var item in GameData.LevelStartInventory)
            {
                // Usamos AddItem para actualizar la lista interna y la selección.
                AddItem(item);
            }
        }

        // 3. Forzamos la actualización visual.
        UpdateUI();
        UpdateSelection();

        Debug.Log("Inventario restablecido al estado de inicio de nivel. Items: " + inventory.Count);
    }

}