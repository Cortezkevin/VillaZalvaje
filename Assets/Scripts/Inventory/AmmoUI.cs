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

        // Si no hay arma equipada o no es un arma de fuego
        if (weaponDisplay.selectedItem == null || weaponDisplay.selectedItem.itemName != "Gun")
        {
            ammoText.text = ""; // Oculta el texto
            return;
        }

        // Mostrar el contador de balas o mensaje de recarga
        if (weaponDisplay.currentAmmo <= 0)
        {
            ammoText.text = "Press R";
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.text = $"{weaponDisplay.currentAmmo} / {weaponDisplay.maxAmmo}";
            ammoText.color = Color.white;
        }
    }
}

