using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    // Asegúrate de que ItemData es una clase/ScriptableObject que ya tienes
    public ItemData itemData;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("ItemWorld requiere un SpriteRenderer.");
        }

        // Añadir una etiqueta (opcional pero bueno para la interacción)
        gameObject.tag = "ItemWorld";
    }

    // Método para configurar el ítem cuando es creado (al soltarlo)
    public void SetItemData(ItemData item)
    {
        itemData = item;
        if (spriteRenderer != null && itemData != null)
        {
            spriteRenderer.sprite = itemData.itemIcon; // Usamos el ícono del ItemData
            gameObject.name = "ItemWorld_" + itemData.itemName;
        }
    }

    // Método para que el script de interacción del jugador obtenga el ItemData
    public ItemData GetItemData()
    {
        return itemData;
    }
}