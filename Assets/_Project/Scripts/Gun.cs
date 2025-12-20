using UnityEngine;
using System.Collections;
using UnityEngine.UI; // <-- NEW: Required for UI Text

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
    public Text ammoDisplay; // <-- NEW: Drag your UI Text here in Inspector
    public float kickbackAmount = 0.1f;
    public float returnSpeed = 5f;

    [Header("Impact Effects")]
    public GameObject impactEffectPrefab;


    private Vector3 gunOriginalPos;

    void Awake()
    {
        currentAmmo = maxAmmo;
        gunOriginalPos = transform.localPosition;
    }

    void Start()
    {
        UpdateAmmoUI(); // Set the initial ammo text
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
        if (currentAmmo <= 0) return;

        currentAmmo--;
        UpdateAmmoUI(); // <-- NEW: Update UI after every shot

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        transform.localPosition -= Vector3.forward * kickbackAmount;

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
            GameObject impactGO = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        // <-- NEW: Visual feedback for reloading
        if (ammoDisplay != null) ammoDisplay.text = "RELOADING...";

        transform.localPosition += Vector3.down * 0.2f;

        yield return new WaitForSeconds(reloadTime);

        transform.localPosition = gunOriginalPos;
        currentAmmo = maxAmmo;
        isReloading = false;

        UpdateAmmoUI(); // <-- NEW: Reset UI after reload
    }

    // Helper function to keep UI updated
    void UpdateAmmoUI()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = currentAmmo + " / " + maxAmmo;
        }
    }
}