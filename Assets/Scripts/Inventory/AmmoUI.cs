using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [Header("Referencias")]
    public WeaponDisplay weaponDisplay;
    public TextMeshProUGUI ammoText;

    void Start()
    {
        if (weaponDisplay == null)
            weaponDisplay = FindAnyObjectByType<WeaponDisplay>();

        UpdateAmmoUI();
    }


    public void UpdateAmmoUI()
    {
        if (weaponDisplay == null || ammoText == null) return;

        bool isFirearm = weaponDisplay.selectedItem != null &&
                         (weaponDisplay.selectedItem.itemName == "Gun" ||
                          weaponDisplay.selectedItem.itemName == "Shotgun"); // 🟢 AÑADIDO: Soporte para Shotgun

        // Si no hay arma equipada o no es un arma de fuego
        if (!isFirearm)
        {
            ammoText.text = ""; // Oculta el texto
            return;
        }

        // Mostrar el contador de balas o mensaje de recarga
        // **Nota:** Los valores de currentAmmo/maxAmmo ya son sincronizados por WeaponDisplay.UpdateWeaponDisplay()
        if (weaponDisplay.displayCurrentAmmo <= 0)
        {
            ammoText.text = "Press R";
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.text = $"{weaponDisplay.displayCurrentAmmo} / {weaponDisplay.displayMaxAmmo}";
            ammoText.color = Color.white;
        }
    }
}