using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    [Header("Attack Settings")]
    public float lightDamage = 50f;
    public float heavyDamage = 100f;
    public float lightRange = 4.5f;
    public float heavyRange = 5.5f;
    public float attackRate = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Visual Settings")]
    public float swingSpeed = 15f;
    public float returnSpeed = 8f;

    [Header("References")]
    public Camera fpsCam;
    public GameObject bloodEffectPrefab;

    // Your specific coordinates
    private Vector3 idlePos = new Vector3(0.33f, -0.04f, 0.8f);
    private Quaternion idleRot = Quaternion.Euler(0, 0, 1);

    void Start()
    {
        // Force the sword to your exact positions at start
        transform.localPosition = idlePos;
        transform.localRotation = idleRot;
        if (fpsCam == null) fpsCam = Camera.main;
    }

    void Update()
    {
        // Continuously smooth back to idle when not attacking
        if (Time.time >= nextAttackTime - (attackRate * 0.5f))
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, idlePos, Time.deltaTime * returnSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, idleRot, Time.deltaTime * returnSpeed);
        }

        if (Time.time < nextAttackTime) return;

        // Left Click: Fast Slash
        if (Input.GetButtonDown("Fire1"))
        {
            StopAllCoroutines();
            StartCoroutine(SlashAnimation(false));
            PerformRaycast(lightDamage, lightRange);
            nextAttackTime = Time.time + attackRate;
        }
        // Right Click: Power Stab/Heavy
        else if (Input.GetButtonDown("Fire2"))
        {
            StopAllCoroutines();
            StartCoroutine(SlashAnimation(true));
            PerformRaycast(heavyDamage, heavyRange);
            nextAttackTime = Time.time + attackRate * 1.8f;
        }
    }

    // This makes the sword follow a "curved" path rather than a straight line
    IEnumerator SlashAnimation(bool isHeavy)
    {
        float t = 0;
        float duration = isHeavy ? 0.3f : 0.15f;

        // Peak Position (Where the sword goes mid-swing)
        Vector3 peakPos = isHeavy ?
            idlePos + new Vector3(-0.5f, -0.2f, 0.5f) : // Heavy: Deep thrust forward
            idlePos + new Vector3(-0.4f, 0.2f, 0.2f);   // Light: Wide sideways arc

        // Peak Rotation
        Quaternion peakRot = isHeavy ?
            Quaternion.Euler(80, -40, 0) :  // Heavy: Pointing down-forward
            Quaternion.Euler(0, 90, 45);    // Light: Tilted horizontal

        // 1. SWING OUT
        while (t < 1)
        {
            t += Time.deltaTime * (1 / duration) * swingSpeed;
            transform.localPosition = Vector3.Lerp(idlePos, peakPos, t);
            transform.localRotation = Quaternion.Slerp(idleRot, peakRot, t);
            yield return null;
        }

        // 2. SHORT PAUSE (Impact feel)
        yield return new WaitForSeconds(0.05f);

        // (The Update() function will handle Lerping back to idlePos automatically)
    }

    void PerformRaycast(float dmg, float range)
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
            if (target != null)
            {
                target.TakeDamage(dmg);
                if (bloodEffectPrefab != null)
                {
                    Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
    }
}