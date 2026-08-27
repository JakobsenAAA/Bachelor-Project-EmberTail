using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private CollectibleManager collectibleManager;

    [Header("Discs")]
    [SerializeField] private PauseMapDisc[] discObjects;

    [Header("Disc Stack")]
    [SerializeField] private Transform discStack;
    [SerializeField] private float discSpacing = 1.2f;
    [SerializeField] private float stackMoveSpeed = 6f;

    [Header("Current Player Location")]
    [SerializeField] private string currentPlayerZoneId = "zone1";

    private int selectedDiscIndex;
    private Vector3 stackStartPosition;
    private Vector3 targetStackPosition;

    private void Awake()
    {
        if (collectibleManager == null)
        {
            collectibleManager =
                CollectibleManager.Instance;
        }

        if (discStack != null)
        {
            stackStartPosition =
                discStack.localPosition;

            targetStackPosition =
                stackStartPosition;
        }
    }

    private void Start()
    {
        InitializeDiscs();
        FocusPlayerLocation();
    }

    private void Update()
    {
        if (
            pauseManager == null ||
            !pauseManager.IsPaused
        )
        {
            return;
        }

        HandleInput();
        UpdateStackPosition();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            RotateLeft();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            RotateRight();
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            MoveDiscUp();
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            MoveDiscDown();
        }
    }

    private void InitializeDiscs()
    {
        if (collectibleManager == null)
        {
            return;
        }

        for (int i = 0; i < discObjects.Length; i++)
        {
            PauseMapDisc discObject =
                discObjects[i];

            if (discObject == null)
            {
                continue;
            }

            ProgressionDiscDefinition definition =
                collectibleManager.GetDisc(
                    discObject.DiscId
                );

            if (definition == null)
            {
                continue;
            }

            int visibleZoneCount = 0;

            for (
                int zoneIndex = 0;
                zoneIndex < definition.Zones.Count;
                zoneIndex++
            )
            {
                ZoneProgressDefinition zone =
                    definition.Zones[zoneIndex];

                if (
                    zone != null &&
                    zone.ShowInProgression
                )
                {
                    visibleZoneCount++;
                }
            }

            discObject.Initialize(
                0,
                visibleZoneCount
            );
        }
    }

    public void FocusPlayerLocation()
    {
        if (collectibleManager == null)
        {
            return;
        }

        int discIndex =
            collectibleManager.GetDiscIndexContainingZone(
                currentPlayerZoneId
            );

        if (discIndex < 0)
        {
            return;
        }

        selectedDiscIndex =
            Mathf.Clamp(
                discIndex,
                0,
                discObjects.Length - 1
            );

        int zoneIndex =
            GetVisibleZoneIndex(
                selectedDiscIndex,
                currentPlayerZoneId
            );

        if (
            selectedDiscIndex >= 0 &&
            selectedDiscIndex < discObjects.Length &&
            discObjects[selectedDiscIndex] != null &&
            zoneIndex >= 0
        )
        {
            discObjects[selectedDiscIndex]
                .SetZone(zoneIndex);
        }

        UpdateTargetStackPosition();
    }

    private int GetVisibleZoneIndex(
        int discIndex,
        string zoneId
    )
    {
        if (
            collectibleManager == null ||
            discIndex < 0 ||
            discIndex >= collectibleManager.Discs.Count
        )
        {
            return -1;
        }

        ProgressionDiscDefinition disc =
            collectibleManager.Discs[discIndex];

        int visibleIndex = 0;

        for (
            int i = 0;
            i < disc.Zones.Count;
            i++
        )
        {
            ZoneProgressDefinition zone =
                disc.Zones[i];

            if (
                zone == null ||
                !zone.ShowInProgression
            )
            {
                continue;
            }

            if (zone.ZoneId == zoneId)
            {
                return visibleIndex;
            }

            visibleIndex++;
        }

        return -1;
    }

    private void RotateLeft()
    {
        if (!HasSelectedDisc())
        {
            return;
        }

        discObjects[selectedDiscIndex]
            .RotateLeft();
    }

    private void RotateRight()
    {
        if (!HasSelectedDisc())
        {
            return;
        }

        discObjects[selectedDiscIndex]
            .RotateRight();
    }

    private void MoveDiscUp()
    {
        selectedDiscIndex--;

        if (selectedDiscIndex < 0)
        {
            selectedDiscIndex =
                discObjects.Length - 1;
        }

        UpdateTargetStackPosition();
    }

    private void MoveDiscDown()
    {
        selectedDiscIndex++;

        if (
            selectedDiscIndex >=
            discObjects.Length
        )
        {
            selectedDiscIndex = 0;
        }

        UpdateTargetStackPosition();
    }

    private void UpdateTargetStackPosition()
    {
        if (discStack == null)
        {
            return;
        }

        targetStackPosition =
            stackStartPosition +
            Vector3.up *
            selectedDiscIndex *
            discSpacing;
    }

    private void UpdateStackPosition()
    {
        if (discStack == null)
        {
            return;
        }

        discStack.localPosition =
            Vector3.MoveTowards(
                discStack.localPosition,
                targetStackPosition,
                stackMoveSpeed *
                Time.unscaledDeltaTime
            );
    }

    private bool HasSelectedDisc()
    {
        return
            discObjects != null &&
            selectedDiscIndex >= 0 &&
            selectedDiscIndex < discObjects.Length &&
            discObjects[selectedDiscIndex] != null;
    }
}