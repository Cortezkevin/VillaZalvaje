using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData; // El ScriptableObject del item

    [Header("Pickup Settings")]
    public KeyCode pickupKey = KeyCode.G;
    public float pickupRange = 2f;

    private Transform player;
    private bool canPickup = false;

    void Start()
    {
        // Buscar el player por tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calcular distancia al jugador
        float distance = Vector3.Distance(transform.position, player.position);
        canPickup = distance <= pickupRange;

        // Si está cerca y presiona G
        if (canPickup && Input.GetKeyDown(pickupKey))
        {
            PickupItem();
        }
    }

    void PickupItem()
    {
        if (itemData == null)
        {
            Debug.LogWarning("No hay ItemData asignado en " + gameObject.name);
            return;
        }

        // Intentar agregar al inventario
        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.AddItem(itemData))
            {
                Debug.Log("Recogiste: " + itemData.itemName);
                Destroy(gameObject); // Destruir el item del mundo
            }
        }
        else
        {
            Debug.LogError("No se encontró InventoryManager en la escena!");
        }
    }

    // Visualizar el rango en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}