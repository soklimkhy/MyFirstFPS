using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectedWeapon = 0;
    private WeaponMovement moveScript;

    [Header("Weapon Settings")]
    public float gunSway = 2f;
    public float knifeSway = 4f; // Knife moves faster/more loosely

    void Start()
    {
        moveScript = GetComponent<WeaponMovement>();
        SelectWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedWeapon = (selectedWeapon == 0) ? 1 : 0;
            SelectWeapon();
            UpdateMovementSettings();
        }
    }

    void UpdateMovementSettings()
    {
        if (moveScript == null) return;

        // Change the sway multiplier based on what we are holding
        moveScript.swayMultiplier = (selectedWeapon == 0) ? gunSway : knifeSway;
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(i == selectedWeapon);
            i++;
        }
    }
}