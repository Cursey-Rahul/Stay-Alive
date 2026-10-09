
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldHealthBar : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Entity target;

    [Header("Masks")]
    [SerializeField] private RectMask2D healthMask;
    [SerializeField] private RectMask2D prefillMask;

    [Header("Settings")]
    [SerializeField] private float emptyPadding = 990f;
    [SerializeField] private float damageDelay = 0.3f;
    [SerializeField] private float animationSpeed = 500f;
    [SerializeField] private Vector3 offset = new Vector3(-1f, 3f, 0f);

    private float previousHealthPercent = 1f;
    private float currentPadding;
    private float prefillPadding;
    private Coroutine damageCoroutine;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("HealthBar: Target is not assigned!", this);
            enabled = false;
            return;
        }

        previousHealthPercent = GetHealthPercent();

        currentPadding = emptyPadding * (1f - previousHealthPercent);
        prefillPadding = currentPadding;

        SetPadding(healthMask, currentPadding);
        SetPadding(prefillMask, prefillPadding);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }


        transform.position = target.transform.position + offset;

        float healthPercent = GetHealthPercent();
        float newPadding = emptyPadding * (1f - healthPercent);

        if (!Mathf.Approximately(healthPercent, previousHealthPercent))
        {
            bool tookDamage = healthPercent < previousHealthPercent;
            previousHealthPercent = healthPercent;

            currentPadding = newPadding;

            if (tookDamage)
            {
                if (damageCoroutine != null)
                    StopCoroutine(damageCoroutine);

                damageCoroutine = StartCoroutine(AnimatePrefill());
            }
            else
            {
                if (damageCoroutine != null)
                    StopCoroutine(damageCoroutine);

                prefillPadding = newPadding;
                SetPadding(prefillMask, prefillPadding);
            }
        }

        SetPadding(healthMask, currentPadding);
    }

    private float GetHealthPercent()
    {
        if (target.maxHealth <= 0)
            return 0f;

        return Mathf.Clamp01(
            (float)target.currentHealth / target.maxHealth
        );
    }

    private IEnumerator AnimatePrefill()
    {
        yield return new WaitForSeconds(damageDelay);

        while (Mathf.Abs(prefillPadding - currentPadding) > 0.1f)
        {
            prefillPadding = Mathf.MoveTowards(
                prefillPadding,
                currentPadding,
                animationSpeed * Time.deltaTime
            );

            SetPadding(prefillMask, prefillPadding);

            yield return null;
        }

        prefillPadding = currentPadding;
        SetPadding(prefillMask, prefillPadding);
        damageCoroutine = null;
    }


    private void SetPadding(RectMask2D mask, float rightPadding)
    {
        if (mask == null)
            return;

        Vector4 padding = mask.padding;
        padding.z = rightPadding; // Right padding
        mask.padding = padding;
    }
    public void SetTarget(Entity character)
    {
        target = character;
    }
}