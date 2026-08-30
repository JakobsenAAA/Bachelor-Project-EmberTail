using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonFeedback :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    [Header("References")]
    [SerializeField] private RectTransform visualTarget;
    [SerializeField] private AudioSource audioSource;

    [Header("Scale")]
    [SerializeField] private Vector3 normalScale = Vector3.one;
    [SerializeField] private Vector3 highlightedScale = new Vector3(1.05f, 1.05f, 1f);
    [SerializeField] private Vector3 pressedScale = new Vector3(0.97f, 0.97f, 1f);
    [SerializeField] private float scaleSpeed = 12f;

    [Header("Sound")]
    [SerializeField] private AudioClip highlightSound;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private float highlightVolume = 1f;
    [SerializeField] private float pressVolume = 1f;

    private Vector3 targetScale;
    private bool highlighted;
    private bool pressed;

    private void Awake()
    {
        if (visualTarget == null)
        {
            visualTarget = transform as RectTransform;
        }

        targetScale = normalScale;

        if (visualTarget != null)
        {
            visualTarget.localScale = normalScale;
        }
    }

    private void Update()
    {
        if (visualTarget == null)
        {
            return;
        }

        visualTarget.localScale = Vector3.Lerp(
            visualTarget.localScale,
            targetScale,
            scaleSpeed * Time.unscaledDeltaTime
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighted = false;
        pressed = false;
        UpdateTargetScale();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        UpdateTargetScale();

        PlaySound(
            pressSound,
            pressVolume
        );
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        UpdateTargetScale();
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        highlighted = false;
        pressed = false;
        UpdateTargetScale();
    }

    private void SetHighlighted(bool value)
    {
        bool wasHighlighted = highlighted;

        highlighted = value;
        pressed = false;

        UpdateTargetScale();

        if (highlighted && !wasHighlighted)
        {
            PlaySound(
                highlightSound,
                highlightVolume
            );
        }
    }

    private void UpdateTargetScale()
    {
        if (pressed)
        {
            targetScale = pressedScale;
        }
        else if (highlighted)
        {
            targetScale = highlightedScale;
        }
        else
        {
            targetScale = normalScale;
        }
    }

    private void PlaySound(
        AudioClip clip,
        float volume
    )
    {
        if (
            audioSource == null ||
            clip == null
        )
        {
            return;
        }

        audioSource.PlayOneShot(
            clip,
            volume
        );
    }
}