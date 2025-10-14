using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [Header("Referencias")]
    public WeaponDisplay weaponDisplay; 
    public TextMeshProUGUI ammoText; 

    void Update()
    {
        if (weaponDisplay.selectedItem != null)
        {
            if (weaponDisplay.selectedItem.itemName.ToString() == "Gun")
            {
                if (weaponDisplay == null || ammoText == null) return;

                // Si no quedan balas, mostrar mensaje
                if (weaponDisplay.currentAmmo <= 0)
                {
                    ammoText.text = "Press R";
                    ammoText.color = Color.red;
                }
                else
                {
                    // Mostrar balas restantes
                    ammoText.text = $"{weaponDisplay.currentAmmo} / {weaponDisplay.maxAmmo}";
                    ammoText.color = Color.white;
                }
            }
        }
    }
}
