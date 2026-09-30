using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBurnEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Graphic targetGraphic;
    [SerializeField] private Material burnMaterialTemplate;

    [Header("Burn")]
    [SerializeField] private float burnDuration = 1.25f;
    [SerializeField] private AnimationCurve burnCurve =
        AnimationCurve.EaseInOut(
            0f,
            0f,
            1f,
            1f
        );

    private Material burnMaterial;
    private Coroutine burnCoroutine;

    private static readonly int BurnAmount =
        Shader.PropertyToID(
            "_BurnAmount"
        );

    private void Awake()
    {
        if (targetGraphic == null)
        {
            targetGraphic =
                GetComponent<Graphic>();
        }

        if (
            targetGraphic == null ||
            burnMaterialTemplate == null
        )
        {
            return;
        }

        burnMaterial =
            new Material(
                burnMaterialTemplate
            );

        targetGraphic.material =
            burnMaterial;

        burnMaterial.SetFloat(
            BurnAmount,
            0f
        );
    }

    private void OnDestroy()
    {
        if (burnMaterial != null)
        {
            Destroy(
                burnMaterial
            );
        }
    }

    public void Burn()
    {
        if (burnMaterial == null)
        {
            return;
        }

        if (burnCoroutine != null)
        {
            StopCoroutine(
                burnCoroutine
            );
        }

        burnCoroutine =
            StartCoroutine(
                BurnRoutine()
            );
    }

    public void ResetBurn()
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(
                burnCoroutine
            );

            burnCoroutine = null;
        }

        if (burnMaterial != null)
        {
            burnMaterial.SetFloat(
                BurnAmount,
                0f
            );
        }
    }

    private IEnumerator BurnRoutine()
    {
        float timer = 0f;

        burnMaterial.SetFloat(
            BurnAmount,
            0f
        );

        if (burnDuration <= 0f)
        {
            burnMaterial.SetFloat(
                BurnAmount,
                1f
            );

            burnCoroutine = null;

            yield break;
        }

        while (timer < burnDuration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer /
                    burnDuration
                );

            float curvedProgress =
                burnCurve.Evaluate(
                    progress
                );

            burnMaterial.SetFloat(
                BurnAmount,
                curvedProgress
            );

            yield return null;
        }

        burnMaterial.SetFloat(
            BurnAmount,
            1f
        );

        burnCoroutine = null;
    }
}