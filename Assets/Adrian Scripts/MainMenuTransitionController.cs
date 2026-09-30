using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTransitionController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera menuCamera;
    [SerializeField] private MainMenuCameraMovement cameraMovement;
    [SerializeField] private Transform bonfireCameraTarget;

    [Header("Camera Transition")]
    [SerializeField] private float cameraMoveDuration = 1.5f;
    [SerializeField] private AnimationCurve cameraMoveCurve =
        AnimationCurve.EaseInOut(
            0f,
            0f,
            1f,
            1f
        );

    [Header("Timing")]
    [SerializeField] private float holdAtBonfireDuration = 0.25f;

    [Header("UI")]
    [SerializeField] private Button[] buttonsToDisable;

    private bool transitionActive;

    public bool TransitionActive =>
        transitionActive;

    public void StartTransition(Action completedAction)
    {
        if (transitionActive)
        {
            return;
        }

        StartCoroutine(
            TransitionRoutine(
                completedAction
            )
        );
    }

    private IEnumerator TransitionRoutine(
        Action completedAction
    )
    {
        transitionActive = true;

        DisableButtons();

        if (cameraMovement != null)
        {
            cameraMovement
                .SetMovementEnabled(false);
        }

        if (
            menuCamera == null ||
            bonfireCameraTarget == null
        )
        {
            completedAction?.Invoke();
            yield break;
        }

        Transform cameraTransform =
            menuCamera.transform;

        Vector3 startingPosition =
            cameraTransform.position;

        Quaternion startingRotation =
            cameraTransform.rotation;

        float timer = 0f;

        if (cameraMoveDuration <= 0f)
        {
            cameraTransform.SetPositionAndRotation(
                bonfireCameraTarget.position,
                bonfireCameraTarget.rotation
            );
        }
        else
        {
            while (timer < cameraMoveDuration)
            {
                timer +=
                    Time.unscaledDeltaTime;

                float progress =
                    Mathf.Clamp01(
                        timer /
                        cameraMoveDuration
                    );

                float curvedProgress =
                    cameraMoveCurve.Evaluate(
                        progress
                    );

                cameraTransform.position =
                    Vector3.Lerp(
                        startingPosition,
                        bonfireCameraTarget.position,
                        curvedProgress
                    );

                cameraTransform.rotation =
                    Quaternion.Slerp(
                        startingRotation,
                        bonfireCameraTarget.rotation,
                        curvedProgress
                    );

                yield return null;
            }

            cameraTransform.SetPositionAndRotation(
                bonfireCameraTarget.position,
                bonfireCameraTarget.rotation
            );
        }

        if (holdAtBonfireDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                holdAtBonfireDuration
            );
        }

        completedAction?.Invoke();
    }

    private void DisableButtons()
    {
        if (buttonsToDisable == null)
        {
            return;
        }

        for (
            int i = 0;
            i < buttonsToDisable.Length;
            i++
        )
        {
            if (buttonsToDisable[i] != null)
            {
                buttonsToDisable[i]
                    .interactable = false;
            }
        }
    }
}