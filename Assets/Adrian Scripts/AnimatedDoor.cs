
using UnityEngine;

public class AnimatedDoor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator doorAnimator;

    [Header("Animation")]
    [SerializeField] private string animationStateName = "DoorOpen";
    [SerializeField, Min(0.01f)] private float animationDuration = 1f;
    [SerializeField] private bool startOpen;
    [SerializeField] private bool blockInteractionDuringAnimation = true;

    [Header("Audio")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip openingSound;
    [SerializeField] private AudioClip closingSound;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 0.8f;

    private bool isOpen;
    private bool isAnimating;
    private float animationProgress;
    private int animationStateHash;

    public bool IsOpen => isOpen;
    public bool IsAnimating => isAnimating;

    private void Awake()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponentInChildren<Animator>();
        }

        if (doorAudioSource == null)
        {
            doorAudioSource = GetComponent<AudioSource>();
        }

        animationStateHash = Animator.StringToHash(animationStateName);
        isOpen = startOpen;
        animationProgress = startOpen ? 1f : 0f;
    }

    private void Start()
    {
        if (doorAnimator == null)
        {
            return;
        }

        doorAnimator.speed = 0f;
        doorAnimator.Play(animationStateHash, 0, animationProgress);
        doorAnimator.Update(0f);
    }

    private void Update()
    {
        if (!isAnimating || doorAnimator == null)
        {
            return;
        }

        float direction = isOpen ? 1f : -1f;

        animationProgress +=
            direction * Time.deltaTime / animationDuration;

        animationProgress = Mathf.Clamp01(animationProgress);

        doorAnimator.Play(animationStateHash, 0, animationProgress);
        doorAnimator.Update(0f);

        if (animationProgress <= 0f || animationProgress >= 1f)
        {
            isAnimating = false;
        }
    }

    public void ToggleDoor()
    {
        if (doorAnimator == null)
        {
            return;
        }

        if (isAnimating && blockInteractionDuringAnimation)
        {
            return;
        }

        isOpen = !isOpen;
        isAnimating = true;

        PlayDoorSound();
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            ToggleDoor();
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            ToggleDoor();
        }
    }

    private void PlayDoorSound()
    {
        if (doorAudioSource == null)
        {
            return;
        }

        AudioClip clip = isOpen ? openingSound : closingSound;

        if (clip != null)
        {
            doorAudioSource.PlayOneShot(clip, soundVolume);
        }
    }
}
