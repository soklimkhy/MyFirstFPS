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

    // These will store your EXACT Inspector values automatically
    private Vector3 idlePos;
    private Quaternion idleRot;

    void Start()
    {
        // This is the "Magic" line: it remembers your 0.33, -0.04, 0.8 setup
        idlePos = transform.localPosition;
        idleRot = transform.localRotation;

        if (fpsCam == null) fpsCam = Camera.main;
    }

    void Update()
    {
        // Smoothly return to your custom Inspector position
        if (Time.time >= nextAttackTime - (attackRate * 0.4f))
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
        // Right Click: Heavy Attack
        else if (Input.GetButtonDown("Fire2"))
        {
            StopAllCoroutines();
            StartCoroutine(SlashAnimation(true));
            PerformRaycast(heavyDamage, heavyRange);
            nextAttackTime = Time.time + attackRate * 1.5f;
        }
    }

    IEnumerator SlashAnimation(bool isHeavy)
    {
        float t = 0;
        float duration = isHeavy ? 0.25f : 0.15f;

        // Moves RELATIVE to your custom position
        // We move it forward (Z) and to the left (X) to create a slash motion
        Vector3 peakPos = isHeavy ?
            idlePos + new Vector3(-0.6f, 0.2f, 0.4f) : // Heavy: Bigger movement
            idlePos + new Vector3(-0.4f, 0.1f, 0.2f);  // Light: Smaller movement

        // Rotates RELATIVE to your custom rotation
        Quaternion peakRot = isHeavy ?
            idleRot * Quaternion.Euler(60, -40, -30) :
            idleRot * Quaternion.Euler(20, -60, 20);

        while (t < 1)
        {
            t += Time.deltaTime * (1 / duration) * swingSpeed;
            transform.localPosition = Vector3.Lerp(idlePos, peakPos, t);
            transform.localRotation = Quaternion.Slerp(idleRot, peakRot, t);
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
    }

    void PerformRaycast(float dmg, float range)
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            // --- THE FIX IS HERE ---
            // Changed from GetComponent to GetComponentInParent
            EnemyHealth target = hit.transform.GetComponentInParent<EnemyHealth>();
            // -----------------------

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