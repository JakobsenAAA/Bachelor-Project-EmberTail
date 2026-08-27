using UnityEngine;

public class PauseMapDisc : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string discId;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 360f;

    private int currentZoneIndex;
    private int visibleZoneCount = 1;
    private float targetRotationY;

    public string DiscId => discId;
    public int CurrentZoneIndex => currentZoneIndex;

    public void Initialize(int startingZoneIndex, int zoneCount)
    {
        visibleZoneCount = Mathf.Max(1, zoneCount);

        currentZoneIndex =
            Mathf.Clamp(
                startingZoneIndex,
                0,
                visibleZoneCount - 1
            );

        targetRotationY =
            CalculateRotation(currentZoneIndex);

        Vector3 euler = transform.localEulerAngles;
        euler.y = targetRotationY;
        transform.localEulerAngles = euler;
    }

    private void Update()
    {
        Vector3 euler = transform.localEulerAngles;

        float currentY = euler.y;

        float newY =
            Mathf.MoveTowardsAngle(
                currentY,
                targetRotationY,
                rotationSpeed * Time.unscaledDeltaTime
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
        if (visibleZoneCount <= 1)
        {
            return;
        }

        currentZoneIndex--;

        if (currentZoneIndex < 0)
        {
            currentZoneIndex =
                visibleZoneCount - 1;
        }

        targetRotationY =
            CalculateRotation(currentZoneIndex);
    }

    public void RotateRight()
    {
        if (visibleZoneCount <= 1)
        {
            return;
        }

        currentZoneIndex++;

        if (currentZoneIndex >= visibleZoneCount)
        {
            currentZoneIndex = 0;
        }

        targetRotationY =
            CalculateRotation(currentZoneIndex);
    }

    public void SetZone(int zoneIndex)
    {
        if (visibleZoneCount <= 0)
        {
            return;
        }

        currentZoneIndex =
            Mathf.Clamp(
                zoneIndex,
                0,
                visibleZoneCount - 1
            );

        targetRotationY =
            CalculateRotation(currentZoneIndex);
    }

    private float CalculateRotation(int zoneIndex)
    {
        float anglePerZone =
            360f / visibleZoneCount;

        return -anglePerZone * zoneIndex;
    }
}