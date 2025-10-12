using UnityEngine;

public class WeaponDisplay : MonoBehaviour
{
    [Header("Weapon Holder")]
    public SpriteRenderer weaponRenderer; // El SpriteRenderer de CurrentWeapon

    [Header("Weapon Sprites")]
    public Sprite knifeSprite;
    public Sprite gunSprite;
    public Sprite grenadeSprite;
    public Sprite shotgunSprite;
    public Sprite cokeSprite; // Si quieres mostrar la Coke también

    void Update()
    {
        UpdateWeaponDisplay();
    }

    private void UpdateWeaponDisplay()
    {
        if (weaponRenderer == null)
        {
            Debug.LogWarning("Weapon Renderer no está asignado!");
            return;
        }

        // Obtener el item seleccionado del inventario
        ItemData selectedItem = InventoryManager.Instance?.GetSelectedItem();

        if (selectedItem == null)
        {
            // No hay item seleccionado - ocultar arma
            weaponRenderer.sprite = null;
            weaponRenderer.enabled = false;
            return;
        }

        // Mostrar el sprite del item seleccionado
        weaponRenderer.enabled = true;

        // Asignar el sprite según el nombre del item
        switch (selectedItem.itemName)
        {
            case "Knife":
                weaponRenderer.sprite = knifeSprite;
                break;
            case "Gun":
                weaponRenderer.sprite = gunSprite;
                break;
            case "Grenade":
                weaponRenderer.sprite = grenadeSprite;
                break;
            case "Shotgun":
                weaponRenderer.sprite = shotgunSprite;
                break;
            case "Coke":
                weaponRenderer.sprite = cokeSprite;
                break;
            default:
                // Si no coincide, usar el sprite del ItemData
                weaponRenderer.sprite = selectedItem.itemIcon;
                break;
        }
    }

    // Método para voltear el arma según la dirección del player
    public void FlipWeapon(bool facingLeft)
    {
        if (weaponRenderer != null)
        {
            weaponRenderer.flipX = facingLeft;
        }
    }
}