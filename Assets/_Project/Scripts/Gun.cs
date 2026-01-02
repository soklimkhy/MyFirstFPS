using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Combat Settings")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("Effects & Recoil")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public Text ammoDisplay;
    public float kickbackAmount = 0.1f;
    public float returnSpeed = 5f;

    [Header("Impact Effects")]
    public GameObject impactEffectPrefab;

    [Header("Audio Settings")]
    public AudioSource gunSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;

    private Vector3 gunOriginalPos;

    void Awake()
    {
        currentAmmo = maxAmmo;
        gunOriginalPos = transform.localPosition;

        // CS Student Tip: Initialize the AudioSource settings via code to be 100% sure
        if (gunSource != null)
        {
            gunSource.playOnAwake = false;
            gunSource.loop = false;
            gunSource.priority = 0; // Highest priority
        }
    }

    void Start()
    {
        UpdateAmmoUI();
    }

    void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {
        if (isReloading) return;

        if (currentAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, gunOriginalPos, Time.deltaTime * returnSpeed);
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            if (gunSource != null && emptySound != null)
                gunSource.PlayOneShot(emptySound);
            return;
        }

        Debug.Log("Firing Bullet #" + currentAmmo);

        // --- ENHANCED AUDIO TRIGGER ---
        if (gunSource != null && shootSound != null)
        {
            // Randomize pitch slightly to prevent "Machine Gun Fatigue" (ear tiredness)
            gunSource.pitch = Random.Range(0.95f, 1.05f);

            // Using PlayOneShot allows sounds to OVERLAP instead of cutting each other off
            gunSource.PlayOneShot(shootSound, 1.0f);
        }

        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        transform.localPosition -= Vector3.forward * kickbackAmount;

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name); // Check the console!

            if (impactEffectPrefab != null)
            {
                GameObject impactGO = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }

            // --- THE FIX IS HERE ---
            // Changed from GetComponent to GetComponentInParent
            EnemyHealth enemy = hit.transform.GetComponentInParent<EnemyHealth>();
            // -----------------------

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (gunSource != null && reloadSound != null)
        {
            gunSource.pitch = 1.0f;
            gunSource.PlayOneShot(reloadSound);
        }

        if (ammoDisplay != null) ammoDisplay.text = "RELOADING...";
        transform.localPosition += Vector3.down * 0.2f;

        yield return new WaitForSeconds(reloadTime);

        transform.localPosition = gunOriginalPos;
        currentAmmo = maxAmmo;
        isReloading = false;

        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = currentAmmo + " / " + maxAmmo;
        }
    }
}