using UnityEngine;

public class PauseMapDisc : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string discId;

    [Header("Sections")]
    [SerializeField] private int sectionCountOverride;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float rotationFinishThreshold = 0.5f;
    [SerializeField] private bool reverseRotation;

    private int currentSectionIndex;
    private int sectionCount = 1;
    private float targetRotationY;

    public string DiscId => discId;
    public int CurrentSectionIndex => currentSectionIndex;
    public int SectionCount => sectionCount;

    public bool IsRotating
    {
        get
        {
            float difference =
                Mathf.Abs(
                    Mathf.DeltaAngle(
                        transform.localEulerAngles.y,
                        targetRotationY
                    )
                );

            return difference >
                rotationFinishThreshold;
        }
    }

    public void Initialize(
        int startingSectionIndex,
        int progressionZoneCount
    )
    {
        if (sectionCountOverride > 0)
        {
            sectionCount =
                sectionCountOverride;
        }
        else
        {
            sectionCount =
                Mathf.Max(
                    1,
                    progressionZoneCount
                );
        }

        currentSectionIndex =
            Mathf.Clamp(
                startingSectionIndex,
                0,
                sectionCount - 1
            );

        targetRotationY =
            CalculateRotation(
                currentSectionIndex
            );

        Vector3 euler =
            transform.localEulerAngles;

        euler.y =
            targetRotationY;

        transform.localEulerAngles =
            euler;
    }

    private void Update()
    {
        Vector3 euler =
            transform.localEulerAngles;

        float newY =
            Mathf.MoveTowardsAngle(
                euler.y,
                targetRotationY,
                rotationSpeed *
                Time.unscaledDeltaTime
            );

        transform.localEulerAngles =
            new Vector3(
                euler.x,
                newY,
                euler.z
            );
    }

    public void RotateLeft()
    {
        if (sectionCount <= 1)
        {
            return;
        }

        currentSectionIndex--;

        if (currentSectionIndex < 0)
        {
            currentSectionIndex =
                sectionCount - 1;
        }

        targetRotationY =
            CalculateRotation(
                currentSectionIndex
            );
    }

    public void RotateRight()
    {
        if (sectionCount <= 1)
        {
            return;
        }

        currentSectionIndex++;

        if (
            currentSectionIndex >=
            sectionCount
        )
        {
            currentSectionIndex = 0;
        }

        targetRotationY =
            CalculateRotation(
                currentSectionIndex
            );
    }

    public void SetSection(
        int sectionIndex
    )
    {
        if (sectionCount <= 0)
        {
            return;
        }

        currentSectionIndex =
            Mathf.Clamp(
                sectionIndex,
                0,
                sectionCount - 1
            );

        targetRotationY =
            CalculateRotation(
                currentSectionIndex
            );
    }

    private float CalculateRotation(
        int sectionIndex
    )
    {
        float anglePerSection =
            360f / sectionCount;

        float direction =
            reverseRotation
                ? 1f
                : -1f;

        return direction *
               anglePerSection *
               sectionIndex;
    }
}