
using System.Collections;
using UnityEngine;

public class CeilingCrusher : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float dropDistance = 4f;
    [SerializeField, Min(0.01f)] private float dropDuration = 0.3f;
    [SerializeField, Min(0.01f)] private float retractDuration = 1.5f;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float initialDelay = 0f;
    [SerializeField, Min(0f)] private float topWaitDuration = 2f;
    [SerializeField, Min(0f)] private float bottomWaitDuration = 0.5f;

    [Header("Options")]
    [SerializeField] private bool startAutomatically = true;
    [SerializeField] private bool repeat = true;

    [Header("Damage")]
    [SerializeField] private CrusherDamageBox damageBox;

    private Vector3 startingPosition;
    private Vector3 loweredPosition;
    private Coroutine movementCoroutine;

    private void Awake()
    {
        startingPosition = transform.localPosition;
        loweredPosition = startingPosition + Vector3.down * dropDistance;

        if (damageBox == null)
        {
            damageBox = GetComponentInChildren<CrusherDamageBox>();
        }

        if (damageBox != null)
        {
            damageBox.SetDamageActive(false);
        }
    }

    private void Start()
    {
        if (startAutomatically)
        {
            StartCrusher();
        }
    }

    public void StartCrusher()
    {
        if (movementCoroutine != null)
        {
            return;
        }

        movementCoroutine = StartCoroutine(CrusherRoutine());
    }

    public void StopCrusher()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        if (damageBox != null)
        {
            damageBox.SetDamageActive(false);
        }
    }

    public void ResetCrusher()
    {
        StopCrusher();
        transform.localPosition = startingPosition;

        if (damageBox != null)
        {
            damageBox.ResetHit();
        }
    }

    private IEnumerator CrusherRoutine()
    {
        if (initialDelay > 0f)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        do
        {
            if (topWaitDuration > 0f)
            {
                yield return new WaitForSeconds(topWaitDuration);
            }

            if (damageBox != null)
            {
                damageBox.ResetHit();
                damageBox.SetDamageActive(true);
            }

            yield return MoveTo(
                loweredPosition,
                dropDuration
            );

            if (damageBox != null)
            {
                damageBox.SetDamageActive(false);
            }

            if (bottomWaitDuration > 0f)
            {
                yield return new WaitForSeconds(bottomWaitDuration);
            }

            yield return MoveTo(
                startingPosition,
                retractDuration
            );
        }
        while (repeat);

        movementCoroutine = null;
    }

    private IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            transform.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                progress
            );

            yield return null;
        }

        transform.localPosition = targetPosition;
    }
}
