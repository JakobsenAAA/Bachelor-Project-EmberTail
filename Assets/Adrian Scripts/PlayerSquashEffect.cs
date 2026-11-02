
using System.Collections;
using UnityEngine;

public class PlayerSquashEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualModel;

    [Header("Squash")]
    [SerializeField, Range(0.1f, 1f)] private float squashHeightMultiplier = 0.35f;
    [SerializeField, Min(0f)] private float squashDuration = 3f;
    [SerializeField, Min(0f)] private float squashTransitionDuration = 0.1f;

    private Vector3 originalScale;
    private Coroutine squashCoroutine;

    public bool IsSquashed { get; private set; }

    private void Awake()
    {
        if (visualModel != null)
        {
            originalScale = visualModel.localScale;
        }
    }

    public void ApplySquash()
    {
        if (visualModel == null)
        {
            return;
        }

        if (squashCoroutine != null)
        {
            StopCoroutine(squashCoroutine);
        }

        squashCoroutine = StartCoroutine(SquashRoutine());
    }

    public void ResetSquash()
    {
        if (squashCoroutine != null)
        {
            StopCoroutine(squashCoroutine);
            squashCoroutine = null;
        }

        if (visualModel != null)
        {
            visualModel.localScale = originalScale;
        }

        IsSquashed = false;
    }

    private IEnumerator SquashRoutine()
    {
        IsSquashed = true;

        Vector3 flattenedScale = new Vector3(
            originalScale.x,
            originalScale.y * squashHeightMultiplier,
            originalScale.z
        );

        yield return AnimateScale(
            visualModel.localScale,
            flattenedScale,
            squashTransitionDuration
        );

        yield return new WaitForSeconds(squashDuration);

        yield return AnimateScale(
            visualModel.localScale,
            originalScale,
            squashTransitionDuration
        );

        IsSquashed = false;
        squashCoroutine = null;
    }

    private IEnumerator AnimateScale(
        Vector3 startScale,
        Vector3 targetScale,
        float duration
    )
    {
        if (duration <= 0f)
        {
            visualModel.localScale = targetScale;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            visualModel.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                progress
            );

            yield return null;
        }

        visualModel.localScale = targetScale;
    }
}
