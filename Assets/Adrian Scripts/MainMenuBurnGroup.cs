using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenuBurnGroup : MonoBehaviour
{
    [Header("Burn Effects")]
    [SerializeField] private MainMenuBurnEffect[] burnEffects;

    [Header("Text")]
    [SerializeField] private TMP_Text[] textElements;

    [Header("Timing")]
    [SerializeField] private float textFadeDuration = 0.8f;
    [SerializeField] private float textFadeDelay = 0.15f;

    private Coroutine burnCoroutine;

    public void Burn()
    {
        if (burnCoroutine != null)
        {
            return;
        }

        for (int i = 0; i < burnEffects.Length; i++)
        {
            if (burnEffects[i] != null)
            {
                burnEffects[i].Burn();
            }
        }

        burnCoroutine =
            StartCoroutine(
                FadeTextRoutine()
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

        for (int i = 0; i < burnEffects.Length; i++)
        {
            if (burnEffects[i] != null)
            {
                burnEffects[i].ResetBurn();
            }
        }

        for (int i = 0; i < textElements.Length; i++)
        {
            if (textElements[i] == null)
            {
                continue;
            }

            Color color =
                textElements[i].color;

            color.a = 1f;

            textElements[i].color =
                color;
        }
    }

    private IEnumerator FadeTextRoutine()
    {
        if (textFadeDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                textFadeDelay
            );
        }

        float[] startingAlpha =
            new float[textElements.Length];

        for (int i = 0; i < textElements.Length; i++)
        {
            if (textElements[i] != null)
            {
                startingAlpha[i] =
                    textElements[i].color.a;
            }
        }

        float timer = 0f;

        while (timer < textFadeDuration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer /
                    textFadeDuration
                );

            for (int i = 0; i < textElements.Length; i++)
            {
                if (textElements[i] == null)
                {
                    continue;
                }

                Color color =
                    textElements[i].color;

                color.a =
                    Mathf.Lerp(
                        startingAlpha[i],
                        0f,
                        progress
                    );

                textElements[i].color =
                    color;
            }

            yield return null;
        }

        for (int i = 0; i < textElements.Length; i++)
        {
            if (textElements[i] == null)
            {
                continue;
            }

            Color color =
                textElements[i].color;

            color.a = 0f;

            textElements[i].color =
                color;
        }

        burnCoroutine = null;
    }
}