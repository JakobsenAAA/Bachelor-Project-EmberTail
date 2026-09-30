using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonBurnVFX :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Hover VFX")]
    [SerializeField] private ParticleSystem hoverSmoke;

    [Header("Click VFX")]
    [SerializeField] private ParticleSystem clickSmoke;
    [SerializeField] private ParticleSystem clickEmbers;

    [Header("Options")]
    [SerializeField] private bool stopHoverOnClick = true;

    private bool hasBeenClicked;

    private void Awake()
    {
        StopParticleSystem(
            hoverSmoke,
            true
        );

        StopParticleSystem(
            clickSmoke,
            true
        );

        StopParticleSystem(
            clickEmbers,
            true
        );
    }

    public void OnPointerEnter(
        PointerEventData eventData
    )
    {
        if (hasBeenClicked)
        {
            return;
        }

        if (hoverSmoke != null)
        {
            hoverSmoke.Play();
        }
    }

    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        if (hasBeenClicked)
        {
            return;
        }

        StopParticleSystem(
            hoverSmoke,
            false
        );
    }

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (hasBeenClicked)
        {
            return;
        }

        hasBeenClicked = true;

        if (stopHoverOnClick)
        {
            StopParticleSystem(
                hoverSmoke,
                false
            );
        }

        if (clickSmoke != null)
        {
            clickSmoke.Play();
        }

        if (clickEmbers != null)
        {
            clickEmbers.Play();
        }
    }

    private void StopParticleSystem(
        ParticleSystem particleSystem,
        bool clearParticles
    )
    {
        if (particleSystem == null)
        {
            return;
        }

        ParticleSystemStopBehavior stopBehavior =
            clearParticles
                ? ParticleSystemStopBehavior
                    .StopEmittingAndClear
                : ParticleSystemStopBehavior
                    .StopEmitting;

        particleSystem.Stop(
            true,
            stopBehavior
        );
    }
}