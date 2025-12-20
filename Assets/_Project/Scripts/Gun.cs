using UnityEngine;
using System.Collections; // Required for Coroutines

public class Gun : MonoBehaviour
{
    [Header("Combat Settings")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f; // How fast the gun fires
    private float nextTimeToFire = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("Effects & Recoil")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public float kickbackAmount = 0.1f;
    public float returnSpeed = 5f;

    private Vector3 gunOriginalPos;

    void Awake()
    {
        currentAmmo = maxAmmo;
        gunOriginalPos = transform.localPosition;
    }

    void OnEnable()
    {
        isReloading = false; // Reset status if gun is switched
    }

    void Update()
    {
        if (isReloading) return;

        // Auto-reload if empty, or manual reload with 'R'
        if (currentAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        // Shooting logic (Fire1 is usually Left-Click)
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        // Smoothly return gun to original position after recoil
        transform.localPosition = Vector3.Lerp(transform.localPosition, gunOriginalPos, Time.deltaTime * returnSpeed);
    }

    void Shoot()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;

        // Play muzzle flash if assigned
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Apply visual recoil (kickback)
        transform.localPosition -= Vector3.forward * kickbackAmount;

        RaycastHit hit;
        // Cast a ray from the center of the camera forward
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Visual feedback: Move gun down slightly during reload
        transform.localPosition += Vector3.down * 0.2f;

        yield return new WaitForSeconds(reloadTime);

        // Reset gun position and refill ammo
        transform.localPosition = gunOriginalPos;
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload Complete!");
    }
}