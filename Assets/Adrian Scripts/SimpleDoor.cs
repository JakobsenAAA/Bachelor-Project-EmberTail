
using System.Collections;
using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform door;

    [Header("Movement")]
    [SerializeField] private Vector3 openOffset =
        new Vector3(0f, 3f, 0f);

    [SerializeField] private float openDuration = 1f;

    [SerializeField] private AnimationCurve openCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpeningSound;
    [SerializeField, Range(0f, 1f)] private float doorOpeningVolume = 0.8f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool opened;
    private Coroutine openCoroutine;

    private void Awake()
    {
        if (door == null)
        {
            door = transform;
        }

        closedPosition = door.localPosition;
        openPosition = closedPosition + openOffset;
    }

    public void Open()
    {
        if (opened)
        {
            return;
        }

        opened = true;

        PlayOpeningSound();

        if (openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
        }

        openCoroutine = StartCoroutine(OpenRoutine());
    }

    private void PlayOpeningSound()
    {
        if (doorAudioSource == null || doorOpeningSound == null)
        {
            return;
        }

        doorAudioSource.PlayOneShot(
            doorOpeningSound,
            doorOpeningVolume
        );
    }

    private IEnumerator OpenRoutine()
    {
        Vector3 startingPosition = door.localPosition;
        float timer = 0f;

        if (openDuration <= 0f)
        {
            door.localPosition = openPosition;
            openCoroutine = null;
            yield break;
        }

        while (timer < openDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / openDuration
            );

            float curvedProgress = openCurve.Evaluate(progress);

            door.localPosition = Vector3.Lerp(
                startingPosition,
                openPosition,
                curvedProgress
            );

            yield return null;
        }

        door.localPosition = openPosition;
        openCoroutine = null;
    }
}
