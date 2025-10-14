using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData;

    [Header("Pickup Settings")]
    public float pickupRange = 2f;

    private Transform player;
    private bool canPickup = false;

    [Header("Visual Indicator")]
    public GameObject pickupIndicator;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player encontrado: " + player.name);
        }
        else
        {
            Debug.LogError("No se encontro ningun objeto con tag 'Player'!");
        }

        if (pickupIndicator != null)
        {
            pickupIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player es null en Update");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Verifica si la condición de recogida ha cambiado
        bool newCanPickup = distance <= pickupRange;

        if (newCanPickup != canPickup)
        {
            canPickup = newCanPickup;

            // Lógica para mostrar/ocultar el indicador
            if (pickupIndicator != null)
            {
                pickupIndicator.SetActive(canPickup);
            }
        }

        if (canPickup)
        {
            Debug.Log("Dentro del rango! Distancia: " + distance + " - Presiona G");

            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                Debug.Log("Tecla G presionada!");
                PickupItem();
            }
        }
    }

    void PickupItem()
    {
        if (itemData == null)
        {
            Debug.LogWarning("No hay ItemData asignado en " + gameObject.name);
            return;
        }

        Debug.Log("Intentando recoger: " + itemData.itemName);

        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.AddItem(itemData))
            {
                // Ocultar el indicador inmediatamente antes de destruir el objeto
                if (pickupIndicator != null)
                {
                    pickupIndicator.SetActive(false);
                }

                Debug.Log("Item recogido exitosamente: " + itemData.itemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("No se pudo agregar el item");
            }
        }
        else
        {
            Debug.LogError("No se encontro InventoryManager!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}