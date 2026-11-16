
using UnityEngine;

public class AnimatedLever : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private AnimatedDoor targetDoor;

    [Header("Animation")]
    [SerializeField] private string animationStateName = "LeverPull";
    [SerializeField, Min(0.01f)] private float animationDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pullSound;
    [SerializeField, Range(0f, 1f)] private float pullVolume = 1f;

    private int animationStateHash;
    private float animationProgress;
    private bool isPulled;
    private bool isAnimating;

    private void Awake()
    {
        if (leverAnimator == null)
        {
            leverAnimator = GetComponentInChildren<Animator>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        animationStateHash = Animator.StringToHash(animationStateName);
    }

    private void Start()
    {
        if (leverAnimator == null)
        {
            return;
        }

        leverAnimator.speed = 0f;
        leverAnimator.Play(animationStateHash, 0, 0f);
        leverAnimator.Update(0f);
    }

    private void Update()
    {
        if (!isAnimating || leverAnimator == null)
        {
            return;
        }

        float direction = isPulled ? 1f : -1f;

        animationProgress +=
            direction * Time.deltaTime / animationDuration;

        animationProgress = Mathf.Clamp01(animationProgress);

        leverAnimator.Play(
            animationStateHash,
            0,
            animationProgress
        );

        leverAnimator.Update(0f);

        if (animationProgress <= 0f || animationProgress >= 1f)
        {
            isAnimating = false;
        }
    }

    public void PullLever()
    {
        if (isAnimating || leverAnimator == null || targetDoor == null)
        {
            return;
        }

        if (targetDoor.IsAnimating)
        {
            return;
        }

        targetDoor.ToggleDoor();

        isPulled = !isPulled;
        isAnimating = true;

        if (audioSource != null && pullSound != null)
        {
            audioSource.PlayOneShot(pullSound, pullVolume);
        }
    }
}
